using UnityEngine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private TMP_Text _questText;

    private bool _isPaused;

    private void Start()
    {
        _pausePanel.SetActive(false);

        QuestManager.Instance.OnQuestStateChanged += RefreshQuestText;
        KeyManager.Instance.OnKeyCountChanged += (_, __) => RefreshQuestText();
    }

    private void Update()
    {
        if (InputManager.PauseWasPressed)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        _isPaused = !_isPaused;
        _pausePanel.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;

        if (_isPaused) RefreshQuestText();
    }

    private void RefreshQuestText()
    {
        if (QuestManager.Instance.KeyQuestActive)
        {
            _questText.gameObject.SetActive(true);
            _questText.text = $"Collect all keys: {KeyManager.Instance.CollectedKeys}/{KeyManager.Instance.RequiredKeys}";
        }
        else
        {
            _questText.gameObject.SetActive(false);
        }
    }

    // hook these to your UI buttons
    public void OnContinuePressed() => TogglePause();
    public void OnExitPressed() => Application.Quit();
}