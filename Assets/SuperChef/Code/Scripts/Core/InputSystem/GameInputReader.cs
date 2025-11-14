using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameInputReader : InputActions.IPlayerActions, InputActions.IGameSceneActions, InputActions.IUIActions, IDisposable
{
    public Action<Vector2> MoveEvent;
    public Action JumpEvent;
    public Action<Vector2> LookEvent;

    public Action AttackEvent;

    public Action CrouchEvent;
    public Action SprintEvent;
    public Action InteractEvent;
    public Action OpenInventoryEvent;
    public Action PreviousEvent;
    public Action NextEvent;

    public InputActions InputActions { get; private set; }
    private InputActionMap[] defaultInputActionMap;

    [Inject]
    private void Init(InputActions inputActions)
    {
        InputActions = inputActions;
        defaultInputActionMap = new InputActionMap[] { InputActions.GameScene, InputActions.Global };
        InputActions.GameScene.SetCallbacks(this);
        InputActions.Player.SetCallbacks(this);
        InputActions.Player.Enable();
        InputActions.GameScene.Enable();

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
        if (context.performed)
            NextEvent?.Invoke();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed)
            PreviousEvent?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Debug.Log("Sprint");
    }

    public void Dispose()
    {
        if (InputActions != null)
        {
            DisableActionMapsExceptSpecified();
            ClearAllCallbacks();
            Debug.Log("GameInputReader disposed and inputs disabled");
        }
    }

    private void DisableActionMapsExceptSpecified(InputActionMap[] excludeActionMaps = null)
    {
        foreach (var actionMap in InputActions.asset.actionMaps)
        {
            if (ShouldDisableActionMap(actionMap, excludeActionMaps))
            {
                actionMap.Disable();
            }
        }
    }
    private bool ShouldDisableActionMap(InputActionMap actionMap, InputActionMap[] excludeActionMaps)
    {
        if (excludeActionMaps == null || excludeActionMaps.Length == 0)
        {
            return true; // Disable if no exclusions are specified
        }

        foreach (var exclude in excludeActionMaps)
        {
            if (actionMap == exclude)
            {
                return false;
            }
        }

        return true;
    }
    private void ClearAllCallbacks()
    {
        InputActions.Player.SetCallbacks(null);
        InputActions.GameScene.SetCallbacks(null);
    }
    public void EnableInputActionMap(InputActionMap actionMap)
    {
        DisableActionMapsExceptSpecified(defaultInputActionMap);
        actionMap?.Enable();
    }
    public void EnableInputActionMaps(InputActionMap[] actionMaps)
    {
        DisableActionMapsExceptSpecified(defaultInputActionMap);
        foreach (var actionMap in actionMaps)
        {
            actionMap?.Enable();
        }
    }
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OpenInventoryEvent?.Invoke();

        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}
