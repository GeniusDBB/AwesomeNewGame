using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FakeWall : MonoBehaviour
{
    [SerializeField] private float _openDistance = 3f;
    [SerializeField] private float _openDuration = 1f;

    [SerializeField] private CinemachineImpulseSource impulseSource;

    public IEnumerator Open()
    {
        //Collider2D col = GetComponent<Collider2D>();
        //if (col != null) col.enabled = false;

        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * _openDistance; // slides up

        impulseSource.GenerateImpulse(); //generates shake

        float t = 0f;
        while (t < _openDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t / _openDuration);
            yield return null;
        }
    }
}   