using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        KeyManager.Instance.AddKey();
        Destroy(gameObject);
    }
}