using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    [TextArea(2, 4)]
    [SerializeField] private string _keyboardText;

    [TextArea(2, 4)]
    [SerializeField] private string _gamepadText;

    private bool _playerInside;

    private void Update()
    {
        // The pause menu hides tutorials immediately. Restore this zone's prompt
        // once play resumes, without requiring the player to leave and re-enter.
        if (!_playerInside || Time.timeScale == 0f) return;

        UIManager uiManager = UIManager.Instance;
        if (uiManager != null && !uiManager.IsTutorialVisible)
        {
            uiManager.ShowTutorial(_keyboardText, _gamepadText, transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInside = true;

        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            uiManager.ShowTutorial(_keyboardText, _gamepadText, transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInside = false;

        // This can run while its scene is unloading. The old UI manager may already
        // have been destroyed, before the next scene has created its replacement.
        UIManager uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            uiManager.HideTutorial();
        }
    }
}
