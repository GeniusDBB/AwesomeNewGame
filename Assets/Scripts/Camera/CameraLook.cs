using UnityEngine;
using Unity.Cinemachine;

/// <summary>Limited, player-controlled look around the camera's normal framing.</summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(CinemachinePositionComposer))]
public class CameraLook : CinemachineExtension
{
    [Tooltip("Maximum horizontal shift as a fraction of the screen width.")]
    [SerializeField, Range(0f, 0.25f)] private float _horizontalAmount = 0.08f;

    [Tooltip("Maximum vertical shift as a fraction of the screen height. Ignored on fixed-height cameras.")]
    [SerializeField, Range(0f, 0.25f)] private float _verticalAmount = 0.1f;

    [SerializeField, Min(0.01f)] private float _lookSmoothTime = 0.2f;
    [SerializeField, Min(0.01f)] private float _returnSmoothTime = 0.3f;

    private CinemachinePositionComposer _composer;
    private CameraFixedHeight _fixedHeight;
    private Transform _followTarget;
    private PlayerMovement _player;
    private Vector2 _offset;
    private Vector2 _velocity;
    private Vector2 _originalScreenPosition;
    private bool _framingApplied;

    public override void PrePipelineMutateCameraStateCallback(
        CinemachineVirtualCameraBase vcam, ref CameraState state, float deltaTime)
    {
        RestoreFraming();

        if (!Application.isPlaying || vcam != ComponentOwner)
            return;

        if (_composer == null)
        {
            _composer = GetComponent<CinemachinePositionComposer>();
            _fixedHeight = GetComponent<CameraFixedHeight>();
        }

        if (_composer == null || !_composer.IsValid)
        {
            ResetLook();
            return;
        }

        if (_followTarget != vcam.Follow)
        {
            _followTarget = vcam.Follow;
            _player = _followTarget != null
                ? _followTarget.GetComponentInParent<PlayerMovement>()
                : null;
            ResetLook();
        }

        if (deltaTime < 0f || !vcam.PreviousStateIsValid)
            ResetLook();

        bool canLook = _player != null && _player.CanLookAround &&
            !InputManager.IsMenuInput && InputManager.PlayerInput != null &&
            InputManager.PlayerInput.isActiveAndEnabled &&
            InputManager.PlayerInput.inputIsActive && CinemachineCore.IsLive(vcam);

        if (!canLook)
        {
            ResetLook();
        }
        else if (deltaTime > 0f)
        {
            // The right stick already has the Input System's stick dead zone.
            Vector2 input = Vector2.ClampMagnitude(InputManager.CameraLook, 1f);
            Vector2 desired = Vector2.Scale(
                input, new Vector2(_horizontalAmount, _verticalAmount));

            _offset = Vector2.SmoothDamp(
                _offset, desired, ref _velocity,
                input.sqrMagnitude > 0f ? _lookSmoothTime : _returnSmoothTime,
                Mathf.Infinity, deltaTime);
        }

        // Clear both displacement and smoothing momentum on the locked axis.
        // CameraFixedHeight remains the final authority on world-space Y.
        if (_fixedHeight != null && _fixedHeight.isActiveAndEnabled)
        {
            _offset.y = 0f;
            _velocity.y = 0f;
        }

        // Change composition before the body/confiner stages, so bounds still
        // apply. Screen-space framing also keeps directions independent of
        // the player's facing rotation and avoids the follow dead zone.
        _originalScreenPosition = _composer.Composition.ScreenPosition;
        _composer.Composition.ScreenPosition += new Vector2(-_offset.x, _offset.y);
        _framingApplied = true;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage,
        ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
            RestoreFraming();
    }

    public override bool OnTransitionFromCamera(
        ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
    {
        ResetLook();
        return base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
    }

    public override void OnTargetObjectWarped(
        CinemachineVirtualCameraBase vcam, Transform target, Vector3 positionDelta)
    {
        base.OnTargetObjectWarped(vcam, target, positionDelta);
        if (target == vcam.Follow)
            ResetLook();
    }

    private void ResetLook()
    {
        _offset = Vector2.zero;
        _velocity = Vector2.zero;
    }

    private void RestoreFraming()
    {
        if (_framingApplied && _composer != null)
            _composer.Composition.ScreenPosition = _originalScreenPosition;

        _framingApplied = false;
    }

    private void OnDisable()
    {
        RestoreFraming();
        ResetLook();
    }
}
