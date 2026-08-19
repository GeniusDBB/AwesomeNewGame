using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    [SerializeField] private string _saveId;

    private bool _used;

    private void Start()
    {
        if (SaveManager.Instance.HasFlag(_saveId))
        {
            _used = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_used) return;
        if (!other.CompareTag("Player")) return;

        _used = true;
        SaveManager.Instance.SetCheckpoint(_sceneName, transform.position);
        SaveManager.Instance.SetFlag(_saveId);
        UIManager.Instance.ShowSaveIcon();
    }
}