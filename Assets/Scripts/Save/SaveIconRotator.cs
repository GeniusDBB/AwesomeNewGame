using UnityEngine;

public class SaveIconRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 180f;

    private void Update()
    {
        transform.Rotate(0f, 0f, -_rotationSpeed * Time.deltaTime);
    }
}