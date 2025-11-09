using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class BuildingManager : NetworkBehaviour
{
    [SerializeField] private BuildableObject currentBuildableObjectPrefab;
    [SerializeField] private Material canPlaceMaterial;
    [SerializeField] private Material canNotPlaceMaterial;
    [SerializeField] private float maxPlaceObjectDistance = 5f;

    private GameInputReader inputReader;
    private BuildableObject visualizeBuildableObject;
    private Parentable latestParentable = null;
    private bool canPlaceHere;
    [Inject]
    private void Init(GameInputReader inputReader)
    {
        this.inputReader = inputReader;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.AttackEvent += OnTryBuildObject;
        StartVisualizeBuildableObject();
    }

    public override void OnNetworkDespawn()
    {
        if (inputReader != null)
            inputReader.AttackEvent -= OnTryBuildObject;

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
        visualizeBuildableObject = Instantiate(currentBuildableObjectPrefab);

        visualizeBuildableObject.GetComponent<Collider>().isTrigger = true;
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
        int placementLayerMask = currentBuildableObjectPrefab.ActiveLayerMask.value;
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

            visualizeBuildableObject.SetGhostMaterial(canPlaceHere ? canPlaceMaterial : canNotPlaceMaterial);
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

private bool IsOccupied(BuildableObject ghost)
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
        if (!canPlaceHere || visualizeBuildableObject == null)
            return;

        Vector3 placePos = visualizeBuildableObject.transform.position;
        Quaternion placeRot = visualizeBuildableObject.transform.rotation;

        if (IsServer)
        {
            SpawnObject(placePos, placeRot);
        }
        else
        {
            RequestSpawnServerRpc(placePos, placeRot, latestParentable.TryGetComponent(out NetworkObjectReference parentRef)
                ? parentRef
                : default);
  
        }
    }

    [ServerRpc]
    private void RequestSpawnServerRpc(Vector3 position, Quaternion rotation, NetworkObjectReference parentRef)
    {
        SpawnObject(position, rotation, parentRef);
    }

    private void SpawnObject(Vector3 position, Quaternion rotation, NetworkObjectReference parentRef = default)
    {
        var newObj = Instantiate(currentBuildableObjectPrefab, position, rotation);
        newObj.NetworkObject.Spawn(true);

        if (parentRef.TryGet(out var parentBehaviour))
        {
            newObj.NetworkObject.TrySetParent(parentBehaviour.transform);
        }
        else if (latestParentable != null)
        {
            newObj.NetworkObject.TrySetParent(latestParentable.transform);
        }
    }
}
