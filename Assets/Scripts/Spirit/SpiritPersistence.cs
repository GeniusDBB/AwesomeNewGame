using UnityEngine;
using UnityEngine.SceneManagement;

public class SpiritPersistence : MonoBehaviour
{
    private static SpiritPersistence _instance;
    private void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckIfMainMenu;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CheckIfMainMenu;
    }
    private void CheckIfMainMenu(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") Destroy(gameObject);
    }
}