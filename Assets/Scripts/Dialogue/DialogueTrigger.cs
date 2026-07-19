using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<DialogueLine> _lines;
    private bool _hasTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        _hasTriggered = true;
        DialogueManager.Instance.StartDialogue(_lines);
    }
}