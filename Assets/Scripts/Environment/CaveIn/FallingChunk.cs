using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingChunk : MonoBehaviour
{
    [SerializeField] private float _lifetime = 4f;

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }
}