using UnityEngine;

/// <summary>
/// An automatic left/right room exit. Put it on a trigger collider positioned
/// in a doorway opening; the player does not need to press an interact button.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class SideRoomTransition : MonoBehaviour
{
    [SerializeField] private string _targetScene;
    [SerializeField] private string _targetSpawnId;
    [SerializeField] private float _entryDirection = 1f;
    [SerializeField, Min(0f)] private float _entryRunDuration = 0.22f;

    public void Configure(
        string targetScene,
        string targetSpawnId,
        float entryDirection,
        float entryRunDuration = 0.22f)
    {
        _targetScene = targetScene;
        _targetSpawnId = targetSpawnId;
        _entryDirection = Mathf.Sign(entryDirection);
        _entryRunDuration = entryRunDuration;
    }

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") ||
            string.IsNullOrWhiteSpace(_targetScene) ||
            SceneTransitionManager.Instance == null)
        {
            return;
        }

        SceneTransitionManager.Instance.LoadDoorwayScene(
            _targetScene,
            _targetSpawnId,
            _entryDirection,
            _entryRunDuration);
    }
}
