using UnityEngine;
using System.Collections;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float _breakDelay = 0.4f;
    [SerializeField] private float _breakAnimationDuration = 0.5f;
    [SerializeField] private float _respawnDelay = 2f;

    [Header("Warning Shake")]
    [SerializeField] private bool _shakeBeforeBreaking = true;
    [SerializeField] private float _shakeAmplitude = 0.05f;
    [SerializeField] private float _shakeFrequency = 25f;

    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Vector3 _originalPosition;

    private bool _isBreaking;
    private bool _isBroken;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _originalPosition = transform.position;
    }

    public void NotifyStandingOn()
    {
        if (_isBreaking || _isBroken)
            return;

        _isBreaking = true;
        StartCoroutine(BreakSequence());
    }

    private IEnumerator BreakSequence()
    {
        // Starts the crumble animation immediately, at the same time as the shake.
        _animator.SetBool("Break", true);

        float totalDuration = Mathf.Max(_breakDelay, _breakAnimationDuration);
        float timer = 0f;

        while (timer < totalDuration)
        {
            timer += Time.deltaTime;

            // Shake only for _breakDelay seconds.
            if (_shakeBeforeBreaking && timer < _breakDelay)
            {
                float offsetX = Mathf.Sin(timer * _shakeFrequency) * _shakeAmplitude;
                transform.position = _originalPosition + new Vector3(offsetX, 0f, 0f);
            }
            else
            {
                transform.position = _originalPosition;
            }

            yield return null;
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
    }

    private void Respawn()
    {
        _isBroken = false;
        _isBreaking = false;
        transform.position = _originalPosition;

        _collider.enabled = true;
        _spriteRenderer.enabled = true;

        // Reset the Animator Bool so it can return to Idle.
        _animator.SetBool("Break", false);

        // Force the default idle state and its first frame.
        _animator.Play("BreakablePlatformIdle", 0, 0f);
    }
}