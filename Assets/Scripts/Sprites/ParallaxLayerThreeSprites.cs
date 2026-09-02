using UnityEngine;

public class ParallaxLayerThreeSprites : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] private float _parallaxFactor = 0.5f;
    [SerializeField] private bool _lockY = true;

    [Header("Repeating Sprite")]
    [SerializeField] private SpriteRenderer _spritePrefab;

    private Transform _cam;

    private Transform[] _pieces = new Transform[3];

    private float _spriteWidth;
    private float _startY;
    private float _lastCamX;

    private void Start()
    {
        _cam = Camera.main.transform;

        _spriteWidth = _spritePrefab.bounds.size.x;
        _startY = transform.position.y;

        CreatePieces();

        _lastCamX = _cam.position.x;
    }

    private void CreatePieces()
    {
        for (int i = 0; i < 3; i++)
        {
            SpriteRenderer piece = Instantiate(_spritePrefab, transform);

            float xOffset = (i - 1) * _spriteWidth;

            piece.transform.position = new Vector3(
                transform.position.x + xOffset,
                transform.position.y,
                transform.position.z
            );

            _pieces[i] = piece.transform;
        }

        // Hide the original template if it's an object in the scene.
        _spritePrefab.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_cam == null)
            return;

        float camX = _cam.position.x;
        float cameraDelta = camX - _lastCamX;

        // Move all three pieces according to parallax.
        for (int i = 0; i < _pieces.Length; i++)
        {
            Transform piece = _pieces[i];

            Vector3 position = piece.position;

            position.x += cameraDelta * _parallaxFactor;

            if (_lockY)
                position.y = _startY;

            piece.position = position;
        }

        RecyclePieces(camX);

        _lastCamX = camX;
    }

    private void RecyclePieces(float camX)
    {
        // Find leftmost and rightmost pieces.
        Transform leftmost = _pieces[0];
        Transform rightmost = _pieces[0];

        for (int i = 1; i < _pieces.Length; i++)
        {
            if (_pieces[i].position.x < leftmost.position.x)
                leftmost = _pieces[i];

            if (_pieces[i].position.x > rightmost.position.x)
                rightmost = _pieces[i];
        }

        // If left piece is far enough behind camera,
        // move it to the right side.
        if (leftmost.position.x < camX - _spriteWidth * 1.5f)
        {
            leftmost.position = new Vector3(
                rightmost.position.x + _spriteWidth,
                leftmost.position.y,
                leftmost.position.z
            );
        }

        // If right piece is far enough ahead,
        // move it to the left side.
        if (rightmost.position.x > camX + _spriteWidth * 1.5f)
        {
            rightmost.position = new Vector3(
                leftmost.position.x - _spriteWidth,
                rightmost.position.y,
                rightmost.position.z
            );
        }
    }
}