using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FallingChunkInstantKill : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<PlayerHealth>(out var health))
            health.InstantKill();
    }
}
