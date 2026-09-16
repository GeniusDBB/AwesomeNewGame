using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    [TextArea(2, 4)]
    [SerializeField] private string _keyboardText;

    [TextArea(2, 4)]
    [SerializeField] private string _gamepadText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager.Instance.ShowTutorial(_keyboardText, _gamepadText, transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager.Instance.HideTutorial();
    }
}