using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool DashWasPressed;
    public static bool InteractWasPressed;

    public static bool PauseWasPressed;
    public static bool MenuBackWasPressed;

    public static bool IsMenuInput { get; private set; }

    private static int _ignoreInputThroughFrame = -1;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAction;
    private InputAction _interactAction;
    private InputAction _pauseAction;
    private InputAction _menuBackAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Player/Move"];
        _jumpAction = PlayerInput.actions["Player/Jump"];
        _runAction = PlayerInput.actions["Player/Run"];
        _dashAction = PlayerInput.actions["Player/Dash"];
        _interactAction = PlayerInput.actions["Player/Interact"];
        _pauseAction = PlayerInput.actions["Player/Pause"];

        _menuBackAction = PlayerInput.actions["UI/Cancel"];

        IsMenuInput = false;
        _ignoreInputThroughFrame = -1;

        ClearGameplayInput();
        PauseWasPressed = false;
        MenuBackWasPressed = false;
    }

    private void Start()
    {
        SetMenuInput(
            SceneManager.GetActiveScene().name == "MainMenu");
    }

    private void Update()
    {
        ClearGameplayInput();
        PauseWasPressed = false;
        MenuBackWasPressed = false;

        // Prevent the press that closes a menu from also
        // becoming a gameplay action during the switch.
        if (Time.frameCount <= _ignoreInputThroughFrame)
            return;

        if (IsMenuInput)
        {
            MenuBackWasPressed =
                _menuBackAction.WasPressedThisFrame();

            return;
        }

        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        RunIsHeld = _runAction.IsPressed();
        DashWasPressed = _dashAction.WasPressedThisFrame();
        InteractWasPressed = _interactAction.WasPressedThisFrame();

        PauseWasPressed = _pauseAction.WasPressedThisFrame();
    }

    public static void SetMenuInput(bool menuOpen)
    {
        if (PlayerInput == null || !PlayerInput.isActiveAndEnabled || !PlayerInput.inputIsActive)
        {
            return;
        }

        IsMenuInput = menuOpen;

        ClearGameplayInput();
        PauseWasPressed = false;
        MenuBackWasPressed = false;

        PlayerInput.SwitchCurrentActionMap(
            menuOpen ? "UI" : "Player");

        _ignoreInputThroughFrame = Time.frameCount + 1;
    }

    private static void ClearGameplayInput()
    {
        Movement = Vector2.zero;
        JumpWasPressed = false;
        JumpIsHeld = false;
        JumpWasReleased = false;
        RunIsHeld = false;
        DashWasPressed = false;
        InteractWasPressed = false;
    }

    public static bool IsUsingGamepad =>
        PlayerInput != null &&
        PlayerInput.currentControlScheme == "Gamepad";

    public static string CurrentPrompt(
        string keyboardText,
        string gamepadText)
    {
        return IsUsingGamepad ? gamepadText : keyboardText;
    }

    private void OnDisable()
    {
        ClearGameplayInput();
        PauseWasPressed = false;
        MenuBackWasPressed = false;
    }
}