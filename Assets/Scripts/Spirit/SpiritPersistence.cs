using UnityEngine;

public class SpiritPersistence : MonoBehaviour
{
    private static SpiritPersistence _instance;
    private void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}