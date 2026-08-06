using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private bool _canBeInstaKilled = false;

    private int _currentHealth;
    private Rigidbody2D _rb;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount, Vector2 knockback)
    {
        if (_currentHealth <= 0) return; // already dead, ignore further hits

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_rb != null)
        {
            _rb.linearVelocity = knockback;
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void InstaKill()
    {
        if (!_canBeInstaKilled || _currentHealth <= 0) return;
        _currentHealth = 0;
        OnHealthChanged?.Invoke(0, _maxHealth);
        Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}