using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraArea : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _areaCamera;

    [Tooltip("Must be above normal gameplay and below cutscene priority.")]
    [SerializeField] private int _activePriority = 20;

    [SerializeField] private int _inactivePriority = 0;

    private BoxCollider2D _area;
    private Transform _player;
    private bool _isInside;

    private void Awake()
    {
        _area = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        _isInside = false;

        if (_areaCamera != null)
            _areaCamera.Priority = _inactivePriority;
    }

    private void Update()
    {
        if (_areaCamera == null) return;

        if (_player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                _player = playerObject.transform;
        }

        if (_player != null && _areaCamera.Follow != _player)
        {
            _areaCamera.Follow = _player;
            _areaCamera.PreviousStateIsValid = false;
        }

        bool inside =
            _player != null &&
            _area.enabled &&
            _area.OverlapPoint((Vector2)_player.position);

        if (inside == _isInside) return;

        // Restore any temporary zoom before changing cameras.
        if (CameraManager.instance != null)
            CameraManager.instance.CancelZoomPulse();

        _isInside = inside;

        _areaCamera.Priority = inside
            ? _activePriority
            : _inactivePriority;
    }

    private void OnDisable()
    {
        _isInside = false;

        if (_areaCamera != null)
            _areaCamera.Priority = _inactivePriority;
    }
}