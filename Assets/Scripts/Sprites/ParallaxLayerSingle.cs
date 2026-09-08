using UnityEngine;

public class ParallaxLayerSingle : MonoBehaviour
{
    [SerializeField] private float _parallaxFactor = 0.3f; // match whatever factor the rest of that layer uses
    [SerializeField] private bool _lockY = true;

    private Transform _cam;
    private Vector3 _startPosition;
    private float _lastCamX;

    private void Start()
    {
        _cam = Camera.main.transform;
        _startPosition = transform.position;
        _lastCamX = _cam.position.x;
    }

    private void LateUpdate()
    {
        float camX = _cam.position.x;
        float cameraDelta = camX - _lastCamX;

        Vector3 position = transform.position;
        position.x += cameraDelta * _parallaxFactor;
        if (_lockY) position.y = _startPosition.y;
        transform.position = position;

        _lastCamX = camX;
    }
}