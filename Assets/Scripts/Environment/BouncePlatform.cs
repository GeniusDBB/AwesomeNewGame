using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private float _bounceForce = 20f;
    [SerializeField] private bool _refundAirJumps = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerMovement>(out var movement)) return;
        if (movement.VerticalVelocity > -10f) return; // only bounce when falling onto it

        movement.ApplyBounce(_bounceForce, _refundAirJumps);
    }
}