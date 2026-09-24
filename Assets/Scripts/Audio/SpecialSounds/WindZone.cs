using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WindZone : MonoBehaviour
{
    [SerializeField] private AudioSource windAudio;
    [SerializeField, Range(0f, 1f)] private float targetVolume = 0.4f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.6f;

    private readonly HashSet<Collider2D> playerColliders =
        new HashSet<Collider2D>();

    private Coroutine fadeRoutine;

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
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (volume > 0f && !windAudio.isPlaying)
        {
            windAudio.volume = 0f;
            windAudio.Play();
        }

        fadeRoutine = StartCoroutine(FadeRoutine(volume));
    }

    private IEnumerator FadeRoutine(float target)
    {
        float start = windAudio.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            windAudio.volume = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }

        windAudio.volume = target;

        if (target <= 0f)
            windAudio.Stop();

        fadeRoutine = null;
    }

    private void OnDisable()
    {
        playerColliders.Clear();

        if (windAudio != null)
            windAudio.Stop();
    }
}