using System;
using Unity.Netcode;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(AutoInjectOnAwake))]
public class PickUp : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform grabPoint;
    private GameInputReader _inputReader;
    public PickableObject currentPickableObject { get; private set; }

    [SerializeField] private float maxPickupDistance = 10f;
    [Inject]
    private void Init(GameInputReader inputReader)
    {
        _inputReader = inputReader;

    }
    public override void OnNetworkSpawn()
    {

        if (!IsOwner) return;
        _inputReader.InteractEvent += OnPickUp;
    }

    private void OnPickUp()
    {
        if (currentPickableObject == null)
        {
            PickUpObject();
        }
        else
        {
            DropObject();
        }
    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _inputReader.InteractEvent -= OnPickUp;
    }
    private void PickUpObject()
    {
            RaycastHit hit;
            Debug.DrawRay(transform.position, transform.forward * maxPickupDistance, Color.green, 1f);
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxPickupDistance))
        {
            if (hit.collider.TryGetComponent(out PickableObject pickableObject))
            {
                pickableObject.SetPickUpState(true, grabPoint, transform);

                currentPickableObject = pickableObject;
            }
        }
    }
    private void DropObject()
    {
        if (currentPickableObject != null)
        {
            currentPickableObject.SetPickUpState(false);
            currentPickableObject = null;
        }
    }

}
