using UnityEngine;

public class CinematicTrigger : MonoBehaviour
{
    [SerializeField] private CinematicPlayer _cinematic;
    private bool _hasTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        _hasTriggered = true;
        _cinematic.Play();
    }
}