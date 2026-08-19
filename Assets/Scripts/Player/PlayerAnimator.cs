using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;

    private PlayerMovement _movement;

    private static readonly int Hash_IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Hash_IsGround = Animator.StringToHash("IsGround");
    private static readonly int Hash_VerticalVelocity = Animator.StringToHash("VerticalVelocity");
    private static readonly int Hash_IsWallSliding = Animator.StringToHash("IsWallSliding");
    private static readonly int Hash_IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int Hash_IsAirDashing = Animator.StringToHash("IsAirDashing");


    private static readonly int Hash_Jump = Animator.StringToHash("Jump");
    private static readonly int Hash_Dash = Animator.StringToHash("Dash");

    private static readonly int Hash_Land = Animator.StringToHash("Land");

    private static readonly int Hash_Hurt = Animator.StringToHash("Hurt");
    private static readonly int Hash_Death = Animator.StringToHash("Death");

    private static readonly int Hash_IsOnLadder = Animator.StringToHash("IsOnLadder");

    private static readonly int Hash_SitDown = Animator.StringToHash("SitDown");
    private static readonly int Hash_GetUp = Animator.StringToHash("GetUp");
    private static readonly int Hash_ResetToIdle = Animator.StringToHash("ResetToIdle");

    private bool _wasGrounded;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();

        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
            Debug.LogError("PlayerAnimator: No Animator found!", this);

        _wasGrounded = _movement.IsGround;
    }

    private void LateUpdate()
    {
        if (_animator == null)
            return;

        UpdateParameters();
        DetectLanding();
    }

    private void UpdateParameters()
    {
        bool moving = _movement.IsOnLadder ? (Mathf.Abs(_movement.VerticalVelocity) > 0.05f || Mathf.Abs(_movement.HorizontalVelocity) > 0.1f) : Mathf.Abs(_movement.HorizontalVelocity) > 0.1f;
        _animator.SetBool(Hash_IsMoving, moving);
        _animator.SetBool(Hash_IsGround, _movement.IsGround);
        _animator.SetBool(Hash_IsWallSliding, _movement.IsWallSliding);
        _animator.SetBool(Hash_IsDashing, _movement.IsDashing);
        _animator.SetBool(Hash_IsAirDashing, _movement.IsAirDashing);
        _animator.SetBool(Hash_IsOnLadder, _movement.IsOnLadder);
        _animator.SetFloat(Hash_VerticalVelocity, _movement.VerticalVelocity);
    }

    private void DetectLanding()
    {
        bool grounded = _movement.IsGround;
        if (!_wasGrounded && grounded)
        {
            _animator.SetTrigger(Hash_Land);
        }
        _wasGrounded = grounded;
    }

    // ===== PUBLIC ANIMATION EVENTS =====

    public void OnJumpStarted()
    {
        _animator.SetTrigger(Hash_Jump);
    }

    public void OnDashStarted()
    {
        _animator.SetTrigger(Hash_Dash);
    }

    public void OnHurt()
    {
        _animator.SetTrigger(Hash_Hurt);
    }

    public void OnDeath()
    {
        _animator.SetTrigger(Hash_Death);
    }

    public void OnSitDown() => _animator.SetTrigger(Hash_SitDown);
    public void OnGetUp() => _animator.SetTrigger(Hash_GetUp);
    public void OnRevive() => _animator.SetTrigger(Hash_ResetToIdle);
}