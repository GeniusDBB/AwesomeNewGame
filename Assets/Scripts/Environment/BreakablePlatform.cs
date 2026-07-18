using UnityEngine;
using System.Collections;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float _breakDelay = 0.4f;   // time standing on it before it breaks
    [SerializeField] private float _respawnDelay = 2f;    // time before it reappears

    [Header("Warning Shake")]
    [SerializeField] private bool _shakeBeforeBreaking = true;
    [SerializeField] private float _shakeAmplitude = 0.05f;
    [SerializeField] private float _shakeFrequency = 25f;

    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _originalPosition;

    private bool _isBreaking;
    private bool _isBroken;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalPosition = transform.position;
    }

    public void NotifyStandingOn()
    {
        if (_isBreaking || _isBroken) return;

        _isBreaking = true;
        StartCoroutine(BreakSequence());
    }

    private IEnumerator BreakSequence()
    {
        if (_shakeBeforeBreaking)
        {
            float shakeTimer = 0f;
            while (shakeTimer < _breakDelay)
            {
                shakeTimer += Time.deltaTime;
                float offsetX = Mathf.Sin(shakeTimer * _shakeFrequency) * _shakeAmplitude;
                transform.position = _originalPosition + new Vector3(offsetX, 0f, 0f);
                yield return null;
            }
            transform.position = _originalPosition;
        }
        else
        {
            yield return new WaitForSeconds(_breakDelay);
        }

        Break();

        yield return new WaitForSeconds(_respawnDelay);

        Respawn();
    }

    private void Break()
    {
        _isBroken = true;
        _collider.enabled = false;
        _spriteRenderer.enabled = false;

        // hook a crumble particle effect or sound here
    }

    private void Respawn()
    {
        _isBroken = false;
        _isBreaking = false;
        transform.position = _originalPosition;
        _collider.enabled = true;
        _spriteRenderer.enabled = true;

        // hook a "pop back in" effect here
    }
}