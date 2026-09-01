using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    [TextArea(2, 4)][SerializeField] private string _keyboardText;
    [TextArea(2, 4)][SerializeField] private string _gamepadText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        string text = InputManager.CurrentPrompt(_keyboardText, _gamepadText);
        UIManager.Instance.ShowTutorial(text);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        UIManager.Instance.HideTutorial();
    }
}