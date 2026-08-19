using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class SaveData
{
    public string CurrentScene;
    public float CheckpointX, CheckpointY;
    public int CurrentHealth;
    public int MaxHealth;

    public int CollectedKeys;
    public bool KeyQuestActive;
    public bool KeyQuestCompleted;

    public List<string> TriggeredFlags = new();
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private SaveData _savedData;
    private SaveData _workingData;
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public SaveData Data => _workingData;
    public event Action OnSaveReverted;

    private void Awake()
    {
        Instance = this;
        Load();
    }

    public bool HasFlag(string id) => _workingData.TriggeredFlags.Contains(id);

    public void SetFlag(string id)
    {
        if (!_workingData.TriggeredFlags.Contains(id))
        {
            _workingData.TriggeredFlags.Add(id);
        }
    }

    public void SetCheckpoint(string sceneName, Vector2 position)
    {
        _workingData.CurrentScene = sceneName;
        _workingData.CheckpointX = position.x;
        _workingData.CheckpointY = position.y;
        Save();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(_workingData, true);
        File.WriteAllText(SavePath, json);
        _savedData = DeepCopy(_workingData);
    }

    public void RevertToLastSave()
    {
        _workingData = DeepCopy(_savedData);
        OnSaveReverted?.Invoke();
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            _savedData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            _savedData = new SaveData();
        }

        _workingData = DeepCopy(_savedData);
    }

    public bool HasSaveFile() => File.Exists(SavePath);

    public void DeleteSaveAndReset()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
        _savedData = new SaveData();
        _workingData = new SaveData();
        OnSaveReverted?.Invoke();
    }

    private SaveData DeepCopy(SaveData source)
    {
        return JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(source));
    }
}