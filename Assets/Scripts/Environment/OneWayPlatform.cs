using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private float _dropThroughDuration = 0.3f;
    private Collider2D _platformCollider;

    private void Awake()
    {
        _platformCollider = GetComponent<Collider2D>();
    }

    public void DropThrough(Collider2D playerCollider)
    {
        StartCoroutine(DropThroughRoutine(playerCollider));
    }

    private System.Collections.IEnumerator DropThroughRoutine(Collider2D playerCollider)
    {
        Physics2D.IgnoreCollision(playerCollider, _platformCollider, true);
        yield return new WaitForSeconds(_dropThroughDuration);
        Physics2D.IgnoreCollision(playerCollider, _platformCollider, false);
    }

    public float DropThroughDuration => _dropThroughDuration;
}