using UnityEngine;
using System.Collections;

public class PlayerRespawnHandler : MonoBehaviour
{
    [SerializeField] private float _deathDelay = 3.5f;
    private PlayerHealth _health;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        _health.OnPlayerDied += HandleDeath;
    }

    private void OnDisable()
    {
        _health.OnPlayerDied -= HandleDeath;
    }

    private void HandleDeath()
    {
        SaveManager.Instance.RevertToLastSave();
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSecondsRealtime(_deathDelay);

        var data = SaveManager.Instance.Data;

        if (!string.IsNullOrEmpty(data.CurrentScene))
        {
            SceneTransitionManager.Instance.LoadSceneAtPosition(
                data.CurrentScene,
                new Vector2(data.CheckpointX, data.CheckpointY),
                onBeforeReveal: ReviveLoadedPlayer);
        }
        else
        {
            SceneTransitionManager.Instance.LoadSceneAtDefaultSpawn(
                UnityEngine.SceneManagement.SceneManager
                    .GetActiveScene().name,
                onBeforeReveal: ReviveLoadedPlayer);
        }
    }

    // Resolve the player in the loaded scene instead of holding
    // a callback to a potentially destroyed PlayerHealth.
    private static void ReviveLoadedPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null &&
            player.TryGetComponent<PlayerHealth>(out var health))
        {
            health.Revive();
        }
    }
}