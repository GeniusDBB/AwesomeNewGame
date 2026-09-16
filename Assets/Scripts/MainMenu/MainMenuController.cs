using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private string _newGameSceneName;
    [SerializeField] private string _newGameSpawnId;

    [Header("Panels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _optionsPanel;

    [Header("Selection")]
    [SerializeField] private GameObject _mainFirstSelected;
    [SerializeField] private GameObject _optionsFirstSelected;
    [SerializeField] private GameObject _optionsButton;

    private bool _optionsOpen;
    private bool _leavingScene;
    private int _lastMenuChangeFrame = -1;

    private void Awake()
    {
        _optionsPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }

    private void Start()
    {
        _continueButton.SetActive(SaveManager.Instance.HasSaveFile());
        InputManager.SetMenuInput(true);
        ShowMenu(false, _mainFirstSelected);
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
        SaveManager.Instance.DeleteSaveAndReset();
        SceneTransitionManager.Instance.LoadScene(_newGameSceneName, _newGameSpawnId);
    }

    public void OnContinuePressed()
    {
        if (_optionsOpen || _leavingScene ||
            !SaveManager.Instance.HasSaveFile()) return;

        _leavingScene = true;
        var data = SaveManager.Instance.Data;
        SceneTransitionManager.Instance.LoadSceneAtPosition(data.CurrentScene, new Vector2(data.CheckpointX, data.CheckpointY));
    }

    public void OnQuitPressed()
    {
        if (_optionsOpen || _leavingScene) return;

        Application.Quit();
    }
}
