using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameInputReader : InputActions.IPlayerActions, IDisposable
{
    public Action<Vector2> MoveEvent;
    public Action JumpEvent;
    public Action<Vector2> LookEvent;

    public Action AttackEvent;

    public Action CrouchEvent;
    public Action SprintEvent;
    public Action InteractEvent;

    private InputActions inputActions;
    [Inject]
    private void Init(InputActions inputActions)
    {
        this.inputActions = inputActions;
        this.inputActions.Player.SetCallbacks(this);
        this.inputActions.Player.Enable();
        Debug.Log("GameInputReader Init");
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttackEvent?.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        Debug.Log("Crouch");
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            InteractEvent?.Invoke();
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

    public void Dispose()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable();
            inputActions.Player.SetCallbacks(null);
            Debug.Log("GameInputReader disposed and inputs disabled");
        }
    }
}
