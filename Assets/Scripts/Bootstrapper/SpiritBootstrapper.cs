using UnityEngine;
using UnityEngine.SceneManagement;

public static class SpiritBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        EnsureSpiritExists(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += EnsureSpiritExists;
    }
    private static void EnsureSpiritExists(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        if (Object.FindAnyObjectByType<SpiritPersistence>() != null) return;

        GameObject prefab = Resources.Load<GameObject>("SpiritCompanion");
        if (prefab == null)
        {
            Debug.LogError("SpiritCompanion prefab not found in a Resources folder!");
            return;
        }

        Object.Instantiate(prefab);
    }
}