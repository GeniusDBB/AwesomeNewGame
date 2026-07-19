using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _bodyText;
    [SerializeField] private GameObject _continueIndicator;

    [Header("Player Reference")]
    [SerializeField] private PlayerMovement _playerMovement;

    [Header("Typing Settings")]
    [SerializeField] private float _charactersPerSecond = 40f;

    private readonly Queue<DialogueLine> _lineQueue = new Queue<DialogueLine>();
    private string _currentFullText;
    private Coroutine _typingCoroutine;
    private bool _isTyping;
    private bool _dialogueActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!_dialogueActive) return;

        if (InputManager.JumpWasPressed)
        {
            if (_isTyping)
            {
                CompleteLineInstantly();
            }
            else
            {
                AdvanceDialogue();
            }
        }
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        if (_dialogueActive) return;

        _lineQueue.Clear();
        foreach (var line in lines)
        {
            _lineQueue.Enqueue(line);
        }

        _dialogueActive = true;
        _dialoguePanel.SetActive(true);
        _playerMovement.SetFrozen(true);

        AdvanceDialogue();
    }

    private void AdvanceDialogue()
    {
        if (_lineQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine next = _lineQueue.Dequeue();
        _nameText.text = next.SpeakerName;

        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = StartCoroutine(TypeLine(next.Text));
    }

    private IEnumerator TypeLine(string text)
    {
        _isTyping = true;
        _currentFullText = text;
        _continueIndicator.SetActive(false);
        _bodyText.text = "";

        float delay = 1f / _charactersPerSecond;

        foreach (char c in text)
        {
            _bodyText.text += c;
            yield return new WaitForSeconds(delay);
        }

        _isTyping = false;
        _continueIndicator.SetActive(true);
    }

    private void CompleteLineInstantly()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);

        _bodyText.text = _currentFullText;
        _isTyping = false;
        _continueIndicator.SetActive(true);
    }

    private void EndDialogue()
    {
        _dialogueActive = false;
        _dialoguePanel.SetActive(false);
        _playerMovement.SetFrozen(false);
    }
}