using UnityEngine;
using System;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    [SerializeField] private int _requiredKeys = 3;

    private int _collectedKeys;

    public int CollectedKeys => _collectedKeys;
    public int RequiredKeys => _requiredKeys;
    public bool HasAllKeys => _collectedKeys >= _requiredKeys;

    public event Action<int, int> OnKeyCountChanged; // (current, required)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _collectedKeys = Mathf.Min(SaveManager.Instance.Data.CollectedKeys, _requiredKeys);
        OnKeyCountChanged?.Invoke(_collectedKeys, _requiredKeys);

        SaveManager.Instance.OnSaveReverted += RefreshFromSave;
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.OnSaveReverted -= RefreshFromSave;
    }

    private void RefreshFromSave()
    {
        _collectedKeys = Mathf.Min(SaveManager.Instance.Data.CollectedKeys, _requiredKeys);
        OnKeyCountChanged?.Invoke(_collectedKeys, _requiredKeys);
    }

    public void AddKey()
    {
        _collectedKeys = Mathf.Min(_collectedKeys + 1, _requiredKeys);
        SaveManager.Instance.Data.CollectedKeys = _collectedKeys;
        OnKeyCountChanged?.Invoke(_collectedKeys, _requiredKeys);
    }
}