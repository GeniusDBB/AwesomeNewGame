using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string _saveId;

    private void Start()
    {
        if (SaveManager.Instance.HasFlag(_saveId))
        {
            Destroy(gameObject);
        }
    }

    public void Interact()
    {
        KeyManager.Instance.AddKey();
        SaveManager.Instance.SetFlag(_saveId);

        if (!QuestManager.Instance.KeyQuestActive && !QuestManager.Instance.KeyQuestCompleted)
        {
            QuestManager.Instance.StartKeyQuest();
        }

        Destroy(gameObject);
    }
}