using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    [SerializeField] private float _fadeDuration = 0.5f;

    private bool _isTransitioning;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadScene(string sceneName, string targetSpawnPointId = "")
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(TransitionRoutine(sceneName, targetSpawnPointId));
    }

    private IEnumerator TransitionRoutine(string sceneName, string spawnId)
    {
        EnsurePlayerReference();
        _playerMovement?.SetFrozen(true);

        yield return StartCoroutine(Fade(1f));

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
        {
            yield return null;
        }

        if (!string.IsNullOrEmpty(spawnId))
        {
            PlacePlayerAtSpawn(spawnId);
        }

        yield return new WaitForSecondsRealtime(1f);

        yield return StartCoroutine(Fade(0f));

        _playerMovement?.SetFrozen(false);
        _isTransitioning = false;
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
            float delta = Mathf.Min(Time.unscaledDeltaTime, 1f / 30f);
            t += delta;
            _fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / _fadeDuration);
            yield return null;
        }

        _fadeCanvasGroup.alpha = targetAlpha;
        _fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.5f;
    }

    private void EnsurePlayerReference()
    {
        if (_playerMovement != null) return;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _playerMovement = playerObj.GetComponent<PlayerMovement>();
        }
    }

    //For loading game through main menu

    public void LoadSceneAtPosition(string sceneName, Vector2 position, System.Action onComplete = null)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(TransitionRoutineAtPosition(sceneName, position, onComplete));
    }

    private IEnumerator TransitionRoutineAtPosition(string sceneName, Vector2 position, System.Action onComplete)
    {
        EnsurePlayerReference();
        _playerMovement?.SetFrozen(true);

        yield return StartCoroutine(Fade(1f));

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
        {
            yield return null;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = position;
        }

        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(Fade(0f));

        _playerMovement?.SetFrozen(false);
        _isTransitioning = false;

        onComplete?.Invoke();
    }

    //Load scene if no checkpoints available
    public void LoadSceneAtDefaultSpawn(string sceneName, System.Action onComplete = null)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(TransitionRoutineDefaultSpawn(sceneName, onComplete));
    }

    private IEnumerator TransitionRoutineDefaultSpawn(string sceneName, System.Action onComplete)
    {
        EnsurePlayerReference();
        _playerMovement?.SetFrozen(true);

        yield return StartCoroutine(Fade(1f));

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
        {
            yield return null;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        var defaultSpawn = FindAnyObjectByType<SceneSpawnPoint>();
        if (player != null && defaultSpawn != null)
        {
            player.transform.position = defaultSpawn.transform.position;
        }

        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(Fade(0f));

        _playerMovement?.SetFrozen(false);
        _isTransitioning = false;

        onComplete?.Invoke();
    }

    //Quit to Main Menu
    public void LoadSceneSimple(string sceneName)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(TransitionRoutineSimple(sceneName));
    }

    private IEnumerator TransitionRoutineSimple(string sceneName)
    {
        yield return StartCoroutine(Fade(1f));
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(Fade(0f));
        _isTransitioning = false;
    }
}