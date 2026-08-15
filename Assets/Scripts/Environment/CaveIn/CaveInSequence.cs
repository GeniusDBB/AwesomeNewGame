using UnityEngine;
using System.Collections;

public class CaveInSequence : MonoBehaviour
{
    [SerializeField] private GameObject _chunkPrefab;
    [SerializeField] private Transform[] _spawnPoints; // ordered along the ceiling, start-to-end of the passage
    [SerializeField] private float _delayBetweenChunks = 0.5f;

    public void StartCaveIn()
    {
        StartCoroutine(CaveInRoutine());
    }

    private IEnumerator CaveInRoutine()
    {
        foreach (var point in _spawnPoints)
        {
            Instantiate(_chunkPrefab, point.position, Quaternion.identity);
            yield return new WaitForSeconds(_delayBetweenChunks);
        }
    }
}