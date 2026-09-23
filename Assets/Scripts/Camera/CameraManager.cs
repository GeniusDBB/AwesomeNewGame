using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineCamera[] _allVirtualCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedDampingChangeThreshold = -15f;

    [Header("Hurt Zoom Pulse")]
    [SerializeField] private CinemachineBrain _brain;

    [Tooltip("Positive zooms out. Negative zooms in.")]
    [SerializeField, Range(-0.15f, 0.15f)]
    private float _hurtZoomAmount = 0.04f;

    [SerializeField, Min(0.01f)]
    private float _hurtZoomOutTime = 0.05f;

    [SerializeField, Min(0.01f)]
    private float _hurtZoomReturnTime = 0.15f;

    private CinemachineCamera _pulseCamera;
    private float _pulseOriginalSize;
    private float _pulseStartSize;
    private float _pulseElapsed;
    private bool _zoomPulseActive;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _positionComposer;
    private float _normYPanAmount;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        foreach (CinemachineCamera cam in _allVirtualCameras)
        {
            if (cam == null || !cam.enabled)
                continue;

            _currentCamera = cam;
            _positionComposer =
                cam.GetComponent<CinemachinePositionComposer>();

            break;
        }

        if (_positionComposer != null)
            _normYPanAmount = _positionComposer.Damping.y;
    }

    private void Update()
    {
        UpdateHurtZoom();
    }

    public void SetFollowTarget(Transform target)
    {
        foreach (CinemachineCamera cam in _allVirtualCameras)
        {
            if (cam == null) continue;

            cam.Follow = target;
            cam.PreviousStateIsValid = false;
        }
    }

    /// <summary>
    /// Lets lightweight room scenes configure the project's normal Cinemachine
    /// camera manager without introducing a second follow-camera system.
    /// </summary>
    public void ConfigureRoomCamera(
        CinemachineCamera followCamera,
        CinemachineBrain brain)
    {
        _allVirtualCameras = new[] { followCamera };
        _brain = brain;
        _currentCamera = followCamera;
        _positionComposer = followCamera != null
            ? followCamera.GetComponent<CinemachinePositionComposer>()
            : null;

        if (_positionComposer != null)
            _normYPanAmount = _positionComposer.Damping.y;
    }

    #region Lerp Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        if (_positionComposer == null) return;

        if (_lerpYPanCoroutine != null)
            StopCoroutine(_lerpYPanCoroutine);

        _lerpYPanCoroutine =
            StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startAmount = _positionComposer.Damping.y;
        float endAmount = isPlayerFalling
            ? _fallPanAmount
            : _normYPanAmount;

        if (isPlayerFalling)
            LerpedFromPlayerFalling = true;

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, _fallYPanTime);

        while (elapsed < duration && _positionComposer != null)
        {
            elapsed += Time.deltaTime;

            ;

            Vector3 damping = _positionComposer.Damping;
            damping.y = Mathf.Lerp(
                startAmount,
                endAmount,
                elapsed / duration);

            _positionComposer.Damping = damping;

            yield return null;
        }

        IsLerpingYDamping = false;
        _lerpYPanCoroutine = null;
    }

    #endregion

    #region Lever Cinematic

    // Kept with its existing signature for compatibility.
    public IEnumerator PlayLeverCinematic(
        CinemachineCamera cutsceneCam,
        FakeWall wall,
        CinemachineImpulseSource impulseSource,
        PlayerMovement player)
    {
        player.SetFrozen(true);

        int originalPriority = cutsceneCam.Priority;
        cutsceneCam.Priority = 999;

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(wall.Open());

        yield return new WaitForSeconds(1f);

        cutsceneCam.Priority = originalPriority;

        yield return new WaitForSeconds(2f);

        player.SetFrozen(false);
    }

    #endregion

    #region Player Hurt Zoom

    public void PlayHurtZoom()
    {
        if (!isActiveAndEnabled) return;

        if (_brain == null)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
                _brain = mainCamera.GetComponent<CinemachineBrain>();
        }

        if (_brain == null || _brain.IsBlending) return;

        CinemachineCamera activeCamera =
            _brain.ActiveVirtualCamera as CinemachineCamera;

        if (activeCamera == null) return;

        if (!_zoomPulseActive || _pulseCamera != activeCamera)
        {
            CancelZoomPulse();

            _pulseCamera = activeCamera;
            _pulseOriginalSize =
                activeCamera.Lens.OrthographicSize;
        }

        _pulseStartSize = activeCamera.Lens.OrthographicSize;
        _pulseElapsed = 0f;
        _zoomPulseActive = true;
    }

    private void UpdateHurtZoom()
    {
        if (!_zoomPulseActive) return;

        if (_pulseCamera == null ||
            _brain == null ||
            _brain.IsBlending ||
            (_brain.ActiveVirtualCamera as CinemachineCamera)
                != _pulseCamera)
        {
            CancelZoomPulse();
            return;
        }

        _pulseElapsed += Time.deltaTime;

        float outTime = Mathf.Max(0.01f, _hurtZoomOutTime);
        float returnTime = Mathf.Max(0.01f, _hurtZoomReturnTime);

        float peakSize =
            _pulseOriginalSize * (1f + _hurtZoomAmount);

        float size;

        if (_pulseElapsed < outTime)
        {
            float t = _pulseElapsed / outTime;
            float easedT = 1f - (1f - t) * (1f - t);

            size = Mathf.Lerp(
                _pulseStartSize, peakSize, easedT);
        }
        else if (_pulseElapsed < outTime + returnTime)
        {
            float t = (_pulseElapsed - outTime) / returnTime;
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            size = Mathf.Lerp(
                peakSize, _pulseOriginalSize, easedT);
        }
        else
        {
            CancelZoomPulse();
            return;
        }

        _pulseCamera.Lens.OrthographicSize = size;
    }

    public void CancelZoomPulse()
    {
        if (_zoomPulseActive && _pulseCamera != null)
        {
            _pulseCamera.Lens.OrthographicSize =
                _pulseOriginalSize;
        }

        _zoomPulseActive = false;
        _pulseCamera = null;
    }

    #endregion

    private void OnDisable()
    {
        CancelZoomPulse();

        if (_lerpYPanCoroutine != null)
        {
            StopCoroutine(_lerpYPanCoroutine);
            _lerpYPanCoroutine = null;
        }

        IsLerpingYDamping = false;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
