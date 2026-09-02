using UnityEngine;
using System.Collections.Generic;

public class ParallaxLayerRepeating : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] private float _parallaxFactor = 0.5f; // 0 = static/far, 1 = moves with camera
    [SerializeField] private bool _lockY = true;

    [Header("Repeating Sprite")]
    [SerializeField] private SpriteRenderer _spritePrefab;
    [SerializeField] private int _copies = 3;

    private Transform _cam;
    private readonly List<Transform> _pieces = new();
    private float _spriteWidth;
    private float _startY;
    private float _wrapRange;

    private void Start()
    {
        _cam = Camera.main.transform;
        _spriteWidth = _spritePrefab.bounds.size.x;
        _startY = transform.position.y;
        _wrapRange = _spriteWidth * _copies;

        for (int i = 0; i < _copies; i++)
        {
            float xOffset = (i - _copies / 2) * _spriteWidth;
            var piece = Instantiate(_spritePrefab, transform);
            piece.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z);
            _pieces.Add(piece.transform);
        }

        _spritePrefab.gameObject.SetActive(false); // hide the original template if it's in-scene
    }

    private void LateUpdate()
    {
        float camX = _cam.position.x;

        foreach (var piece in _pieces)
        {
            float targetX = piece.position.x + (camX - _lastCamX) * _parallaxFactor;

            while (targetX - camX > _wrapRange / 2f) targetX -= _wrapRange;
            while (targetX - camX < -_wrapRange / 2f) targetX += _wrapRange;

            float y = _lockY ? _startY : piece.position.y;
            piece.position = new Vector3(targetX, y, piece.position.z);
        }

        _lastCamX = camX;
    }

    private float _lastCamX;

    private void OnEnable()
    {
        if (_cam == null) _cam = Camera.main.transform;
        _lastCamX = _cam.position.x;
    }
}