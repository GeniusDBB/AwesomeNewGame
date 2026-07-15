using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private float _speed = 3f;

    private Rigidbody2D _rb;
    private Vector2 _targetPoint;

    public Vector2 DeltaMovement { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _targetPoint = _pointB.position;
    }

    private void FixedUpdate()
    {
        Vector2 previousPosition = _rb.position;

        Vector2 newPosition = Vector2.MoveTowards(_rb.position, _targetPoint, _speed * Time.fixedDeltaTime);
        _rb.MovePosition(newPosition);

        DeltaMovement = newPosition - previousPosition;

        if (Vector2.Distance(_rb.position, _targetPoint) < 0.01f)
        {
            _targetPoint = ((Vector2)_targetPoint == (Vector2)_pointB.position) ? (Vector2)_pointA.position : (Vector2)_pointB.position;
        }
    }
}