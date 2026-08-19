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
        yield return new WaitForSeconds(_deathDelay);

        var data = SaveManager.Instance.Data;

        if (!string.IsNullOrEmpty(data.CurrentScene))
        {
            SceneTransitionManager.Instance.LoadSceneAtPosition(
                data.CurrentScene,
                new Vector2(data.CheckpointX, data.CheckpointY),
                _health.Revive);
        }
        else
        {
            SceneTransitionManager.Instance.LoadSceneAtDefaultSpawn(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                _health.Revive);
        }
    }
}