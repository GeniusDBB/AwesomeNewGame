using UnityEngine;

public class DamagePlatform : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private bool _instantKill = false;

    [Header("Knockback Settings")]
    [SerializeField] private bool _applyKnockback = true;
    [SerializeField] private float _knockbackForce = 15f;
    [SerializeField] private float _knockbackUpwardBias = 0.5f; // pushes them up a bit so it feels good

    [Header("Timing")]
    [SerializeField] private float _damageCooldown = 1f; // prevents multi-hit per frame/stay

    private float _lastDamageTime = -999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // handles hazards you can stand in, like lava, without instakill
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerHealth>(out var health)) return;
        if (Time.time - _lastDamageTime < _damageCooldown) return;

        _lastDamageTime = Time.time;

        int dmg = _instantKill ? int.MaxValue : _damageAmount;

        Vector2 knockDir = Vector2.zero;
        if (_applyKnockback)
        {
            knockDir = (other.transform.position - transform.position).normalized;
            Debug.Log($"Player: {other.transform.position} Hazard: {transform.position} KnockDir: {knockDir}");
            knockDir.y = Mathf.Max(knockDir.y, _knockbackUpwardBias);
        }

        health.TakeDamage(dmg, knockDir * _knockbackForce);
    }
}