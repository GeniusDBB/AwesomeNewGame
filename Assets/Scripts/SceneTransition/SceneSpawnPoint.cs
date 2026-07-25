using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    [SerializeField] private string _spawnId;
    public string SpawnId => _spawnId;
}