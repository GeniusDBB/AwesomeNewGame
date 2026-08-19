using UnityEngine;

public class CinematicTrigger : MonoBehaviour
{
    [SerializeField] private CinematicPlayer _cinematic;
    [SerializeField] private string _saveId;

    private bool _hasTriggered;

    private void Start()
    {
        if (SaveManager.Instance.HasFlag(_saveId))
        {
            _hasTriggered = true;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        _hasTriggered = true;
        SaveManager.Instance.SetFlag(_saveId);
        _cinematic.Play();
    }
}