using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private string _newGameSceneName;
    [SerializeField] private string _newGameSpawnId;

    private void Start()
    {
        _continueButton.SetActive(SaveManager.Instance.HasSaveFile());
    }

    public void OnNewGamePressed()
    {
        SaveManager.Instance.DeleteSaveAndReset();
        SceneTransitionManager.Instance.LoadScene(_newGameSceneName, _newGameSpawnId);
    }

    public void OnContinuePressed()
    {
        var data = SaveManager.Instance.Data;
        SceneTransitionManager.Instance.LoadSceneAtPosition(data.CurrentScene, new Vector2(data.CheckpointX, data.CheckpointY));
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}