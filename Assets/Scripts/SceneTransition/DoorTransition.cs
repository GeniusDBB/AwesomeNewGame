using UnityEngine;
using System.Collections.Generic;

public class DoorTransition : MonoBehaviour, IInteractable
{
    [Header("Dialogue")]
    [SerializeField] private List<DialogueLine> _notEnoughKeysDialogue;

    [Header("Scene Transition")]
    [SerializeField] private string _targetScene;
    [SerializeField] private string _targetSpawnId;

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
        SceneTransitionManager.Instance.LoadScene(_targetScene, _targetSpawnId);
    }
}