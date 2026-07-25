using UnityEngine;

public static class PlayerBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsurePlayerExists()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null) return;

        GameObject prefab = Resources.Load<GameObject>("Player");
        if (prefab == null)
        {
            Debug.LogError("Player prefab not found in a Resources folder!");
            return;
        }

        GameObject player = Object.Instantiate(prefab);

        // no incoming transition, so just drop them at whatever default spawn exists in this scene
        var defaultSpawn = Object.FindAnyObjectByType<SceneSpawnPoint>();
        if (defaultSpawn != null)
        {
            player.transform.position = defaultSpawn.transform.position;
        }
    }
}