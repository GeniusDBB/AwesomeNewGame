using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private string _newGameSceneName;
    [SerializeField] private string _newGameSpawnId;

    [Header("Opening Cinematic")]
    [Tooltip("When enabled, New Game starts the opening Timeline and then places the player at the cinematic end spawn.")]
    [SerializeField] private bool _playOpeningCinematicOnNewGame = true;
    [Tooltip("Must match the SceneSpawnPoint ID at the bench where the opening ends.")]
    [SerializeField] private string _openingCinematicEndSpawnId = "bench_intro";

    [Header("Panels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField, Min(0f)] private float _mainPanelFadeDuration = 1f;

    [Header("Selection")]
    [SerializeField] private GameObject _mainFirstSelected;
    [SerializeField] private GameObject _optionsFirstSelected;
    [SerializeField] private GameObject _optionsButton;

    private bool _optionsOpen;
    private bool _leavingScene;
    private int _lastMenuChangeFrame = -1;
    private CanvasGroup _mainPanelCanvasGroup;

    private void Awake()
    {
        _optionsPanel.SetActive(false);
        _mainPanel.SetActive(true);

        _mainPanelCanvasGroup = _mainPanel.GetComponent<CanvasGroup>();
        if (_mainPanelCanvasGroup == null)
            _mainPanelCanvasGroup = _mainPanel.AddComponent<CanvasGroup>();

        SetMainPanelVisibility(0f, interactable: false);
    }

    private void Start()
    {
        _continueButton.SetActive(SaveManager.Instance.HasSaveFile());
        InputManager.SetMenuInput(true);
        ShowMenu(false, _mainFirstSelected);
        StartCoroutine(FadeMainPanelInAfterTransition());
    }

    // Match the pause menu: read input after InputManager.Update.
    private void LateUpdate()
    {
        if (_leavingScene || _lastMenuChangeFrame == Time.frameCount)
            return;

        if (_optionsOpen && InputManager.MenuBackWasPressed)
            OnBackPressed();
    }

    private void ShowMenu(bool optionsOpen, GameObject selected)
    {
        _optionsOpen = optionsOpen;
        _lastMenuChangeFrame = Time.frameCount;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        _mainPanel.SetActive(!optionsOpen);
        _optionsPanel.SetActive(optionsOpen);

        if (EventSystem.current != null &&
            selected != null && selected.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(selected);
        }
    }

    public void OnOptionsPressed()
    {
        if (_optionsOpen || _leavingScene) return;

        ShowMenu(true, _optionsFirstSelected);
    }

    public void OnBackPressed()
    {
        if (!_optionsOpen || _leavingScene) return;

        ShowMenu(false, _optionsButton);
    }

    public void OnNewGamePressed()
    {
        if (_optionsOpen || _leavingScene) return;

        _leavingScene = true;
        StartCoroutine(StartNewGameAfterFade());
    }

    private IEnumerator StartNewGameAfterFade()
    {
        yield return FadeMainPanel(0f, interactable: false);
        SaveManager.Instance.DeleteSaveAndReset();

        NewGameLaunchContext.Begin(_playOpeningCinematicOnNewGame);

        string spawnId = _playOpeningCinematicOnNewGame
            ? _openingCinematicEndSpawnId
            : _newGameSpawnId;

        SceneTransitionManager.Instance.LoadScene(_newGameSceneName, spawnId);
    }

    public void OnContinuePressed()
    {
        if (_optionsOpen || _leavingScene ||
            !SaveManager.Instance.HasSaveFile()) return;

        _leavingScene = true;
        StartCoroutine(ContinueAfterFade());
    }

    private IEnumerator ContinueAfterFade()
    {
        yield return FadeMainPanel(0f, interactable: false);
        var data = SaveManager.Instance.Data;
        SceneTransitionManager.Instance.LoadSceneAtPosition(data.CurrentScene, new Vector2(data.CheckpointX, data.CheckpointY));
    }

    public void OnQuitPressed()
    {
        if (_optionsOpen || _leavingScene) return;

        _leavingScene = true;
        StartCoroutine(QuitAfterFade());
    }

    private IEnumerator QuitAfterFade()
    {
        yield return FadeMainPanel(0f, interactable: false);
        Application.Quit();
    }

    private IEnumerator FadeMainPanelInAfterTransition()
    {
        while (SceneTransitionManager.Instance != null &&
               SceneTransitionManager.Instance.IsTransitioning)
        {
            yield return null;
        }

        yield return FadeMainPanel(1f, interactable: true);
    }

    private IEnumerator FadeMainPanel(float targetAlpha, bool interactable)
    {
        float startAlpha = _mainPanelCanvasGroup.alpha;
        _mainPanelCanvasGroup.interactable = false;
        _mainPanelCanvasGroup.blocksRaycasts = false;

        if (_mainPanelFadeDuration <= 0f)
        {
            SetMainPanelVisibility(targetAlpha, interactable);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < _mainPanelFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _mainPanelCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / _mainPanelFadeDuration);
            yield return null;
        }

        SetMainPanelVisibility(targetAlpha, interactable);
    }

    private void SetMainPanelVisibility(float alpha, bool interactable)
    {
        _mainPanelCanvasGroup.alpha = alpha;
        _mainPanelCanvasGroup.interactable = interactable;
        _mainPanelCanvasGroup.blocksRaycasts = interactable;
    }
}
