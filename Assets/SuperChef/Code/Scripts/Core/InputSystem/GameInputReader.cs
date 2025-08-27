using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameInputReader : InputActions.IPlayerActions
{
    public Action<Vector2> MoveEvent;
    public Action JumpEvent;
    public Action<Vector2> LookEvent;

    public Action AttackEvent;

    public Action CrouchEvent;
    public Action SprintEvent;

    [Inject]
    private void Init(InputActions inputActions)
    {
        inputActions.Player.SetCallbacks(this);
        inputActions.Player.Enable();
        Debug.Log("GameInputReader Init");
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack");
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        Debug.Log("Crouch");
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            JumpEvent?.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        Debug.Log("Next");
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        Debug.Log("Previous");  
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Debug.Log("Sprint");
    }
}
