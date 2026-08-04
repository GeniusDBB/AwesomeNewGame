using Unity.Cinemachine;
using UnityEngine;

public class PlayerLadderDetector : MonoBehaviour
{
    private PlayerMovement _movement;

    private void Awake()
    {
        _movement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Ladder>(out var ladder))
        {
            _movement.SetNearbyLadder(ladder);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Ladder>(out var ladder))
        {
            _movement.ClearNearbyLadder(ladder);
        }
    }
}
