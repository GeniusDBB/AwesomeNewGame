using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Interact Prompt")]
    [SerializeField] private RectTransform _interactIcon;

    [Header("Quest Display")]
    [SerializeField] private TMP_Text _questText;

    [Header("Key Socket UI")]
    [SerializeField] private GameObject _keySocketPanel;
    [SerializeField] private UnityEngine.UI.Image[] _keySlotIcons;

    [Header("Save Icon")]
    [SerializeField] private GameObject _saveIcon;

    [Header("Tutorial")]
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private TMP_Text _tutorialText;
    [SerializeField] private CanvasGroup _tutorialCanvasGroup;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            HideInteractPrompt();
            HideKeySocketUI();
            HideTutorial();
        }
    }

    // Later: ShowPauseMenu(), HidePauseMenu(), UpdateQuestLog(...), etc.
    // all future UI plugs into this same hub as new methods.

    #region KeySocketUI

    public void ShowKeySocketUI(int placed, int required)
    {
        _keySocketPanel.SetActive(true);
        UpdateKeySocketUI(placed, required);
    }

    public void HideKeySocketUI()
    {
        _keySocketPanel.SetActive(false);
    }

    public void UpdateKeySocketUI(int placed, int required)
    {
        for (int i = 0; i < _keySlotIcons.Length; i++)
        {
            Color c = _keySlotIcons[i].color;
            c.a = i < placed ? 1f : 0f;
            _keySlotIcons[i].color = c;
        }
    }

    #endregion

    #region Save Icon

    public void ShowSaveIcon(float duration = 1.2f)
    {
        StartCoroutine(SaveIconRoutine(duration));
    }

    private IEnumerator SaveIconRoutine(float duration)
    {
        _saveIcon.SetActive(true);
        yield return new WaitForSeconds(duration);
        _saveIcon.SetActive(false);
    }

    #endregion

    #region Tutorial

    public void ShowTutorial(string text)
    {
        _tutorialPanel.SetActive(true);
        _tutorialText.text = text;
        StartCoroutine(FadeCanvasGroup(_tutorialCanvasGroup, 1f));
    }

    public void HideTutorial()
    {
        StartCoroutine(FadeCanvasGroup(_tutorialCanvasGroup, 0f));
    }

    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup group, float target)
    {
        float start = group.alpha;
        float t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, t / 0.3f);
            yield return null;
        }
        group.alpha = target;
    }

    #endregion
}