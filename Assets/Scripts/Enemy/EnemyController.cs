using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Patrol")]
    public Transform PointA;
    public Transform PointB;
    public float MoveSpeed = 2f;

    public Rigidbody2D Rb { get; private set; }
    public bool IsFacingRight { get; private set; } = true;

    private EnemyStateMachine _stateMachine;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        _stateMachine = new EnemyStateMachine(this);
    }

    private void Start()
    {
        _stateMachine.ChangeState(new EnemyPatrolState());
    }

    private void Update()
    {
        _stateMachine.Tick();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedTick();
    }

    public void SetFacing(bool faceRight)
    {
        if (IsFacingRight == faceRight) return;
        IsFacingRight = faceRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void SetHorizontalVelocity(float x)
    {
        Rb.linearVelocity = new Vector2(x, Rb.linearVelocity.y);
    }
}