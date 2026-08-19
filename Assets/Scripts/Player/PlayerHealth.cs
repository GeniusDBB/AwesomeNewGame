using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _invincibilityDuration = 1f;
    [SerializeField] private bool _isInvincible = false;

    private int _currentHealth;
    private PlayerMovement _movement;
    private Rigidbody2D _rb;
    private PlayerAnimator _animator;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action OnPlayerDied;

    private bool _isDead;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _movement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<PlayerAnimator>();
    }

    private void Start()
    {
        // fire once at start so UI can initialize correctly
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void TakeDamage(int amount, Vector2 knockback)
    {
        if (_isInvincible || _isDead) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        _movement.ApplyKnockback(knockback);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            _animator?.OnHurt();
            StartCoroutine(InvincibilityFrames());
        }
    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    private System.Collections.IEnumerator InvincibilityFrames()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibilityDuration);
        _isInvincible = false;
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _animator?.OnDeath();
        _movement.SetFrozen(true);
        OnPlayerDied?.Invoke();
    }

    public void Revive()
    {
        _animator?.OnRevive();
        _currentHealth = _maxHealth;
        _isDead = false;
        _movement.SetFrozen(false);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }
}