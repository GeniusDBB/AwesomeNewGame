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

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;

    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _positionComposer;

    private float _normYPanAmount;

    //NarrowPassage

    //Stari awake prije neg kaj sam dodo player persistent
    /*private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for(int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                //set the current active camera
                _currentCamera = _allVirtualCameras[i];

                //set the position composer
                _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
                //ovo sam ja isto dodo valjda je ok
                break;
            }

        }

        _normYPanAmount = _positionComposer.Damping.y;
    }*/
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

    #region Narrow Passage

    

    #endregion
}
