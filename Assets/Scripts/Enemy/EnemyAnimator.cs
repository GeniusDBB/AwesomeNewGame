using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private EnemyController _enemy;

    private static readonly int Hash_IsMoving = Animator.StringToHash("IsMoving");

    private void Awake()
    {
        _enemy = GetComponent<EnemyController>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        _animator.SetBool(Hash_IsMoving, Mathf.Abs(_enemy.Rb.linearVelocity.x) > 0.05f);
    }
}