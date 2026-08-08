using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        KeyManager.Instance.AddKey();
        
        if (!QuestManager.Instance.KeyQuestActive && !QuestManager.Instance.KeyQuestCompleted)
        {
            QuestManager.Instance.StartKeyQuest();
        }

        Destroy(gameObject);
    }
}