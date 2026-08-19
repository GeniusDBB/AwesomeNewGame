using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<DialogueLine> _lines;
    private bool _hasTriggered;

    [SerializeField] private string _saveId;

    //KeyQuest
    //[SerializeField] private bool _startKeyQuest;

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
        DialogueManager.Instance.StartDialogue(_lines);

        /*if (_startKeyQuest)
        {
            QuestManager.Instance.StartKeyQuest();
        }*/
    }
}