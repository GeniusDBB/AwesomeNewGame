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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        OnKeyCountChanged?.Invoke(_collectedKeys, _requiredKeys);
    }

    public void AddKey()
    {
        _collectedKeys = Mathf.Min(_collectedKeys + 1, _requiredKeys);
        OnKeyCountChanged?.Invoke(_collectedKeys, _requiredKeys);
    }
}