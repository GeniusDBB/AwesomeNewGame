using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FakeWall : MonoBehaviour
{
    public enum SlideDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private float _openDistance = 3f;
    [SerializeField] private float _openDuration = 1f;
    [SerializeField] private SlideDirection _slideDirection = SlideDirection.Up;

    [SerializeField] private CinemachineImpulseSource _impulseSource;

    public IEnumerator Open()
    {
        Vector3 start = transform.position;

        Vector3 direction = GetDirection();
        Vector3 end = start + direction * _openDistance;

        _impulseSource.GenerateImpulse();

        float t = 0f;

        while (t < _openDuration)
        {
            t += Time.deltaTime;

            transform.position = Vector3.Lerp(
                start,
                end,
                t / _openDuration
            );

            yield return null;
        }

        transform.position = end;
    }

    private Vector3 GetDirection()
    {
        return _slideDirection switch
        {
            SlideDirection.Up => Vector3.up,
            SlideDirection.Down => Vector3.down,
            SlideDirection.Left => Vector3.left,
            SlideDirection.Right => Vector3.right,
            _ => Vector3.up
        };
    }

    //For save system to snap open
    public void SnapOpen()
    {
        StopAllCoroutines();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        transform.position += GetDirection() * _openDistance;
    }
}