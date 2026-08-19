using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public bool KeyQuestActive => SaveManager.Instance.Data.KeyQuestActive;
    public bool KeyQuestCompleted => SaveManager.Instance.Data.KeyQuestCompleted;

    public event Action OnQuestStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    #region KeyQuest
    public void StartKeyQuest()
    {
        if (KeyQuestActive) return;
        SaveManager.Instance.Data.KeyQuestActive = true;
        OnQuestStateChanged?.Invoke();
    }

    public void CompleteKeyQuest()
    {
        SaveManager.Instance.Data.KeyQuestActive = false;
        SaveManager.Instance.Data.KeyQuestCompleted = true;
        OnQuestStateChanged?.Invoke();
    }
    #endregion
}