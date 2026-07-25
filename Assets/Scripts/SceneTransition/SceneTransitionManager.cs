using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    [SerializeField] private float _fadeDuration = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadScene(string sceneName, string targetSpawnPointId = "")
    {
        StartCoroutine(TransitionRoutine(sceneName, targetSpawnPointId));
    }

    private IEnumerator TransitionRoutine(string sceneName, string spawnId)
    {
        yield return StartCoroutine(Fade(1f)); // fade to black

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
        {
            yield return null;
        }

        if (!string.IsNullOrEmpty(spawnId))
        {
            PlacePlayerAtSpawn(spawnId);
        }

        yield return StartCoroutine(Fade(0f)); // fade back in
    }

    private void PlacePlayerAtSpawn(string spawnId)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No Player found in scene during spawn placement!");
            return;
        }

        var spawnPoints = FindObjectsByType<SceneSpawnPoint>(FindObjectsSortMode.None);
        foreach (var point in spawnPoints)
        {
            if (point.SpawnId == spawnId)
            {
                player.transform.position = point.transform.position;
                return;
            }
        }

        Debug.LogWarning($"No spawn point found with id: {spawnId}");
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = _fadeCanvasGroup.alpha;
        float t = 0f;

        _fadeCanvasGroup.blocksRaycasts = true;

        while (t < _fadeDuration)
        {
            t += Time.unscaledDeltaTime; // unscaled in case paused/timeScale weirdness
            _fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / _fadeDuration);
            yield return null;
        }

        _fadeCanvasGroup.alpha = targetAlpha;
        _fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.5f;
    }
}