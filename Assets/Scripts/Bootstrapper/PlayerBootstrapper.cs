using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayerBootstrapper
{
    private const string MainMenuSceneName = "MainMenu";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        EnsurePlayerExists(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += EnsurePlayerExists;
    }

    private static void EnsurePlayerExists(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MainMenuSceneName) return;
        if (GameObject.FindGameObjectWithTag("Player") != null) return;

        GameObject prefab = Resources.Load<GameObject>("Player");
        if (prefab == null)
        {
            Debug.LogError("Player prefab not found in a Resources folder!");
            return;
        }

        GameObject player = Object.Instantiate(prefab);

        var defaultSpawn = Object.FindAnyObjectByType<SceneSpawnPoint>();
        if (defaultSpawn != null)
        {
            player.transform.position = defaultSpawn.transform.position;
        }
    }
}