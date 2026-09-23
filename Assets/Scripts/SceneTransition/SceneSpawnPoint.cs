using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    [SerializeField] private string _spawnId;
    public string SpawnId => _spawnId;

    public void SetSpawnId(string spawnId)
    {
        _spawnId = spawnId;
    }
}
