using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class BuildingManager : NetworkBehaviour
{
    [SerializeField] private Material canPlaceMaterial;
    [SerializeField] private Material canNotPlaceMaterial;
    [SerializeField] private float maxPlaceObjectDistance = 5f;
    [SerializeField] private LayerMask ghostLayerMask;
    [SerializeField] private BuildableObjectSO currentBuildableObjectSO;

    private GameInputReader inputReader;
    private GameObject visualizeBuildableObject;
    private Parentable latestParentable = null;
    private bool canPlaceHere;
    private Inventory inventory;

    [Inject]
    private void Init(GameInputReader inputReader, Inventory inventory)
    {
        this.inputReader = inputReader;
        this.inventory = inventory;

    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.AttackEvent += OnTryBuildObject;
        inventory.OnSelectedSlotIndexChanged += ChangeCurrentBuildable;
        inventory.OnInventoryChanged += InventoryChanged;
        currentBuildableObjectSO = inventory.InventorySlots[0].InventoryItemSO as BuildableObjectSO;
        StartVisualizeBuildableObject();
    }

    private void InventoryChanged()
    {
        StartVisualizeBuildableObject();
    }

    private void ChangeCurrentBuildable()
    {
        StartVisualizeBuildableObject();
    }

    public override void OnNetworkDespawn()
    {
        if (inputReader != null)
            inputReader.AttackEvent -= OnTryBuildObject;
        if (inventory != null)
        {
            inventory.OnSelectedSlotIndexChanged -= ChangeCurrentBuildable;
            inventory.OnInventoryChanged -= InventoryChanged;
        }
        StopVisualizeBuildableObject();
    }

    private void Update()
    {
        if (!IsOwner || visualizeBuildableObject == null) return;
        VisualizeBuildableObject();
    }

    private void StartVisualizeBuildableObject()
    {
        StopVisualizeBuildableObject();
        currentBuildableObjectSO = inventory.InventorySlots[inventory.SelectedInventorySlotIndex].InventoryItemSO as BuildableObjectSO;
        if (currentBuildableObjectSO == null) return;
        visualizeBuildableObject = Instantiate(currentBuildableObjectSO.BuildableObjectGhostPrefab);

        var collider = visualizeBuildableObject.GetComponent<Collider>();
        if (collider != null) collider.isTrigger = true;
        int ghostLayer = ghostLayerMask.value == 0 ? 0 : (int)Mathf.Log(ghostLayerMask.value, 2);
        visualizeBuildableObject.gameObject.layer = ghostLayer;
    }

    private void StopVisualizeBuildableObject()
    {
        if (visualizeBuildableObject != null)
        {
            Destroy(visualizeBuildableObject.gameObject);
            visualizeBuildableObject = null;
        }
    }

    private void VisualizeBuildableObject()
    {
        if (visualizeBuildableObject == null) return;
        int placementLayerMask = currentBuildableObjectSO.BuildableObject.ActiveLayerMask.value;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, maxPlaceObjectDistance, placementLayerMask))
        {
            latestParentable = hit.collider.TryGetComponent(out Parentable parentable) ? parentable : null;

            if (!visualizeBuildableObject.gameObject.activeSelf)
            {
                visualizeBuildableObject.gameObject.SetActive(true);
            }

            visualizeBuildableObject.transform.position = hit.point;
            visualizeBuildableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);


            canPlaceHere = !IsOccupied(visualizeBuildableObject);

            SetGhostMaterial();
        }
        else
        {
            if (visualizeBuildableObject.gameObject.activeSelf)
            {
                visualizeBuildableObject.gameObject.SetActive(false);
            }

            canPlaceHere = false;
        }
    }

    private void SetGhostMaterial()
    {
        var renderers = visualizeBuildableObject.GetComponentsInChildren<Renderer>();
        var mat = canPlaceHere ? canPlaceMaterial : canNotPlaceMaterial;
        foreach (var r in renderers)
        {
            r.material = mat;
        }
    }

    private bool IsOccupied(GameObject ghost)
    {
        Collider ghostCollider = ghost.GetComponentInChildren<Collider>();
        if (ghostCollider == null) return false; // if no collider, skip

        // Get collider's position and size in world space
        Bounds bounds = ghostCollider.bounds;
        Vector3 center = bounds.center;
        Vector3 halfExtents = bounds.extents;

        // Check overlapping colliders
        Collider[] overlaps = Physics.OverlapBox(center, halfExtents, ghost.transform.rotation);

        foreach (Collider col in overlaps)
        {
            if (col.transform.IsChildOf(ghost.transform))
                continue;

            // If we hit another BuildableObject, it's occupied
            if (col.GetComponent<BuildableObject>() != null)
                return true;
        }

        return false;
    }



    private void OnTryBuildObject()
    {
        if (!canPlaceHere || currentBuildableObjectSO == null)
            return;

        Vector3 placePos = visualizeBuildableObject.transform.position;
        Quaternion placeRot = visualizeBuildableObject.transform.rotation;

        if (IsServer)
        {
            SpawnObject(placePos, placeRot);
        }
        else
        {
            NetworkObjectReference parentRef = default;
            if (latestParentable != null && latestParentable.TryGetComponent(out NetworkObject parentNetObj))
            {
                parentRef = parentNetObj;
            }

            RequestSpawnServerRpc(placePos, placeRot, parentRef);
        }

        inventory.DecrementSelectedSlot();
        StartVisualizeBuildableObject();
    }

    [ServerRpc]
    private void RequestSpawnServerRpc(Vector3 position, Quaternion rotation, NetworkObjectReference parentRef)
    {
        SpawnObject(position, rotation, parentRef);
    }

    private void SpawnObject(Vector3 position, Quaternion rotation, NetworkObjectReference parentRef = default)
    {
        var newObj = Instantiate(currentBuildableObjectSO.BuildableObject, position, rotation);
        newObj.NetworkObject.Spawn(true);

        if (parentRef.TryGet(out NetworkObject parentNetworkObject))
        {
            newObj.NetworkObject.TrySetParent(parentNetworkObject.transform);
        }
        else if (latestParentable != null)
        {
            newObj.NetworkObject.TrySetParent(latestParentable.transform);
        }

        if (newObj.TryGetComponent(out BuildableObject buildableObject))
        {
            buildableObject.NotifyBuildingObjectPlacedClientRpc();
        }
    }

}
