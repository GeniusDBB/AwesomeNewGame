using UnityEngine;

public static class GameBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsurePersistentExists()
    {
        if (Object.FindAnyObjectByType<PersistentRoot>() != null) return;

        GameObject prefab = Resources.Load<GameObject>("Persistent");
        if (prefab == null)
        {
            Debug.LogError("Persistent prefab not found in a Resources folder!");
            return;
        }

        Object.Instantiate(prefab);
    }
}