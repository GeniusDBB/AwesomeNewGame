using UnityEngine;
using System.Collections.Generic;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private List<DialogueLine> _notEnoughKeysDialogue;
    private bool _isOpen;

    public void Interact()
    {
        if (_isOpen) return;

        if (KeyManager.Instance.HasAllKeys)
        {
            OpenDoor();
        }
        else
        {
            DialogueManager.Instance.StartDialogue(_notEnoughKeysDialogue);
        }
    }

    private void OpenDoor()
    {
        _isOpen = true;
        QuestManager.Instance.CompleteKeyQuest();
        // play open animation, disable collider, etc.
    }
}