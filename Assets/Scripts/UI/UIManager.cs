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
    [SerializeField] private Vector3 _interactPromptOffset = new Vector3(0f, 1.5f, 0f);

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

    [Header("Input distinction")]
    [SerializeField] private UnityEngine.UI.Image _interactImage;
    [SerializeField] private Sprite _keyboardInteractSprite;
    [SerializeField] private Sprite _gamepadInteractSprite;
    private string _tutorialKeyboardText;
    private string _tutorialGamepadText;
    [SerializeField] private TMP_SpriteAsset _keyboardTutorialSprites;
    [SerializeField] private TMP_SpriteAsset _gamepadTutorialSprites;

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

        ResetTutorial();
    }

    private void Start()
    {
        KeyManager.Instance.OnKeyCountChanged += UpdateKeyQuestText;
    }

    private void Update()
    {
        if (_interactIcon.gameObject.activeInHierarchy)
        {
            RefreshInteractSprite();
        }

        if (_tutorialPanel.activeInHierarchy)
        {
            RefreshTutorialText();
        }
    }

    private void RefreshInteractSprite()
    {
        if (_interactImage == null) return;

        Sprite desiredSprite = InputManager.IsUsingGamepad
            ? _gamepadInteractSprite
            : _keyboardInteractSprite;

        if (_interactImage.sprite != desiredSprite)
        {
            _interactImage.sprite = desiredSprite;
        }
    }

    public void ShowInteractPrompt(Vector3 worldPosition)
    {
        RefreshInteractSprite();

        _interactIcon.gameObject.SetActive(true);
        UpdateInteractIconPosition(worldPosition);
    }

    public void HideInteractPrompt()
    {
        _interactIcon.gameObject.SetActive(false);
    }

    public void UpdateInteractIconPosition(Vector3 worldPosition)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(worldPosition + _interactPromptOffset);

        screenPos.z = 0f;
        _interactIcon.position = screenPos;
    }

    public void UpdateKeyQuestText(int current, int required)
    {
        _questText.text = $"Collect all keys: {current}/{required}";
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetTutorial();

        if (scene.name == "MainMenu")
        {
            HideInteractPrompt();
            HideKeySocketUI();
        }
    }


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
            //c.a = i < placed ? 1f : 0f;
            c.a = i < placed ? 200f / 255f : 0f;
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

    private Vector3 _tutorialWorldPosition;
    private Coroutine _tutorialFadeRoutine;

    private void LateUpdate()
    {
        if (_tutorialPanel.activeSelf)
        {
            UpdateTutorialPosition();
        }
    }

    private void UpdateTutorialPosition()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 screenPosition =
            cam.WorldToScreenPoint(_tutorialWorldPosition);

        // Overlay UI uses screen X/Y; camera depth isn't needed.
        screenPosition.z = 0f;

        _tutorialPanel.transform.position = screenPosition;
    }

    public void ShowTutorial(string keyboardText, string gamepadText, Vector3 worldPosition)
    {
        _tutorialKeyboardText = keyboardText;
        _tutorialGamepadText = gamepadText;
        _tutorialWorldPosition = worldPosition;

        RefreshTutorialText();

        _tutorialPanel.SetActive(true);
        UpdateTutorialPosition();

        StartTutorialFade(1f);
    }

    private void RefreshTutorialText()
    {
        TMP_SpriteAsset desiredAsset = InputManager.IsUsingGamepad
            ? _gamepadTutorialSprites
            : _keyboardTutorialSprites;

        if (_tutorialText.spriteAsset != desiredAsset)
        {
            _tutorialText.spriteAsset = desiredAsset;
        }

        string desiredText = InputManager.CurrentPrompt(
            _tutorialKeyboardText,
            _tutorialGamepadText
        );

        if (_tutorialText.text != desiredText)
        {
            _tutorialText.text = desiredText;
        }
    }
    public void HideTutorial()
    {
        StartTutorialFade(0f);
    }

    private void StartTutorialFade(float targetAlpha)
    {
        if (_tutorialFadeRoutine != null)
        {
            StopCoroutine(_tutorialFadeRoutine);
        }

        _tutorialFadeRoutine =
            StartCoroutine(FadeTutorial(targetAlpha));
    }

    private IEnumerator FadeTutorial(float targetAlpha)
    {
        float startAlpha = _tutorialCanvasGroup.alpha;
        float elapsed = 0f;
        const float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            _tutorialCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / duration
            );

            yield return null;
        }

        _tutorialCanvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
        {
            _tutorialPanel.SetActive(false);
        }

        _tutorialFadeRoutine = null;
    }

    private void ResetTutorial()
    {
        if (_tutorialFadeRoutine != null)
        {
            StopCoroutine(_tutorialFadeRoutine);
            _tutorialFadeRoutine = null;
        }

        _tutorialCanvasGroup.alpha = 0f;
        _tutorialPanel.SetActive(false);
    }

    #endregion
}