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

    //Player Shake
    //=============================================================
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
    //=============================================================

    //Door Zoom Pulse
    //=============================================================
    [Header("Door Zoom Pulses")]
    [SerializeField, Range(-0.15f, 0.15f)]
    private float _doorStartZoomAmount = 0.025f;

    [SerializeField, Range(-0.15f, 0.15f)]
    private float _doorStopZoomAmount = 0.04f;

    // Settings captured for the currently playing pulse.
    private float _pulseAmount;
    private float _pulseOutTime;
    private float _pulseReturnTime;
    //=============================================================

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;

    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _positionComposer;

    private float _normYPanAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                _currentCamera = _allVirtualCameras[i];
                _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
                break;
            }
        }

        _normYPanAmount = _positionComposer.Damping.y;
    }

    public void SetFollowTarget(Transform target)
    {
        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            _allVirtualCameras[i].Follow = target;
            _allVirtualCameras[i].PreviousStateIsValid = false;
            // _allVirtualCameras[i].LookAt = target; // if you use LookAt too
        }
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        if (_lerpYPanCoroutine != null)
        {
            StopCoroutine(_lerpYPanCoroutine);
        }

        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        //Grab the starting damping amount
        float StartDampAmount = _positionComposer.Damping.y;
        float endDampAmount = 0f;

        //determine the end damping amount
        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }

        else
        {
            endDampAmount = _normYPanAmount;
        }

        //lerp the pan amount
        float elapsedTime = 0f;
        while(elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(StartDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));

            Vector3 damping = _positionComposer.Damping;
            damping.y = lerpedPanAmount;
            _positionComposer.Damping = damping;


            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion

    #region LeverCinematic

    public IEnumerator PlayLeverCinematic(CinemachineCamera cutsceneCam, FakeWall wall, CinemachineImpulseSource impulseSource, PlayerMovement player)
    {
        player.SetFrozen(true);

        int originalPriority = cutsceneCam.Priority;
        cutsceneCam.Priority = 999; // higher than any gameplay camera, forces the blend

        yield return new WaitForSeconds(2f); // let the blend-to-wall finish and hold a beat

        yield return StartCoroutine(wall.Open());

        yield return new WaitForSeconds(1f); // hold on the open wall briefly

        cutsceneCam.Priority = originalPriority; // blend back to whichever gameplay cam has highest priority

        yield return new WaitForSeconds(2f); // let blend-back finish before returning control

        player.SetFrozen(false);
    }

    #endregion

    #region Zoom Pulse

    public void PlayHurtZoom()
    {
        if (!isActiveAndEnabled) return;

        if (_brain == null)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                _brain = mainCamera.GetComponent<CinemachineBrain>();
            }
        }

        // Avoid adding a pulse during a camera transition.
        if (_brain == null || _brain.IsBlending) return;

        CinemachineCamera activeCamera =
            _brain.ActiveVirtualCamera as CinemachineCamera;

        if (activeCamera == null) return;

        // Preserve the original size if another hit restarts the pulse.
        if (!_zoomPulseActive || _pulseCamera != activeCamera)
        {
            CancelZoomPulse();

            _pulseCamera = activeCamera;
            _pulseOriginalSize = activeCamera.Lens.OrthographicSize;
        }

        // Restart smoothly from the current size.
        _pulseStartSize = activeCamera.Lens.OrthographicSize;
        _pulseElapsed = 0f;
        _zoomPulseActive = true;
    }

    private void Update()
    {
        if (!_zoomPulseActive) return;

        // Restore the old camera if a transition starts.
        if (_pulseCamera == null ||
            _brain == null ||
            _brain.IsBlending ||
             (_brain.ActiveVirtualCamera as CinemachineCamera) != _pulseCamera)
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

            // Fast initial response, slowing toward the peak.
            float easedT = 1f - (1f - t) * (1f - t);

            size = Mathf.Lerp(_pulseStartSize, peakSize, easedT);
        }
        else if (_pulseElapsed < outTime + returnTime)
        {
            float t = (_pulseElapsed - outTime) / returnTime;
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            size = Mathf.Lerp(peakSize, _pulseOriginalSize, easedT);
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
            _pulseCamera.Lens.OrthographicSize = _pulseOriginalSize;
        }

        _zoomPulseActive = false;
        _pulseCamera = null;
    }

    private void OnDisable()
    {
        CancelZoomPulse();
    }

    #endregion

    #region Narrow Passage



    #endregion
}
