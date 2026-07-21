using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool DashWasPressed;

    //Interact
    public static bool InteractWasPressed;
    private InputAction _interactAction;

    //Pause Menu
    public static bool PauseWasPressed;
    private InputAction _pauseAction;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];

        _dashAction = PlayerInput.actions["Dash"];

        _interactAction = PlayerInput.actions["Interact"];

        _pauseAction = PlayerInput.actions["Pause"];
    }
    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        RunIsHeld = _runAction.IsPressed();

        DashWasPressed = _dashAction.WasPressedThisFrame();

        InteractWasPressed = _interactAction.WasPressedThisFrame();

        PauseWasPressed = _pauseAction.WasPressedThisFrame();
    }
}
