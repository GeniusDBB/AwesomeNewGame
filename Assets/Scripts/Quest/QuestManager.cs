using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public bool KeyQuestActive { get; private set; }
    public bool KeyQuestCompleted { get; private set; }

    public event Action OnQuestStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    #region KeyQuest
    public void StartKeyQuest()
    {
        if (KeyQuestActive) return;
        KeyQuestActive = true;
        OnQuestStateChanged?.Invoke();
    }

    public void CompleteKeyQuest()
    {
        KeyQuestActive = false;
        KeyQuestCompleted = true;
        OnQuestStateChanged?.Invoke();
    }
    #endregion
}