using UnityEngine;

public static class SpiritBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureSpiritExists()
    {
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