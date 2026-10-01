using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WindAudioZone : MonoBehaviour
{
    [SerializeField] private AudioSource windAudio;
    [SerializeField, Range(0f, 1f)] private float targetVolume = 0.4f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.6f;

    private readonly HashSet<Collider2D> playerColliders =
        new HashSet<Collider2D>();

    private bool isFading;
    private float fadeElapsed;
    private float fadeStartVolume;
    private float fadeTargetVolume;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        windAudio = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (windAudio == null)
            windAudio = GetComponent<AudioSource>();

        windAudio.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RegisterPlayer(other);
    }

    // This also handles starting the game while the player is already inside.
    private void OnTriggerStay2D(Collider2D other)
    {
        RegisterPlayer(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerColliders.Remove(other);

        if (playerColliders.Count == 0)
            FadeTo(0f);
    }

    private void RegisterPlayer(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        bool wasOutsideZone = playerColliders.Count == 0;
        playerColliders.Add(other);

        if (wasOutsideZone)
            FadeTo(targetVolume);
    }

    private bool IsPlayer(Collider2D other)
    {
        return other.CompareTag("Player") ||
               (other.attachedRigidbody != null &&
                other.attachedRigidbody.CompareTag("Player")) ||
               other.transform.root.CompareTag("Player");
    }

    private void FadeTo(float volume)
    {
        // Trigger exit events can be raised while this zone is being disabled.
        // An inactive GameObject cannot start a coroutine.
        if (!isActiveAndEnabled || windAudio == null)
            return;

        // An exit can be reported during Intro's startup cleanup before this
        // zone has ever played. There is nothing to fade in that case.
        if (volume <= 0f && !windAudio.isPlaying)
        {
            windAudio.volume = 0f;
            return;
        }

        if (volume > 0f && !windAudio.isPlaying)
        {
            windAudio.volume = 0f;
            windAudio.Play();
        }

        fadeStartVolume = windAudio.volume;
        fadeTargetVolume = Mathf.Clamp01(volume);
        fadeElapsed = 0f;
        isFading = true;
    }

    private void Update()
    {
        if (!isFading || windAudio == null)
            return;

        fadeElapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(fadeElapsed / fadeDuration);
        windAudio.volume = Mathf.Lerp(fadeStartVolume, fadeTargetVolume, progress);

        if (progress < 1f)
            return;

        isFading = false;

        if (fadeTargetVolume <= 0f)
            windAudio.Stop();
    }

    private void OnDisable()
    {
        isFading = false;
        playerColliders.Clear();

        if (windAudio != null)
            windAudio.Stop();
    }
}
