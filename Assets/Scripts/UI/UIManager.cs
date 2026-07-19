using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Interact Prompt")]
    [SerializeField] private GameObject _interactPromptObject;
    [SerializeField] private TMP_Text _interactPromptText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        HideInteractPrompt();
    }

    public void ShowInteractPrompt(string text = "Press E")
    {
        _interactPromptObject.SetActive(true);
        _interactPromptText.text = text;
    }

    public void HideInteractPrompt()
    {
        _interactPromptObject.SetActive(false);
    }

    // Later: ShowPauseMenu(), HidePauseMenu(), UpdateQuestLog(...), etc.
    // all future UI plugs into this same hub as new methods.
}