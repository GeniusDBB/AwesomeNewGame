using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}