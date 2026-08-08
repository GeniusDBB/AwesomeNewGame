using UnityEngine;
using System.Collections.Generic;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Dialogue")]
    [SerializeField] private List<DialogueLine> _notEnoughKeysDialogue;

    [Header("Scene Transition")]
    [SerializeField] private string _targetScene;
    [SerializeField] private string _targetSpawnId;

    private int _placedKeys;
    private bool _isOpen;

    public int PlacedKeys => _placedKeys;

    public void Interact()
    {
        if (_isOpen) return;

        if (_placedKeys < KeyManager.Instance.CollectedKeys && _placedKeys < KeyManager.Instance.RequiredKeys)
        {
            _placedKeys++;
            UIManager.Instance.UpdateKeySocketUI(_placedKeys, KeyManager.Instance.RequiredKeys);

            if (_placedKeys >= KeyManager.Instance.RequiredKeys)
            {
                OpenDoor();
            }
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