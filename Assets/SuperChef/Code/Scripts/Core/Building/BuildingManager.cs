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
    private InventoryHelper inventoryHelper;

    private InventoryItemProviderSO inventoryItemProviderSO;
    [Inject]
    private void Init(GameInputReader inputReader, Inventory inventory, InventoryItemProviderSO inventoryItemProviderSO, InventoryHelper inventoryHelper)
    {
        this.inputReader = inputReader;
        this.inventory = inventory;
        this.inventoryItemProviderSO = inventoryItemProviderSO;
        this.inventoryHelper = inventoryHelper;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
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
        int placementLayerMask = currentBuildableObjectSO.ActiveLayerMask.value;
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



    public bool OnTryBuildObject()
    {
        if (!canPlaceHere || currentBuildableObjectSO == null)
            return false;

        visualizeBuildableObject.transform.GetPositionAndRotation(out var placePos, out var placeRot);
        var buildableObjectSOID = currentBuildableObjectSO.ID;

        NetworkObjectReference parentRef = default;
        if (latestParentable != null && latestParentable.TryGetComponent(out NetworkObject parentNetObj))
        {
            parentRef = parentNetObj;
        }

        inventoryHelper.RequestSpawnInventoryItemServerRpc(buildableObjectSOID, placePos, placeRot, parentRef, true);

        inventory.DecrementSelectedSlot();
        StartVisualizeBuildableObject();
        return true;
    }



}
