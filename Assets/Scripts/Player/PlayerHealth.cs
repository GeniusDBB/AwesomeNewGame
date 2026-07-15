using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _invincibilityDuration = 1f;

    private int _currentHealth;
    private bool _isInvincible;
    private PlayerMovement _movement;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _movement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount, Vector2 knockback)
    {
        if (_isInvincible) return;

        _currentHealth -= amount;
        _movement.ApplyKnockback(knockback);

        Debug.Log($"{_currentHealth}/{_maxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    private System.Collections.IEnumerator InvincibilityFrames()
    {
        _isInvincible = true;
        // flicker sprite, disable hazard collision layer, etc.
        yield return new WaitForSeconds(_invincibilityDuration);
        _isInvincible = false;
    }

    private void Die()
    {
        // trigger death animation, respawn, etc.
        Debug.Log("Player died");
    }
}