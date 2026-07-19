using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _invincibilityDuration = 1f;

    private int _currentHealth;
    private bool _isInvincible;
    private PlayerMovement _movement;
    private Rigidbody2D _rb;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action OnPlayerDied;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _movement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // fire once at start so UI can initialize correctly
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void TakeDamage(int amount, Vector2 knockback)
    {
        if (_isInvincible) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        _movement.ApplyKnockback(knockback);

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
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
        OnPlayerDied?.Invoke();
    }
}