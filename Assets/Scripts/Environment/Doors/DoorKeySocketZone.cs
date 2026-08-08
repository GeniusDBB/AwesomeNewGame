using UnityEngine;

public class DoorKeySocketZone : MonoBehaviour
{
    private DoorInteractable _door;

    private void Awake()
    {
        _door = GetComponentInParent<DoorInteractable>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        UIManager.Instance.ShowKeySocketUI(_door.PlacedKeys, KeyManager.Instance.RequiredKeys);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        UIManager.Instance.HideKeySocketUI();
    }
}