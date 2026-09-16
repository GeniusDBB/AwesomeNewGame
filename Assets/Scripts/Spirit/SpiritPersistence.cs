using UnityEngine;
using UnityEngine.SceneManagement;

public class SpiritPersistence : MonoBehaviour
{
    private static SpiritPersistence _instance;
    private void Awake()
    {
        if (_instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckIfMainMenu;
        CheckIfMainMenu(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CheckIfMainMenu;
    }
    private void CheckIfMainMenu(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
