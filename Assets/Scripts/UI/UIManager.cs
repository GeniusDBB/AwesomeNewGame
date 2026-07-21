using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Interact Prompt")]
    [SerializeField] private RectTransform _interactIcon;

    [Header("Quest Display")]
    [SerializeField] private TMP_Text _questText;

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

    private void Start()
    {
        KeyManager.Instance.OnKeyCountChanged += UpdateKeyQuestText;
    }

    public void ShowInteractPrompt(Vector3 worldPosition)
    {
        _interactIcon.gameObject.SetActive(true);
        UpdateInteractIconPosition(worldPosition);
    }

    public void HideInteractPrompt()
    {
        _interactIcon.gameObject.SetActive(false);
    }

    public void UpdateInteractIconPosition(Vector3 worldPosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        _interactIcon.position = screenPos;
    }

    public void UpdateKeyQuestText(int current, int required)
    {
        _questText.text = $"Collect all keys: {current}/{required}";
    }

    // Later: ShowPauseMenu(), HidePauseMenu(), UpdateQuestLog(...), etc.
    // all future UI plugs into this same hub as new methods.
}