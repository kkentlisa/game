using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance {  get; private set; }

    public event Action OnAttack;

    private PlayerInputActions playerInputActions;
    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Attack.Enable();

        playerInputActions.Attack.Hit.performed += _ => OnAttack?.Invoke();
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        return mousePosition;
    }
}
