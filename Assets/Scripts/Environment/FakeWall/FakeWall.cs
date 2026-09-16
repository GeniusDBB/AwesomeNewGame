using System.Collections;
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
    [SerializeField]
    private SlideDirection _slideDirection =
        SlideDirection.Up;

    [Header("Door Camera Shake")]
    [SerializeField] private DoorCameraShake _cameraShake;

    public IEnumerator Open()
    {
        Vector3 start = transform.position;
        Vector3 end = start + GetDirection() * _openDistance;

        if (_cameraShake != null)
            _cameraShake.PlayShake(_openDuration);

        float t = 0f;

        while (t < _openDuration)
        {
            t += Time.deltaTime;

            transform.position = Vector3.Lerp(
                start,
                end,
                t / _openDuration);

            yield return null;
        }

        transform.position = end;

        if (_cameraShake != null)
            _cameraShake.StopShake();
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

    public void SnapOpen()
    {
        StopAllCoroutines();

        if (_cameraShake != null)
            _cameraShake.StopShake();

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        transform.position += GetDirection() * _openDistance;
    }

    private void OnDisable()
    {
        if (_cameraShake != null)
            _cameraShake.StopShake();
    }
}