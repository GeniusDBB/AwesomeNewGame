using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CaveInSequence : MonoBehaviour
{
    [SerializeField] private GameObject _chunkPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _delayBetweenChunks = 0.5f;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    public void StartCaveIn()
    {
        StartCoroutine(CaveInRoutine());
    }

    private IEnumerator CaveInRoutine()
    {
        foreach (var point in _spawnPoints)
        {
            Instantiate(_chunkPrefab, point.position, Quaternion.identity);
            _impulseSource.GenerateImpulse();  // Shake za svaki instantiate
            yield return new WaitForSeconds(_delayBetweenChunks);
        }
    }
}