using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public UnityEvent<Vector2> onMovementInput;
    public UnityEvent<InputAction.CallbackContext> onFire;
    public UnityEvent<InputAction.CallbackContext> onFire2;
    public UnityEvent<Vector2> onLook;
    public UnityEvent onShiftInput;
    private PlayerInput playerInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        Vector2 movementInput = value.ReadValue<Vector2>();
        if (onMovementInput != null)
        {
            int listenerCount = onMovementInput.GetPersistentEventCount();
            onMovementInput.Invoke(movementInput);
        }
    }

    public void OnFire(InputAction.CallbackContext value)
    {
        onFire?.Invoke(value);
    }
    
    public void OnFire2(InputAction.CallbackContext value)
    {
        onFire2?.Invoke(value);
    }
    
    public void OnLook(InputAction.CallbackContext value)
    {
        Vector2 dir = value.ReadValue<Vector2>();
        onLook?.Invoke(dir);
    }


    public void OnShift(InputAction.CallbackContext value)
    {
        onShiftInput.Invoke();
    }

    public string GetCurrentControlSchema()
    {
        return playerInput.currentControlScheme;
    }
}