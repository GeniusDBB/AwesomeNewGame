using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPauseAllowed { get; private set; } = true;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetPausePermission()
    {
        IsPauseAllowed = true;
    }

    private enum MenuState
    {
        Closed,
        Pause,
        Options,
        QuitConfirmation
    }

    [Header("Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _quitConfirmPanel;

    [Header("Quest")]
    [SerializeField] private TMP_Text _questText;

    [Header("First selected control in each panel")]
    [SerializeField] private GameObject _pauseFirstSelected;
    [SerializeField] private GameObject _optionsFirstSelected;
    [SerializeField] private GameObject _quitFirstSelected;

    private MenuState _state = MenuState.Closed;
    private float _timeScaleBeforePause = 1f;
    private bool _leavingScene;
    private int _lastMenuChangeFrame = -1;

    public bool IsPaused => _state != MenuState.Closed;

    private void Awake()
    {
        HideAllPanels();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SaveManager.Instance.OnSaveReverted += RefreshQuestText;
        QuestManager.Instance.OnQuestStateChanged += RefreshQuestText;
        KeyManager.Instance.OnKeyCountChanged += OnKeyCountChanged;
    }

    // Runs after InputManager.Update has refreshed input.
    private void LateUpdate()
    {
        if (_leavingScene ||
            SceneManager.GetActiveScene().name == "MainMenu" ||
            !IsPauseAllowed)
        {
            return;
        }

        // Avoid processing a second menu action in the same frame.
        if (_lastMenuChangeFrame == Time.frameCount)
            return;

        if (InputManager.PauseWasPressed)
        {
            if (_state == MenuState.Closed)
                OpenPause();
            else
                OnBackPressed();
        }
        else if (IsPaused && InputManager.MenuBackWasPressed)
        {
            OnBackPressed();
        }
    }

    private void OpenPause()
    {
        if (IsPaused || _leavingScene || !IsPauseAllowed) return;

        _timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
        AudioManager.Instance?.SetPaused(true);

        InputManager.SetMenuInput(true);

        if (UIManager.Instance != null)
        {
            // Tutorial fades use scaled time, so hide it immediately before pausing.
            UIManager.Instance.HideTutorialImmediately();
        }

        RefreshQuestText();
        ShowMenu(MenuState.Pause);
    }

    private void ResumeGame()
    {
        if (!IsPaused) return;

        ShowMenu(MenuState.Closed);

        InputManager.SetMenuInput(false);
        Time.timeScale = _timeScaleBeforePause;
        AudioManager.Instance?.SetPaused(false);
    }

    private void ShowMenu(MenuState state)
    {
        _state = state;
        _lastMenuChangeFrame = Time.frameCount;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        _pausePanel.SetActive(state == MenuState.Pause);
        _optionsPanel.SetActive(state == MenuState.Options);
        _quitConfirmPanel.SetActive(
            state == MenuState.QuitConfirmation);

        GameObject firstSelected = state switch
        {
            MenuState.Pause => _pauseFirstSelected,
            MenuState.Options => _optionsFirstSelected,
            MenuState.QuitConfirmation => _quitFirstSelected,
            _ => null
        };

        if (EventSystem.current != null &&
            firstSelected != null &&
            firstSelected.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public void OnBackPressed()
    {
        if (_leavingScene) return;

        switch (_state)
        {
            case MenuState.Options:
            case MenuState.QuitConfirmation:
                ShowMenu(MenuState.Pause);
                break;

            case MenuState.Pause:
                ResumeGame();
                break;
        }
    }

    public void OnOptionsPressed()
    {
        if (_state != MenuState.Pause || _leavingScene) return;

        ShowMenu(MenuState.Options);
    }

    public void OnContinuePressed()
    {
        if (_state != MenuState.Pause || _leavingScene) return;

        ResumeGame();
    }

    public void OnQuitToMenuPressed()
    {
        if (_state != MenuState.Pause || _leavingScene) return;

        ShowMenu(MenuState.QuitConfirmation);
    }

    public void OnQuitConfirmNo()
    {
        if (_state != MenuState.QuitConfirmation) return;

        OnBackPressed();
    }

    public void OnQuitConfirmYes()
    {
        if (_state != MenuState.QuitConfirmation ||
            _leavingScene)
        {
            return;
        }

        _leavingScene = true;

        SaveManager.Instance.RevertToLastSave();

        ResumeGame();
        Time.timeScale = 1f;

        SceneTransitionManager.Instance.LoadSceneSimple("MainMenu");
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }

    private void OnKeyCountChanged(int current, int required)
    {
        RefreshQuestText();
    }

    private void RefreshQuestText()
    {
        if (_questText == null ||
            QuestManager.Instance == null ||
            KeyManager.Instance == null)
        {
            return;
        }

        bool questActive = QuestManager.Instance.KeyQuestActive;
        _questText.gameObject.SetActive(questActive);

        if (questActive)
        {
            _questText.text =
                $"Collect all keys: {KeyManager.Instance.CollectedKeys}/" +
                $"{KeyManager.Instance.RequiredKeys}";
        }
    }

    private void HideAllPanels()
    {
        _pausePanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _quitConfirmPanel.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsPaused)
            ResumeGame();
        else
            HideAllPanels();

        _leavingScene = false;

        InputManager.SetMenuInput(scene.name == "MainMenu");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (IsPaused)
            Time.timeScale = _timeScaleBeforePause;

        AudioManager.Instance?.SetPaused(false);

        _state = MenuState.Closed;
        HideAllPanels();

        // Don't switch input maps during shutdown.
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.OnSaveReverted -= RefreshQuestText;

        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestStateChanged -= RefreshQuestText;

        if (KeyManager.Instance != null)
            KeyManager.Instance.OnKeyCountChanged -= OnKeyCountChanged;
    }

    /// <summary>
    /// Lets scripted sequences temporarily prevent Escape/Start from opening
    /// the pause menu.
    /// </summary>
    public static void SetPauseAllowed(bool allowed)
    {
        IsPauseAllowed = allowed;
    }
}
