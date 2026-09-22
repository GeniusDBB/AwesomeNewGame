using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-100)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioMixerGroup _musicGroup;
    [SerializeField] private AudioMixerGroup _sfxGroup;

    [Header("First-launch volume")]
    [SerializeField, Range(0f, 1f)]
    private float _defaultMusicVolume = 0.7f;

    [SerializeField, Range(0f, 1f)]
    private float _defaultSFXVolume = 0.8f;

    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public bool IsReady { get; private set; }

    public event Action OnVolumeChanged;

    private const string MusicKey = "Audio.MusicVolume";
    private const string SFXKey = "Audio.SFXVolume";

    private AudioSource _musicSource;
    private AudioSource _sfxSource;
    private Coroutine _musicResumeRoutine;
    private bool _settingsDirty;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Instance = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        if (_mixer == null ||
            _musicGroup == null ||
            _sfxGroup == null)
        {
            Debug.LogError("AudioManager: assign the mixer and both mixer groups.", this);
            enabled = false;
            return;
        }

        Instance = this;

        MusicVolume = Mathf.Clamp01(
            PlayerPrefs.GetFloat(MusicKey, _defaultMusicVolume));

        SFXVolume = Mathf.Clamp01(
            PlayerPrefs.GetFloat(SFXKey, _defaultSFXVolume));

        _musicSource = CreateSource("Music Source", _musicGroup);
        _musicSource.loop = true;

        _sfxSource = CreateSource("SFX Source", _sfxGroup);
    }

    private AudioSource CreateSource(
        string objectName,
        AudioMixerGroup output)
    {
        GameObject sourceObject = new GameObject(objectName);
        sourceObject.transform.SetParent(transform, false);

        AudioSource source = sourceObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 1f;
        source.outputAudioMixerGroup = output;

        return source;
    }

    private void Start()
    {
        ApplyMixerVolume("MusicVolume", MusicVolume);
        ApplyMixerVolume("SFXVolume", SFXVolume);

        IsReady = true;
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);

        if (IsReady)
            ApplyMixerVolume("MusicVolume", MusicVolume);

        PlayerPrefs.SetFloat(MusicKey, MusicVolume);
        _settingsDirty = true;

        OnVolumeChanged?.Invoke();
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);

        if (IsReady)
            ApplyMixerVolume("SFXVolume", SFXVolume);

        PlayerPrefs.SetFloat(SFXKey, SFXVolume);
        _settingsDirty = true;

        OnVolumeChanged?.Invoke();
    }

    private void ApplyMixerVolume(string parameter, float value)
    {
        float decibels = value <= 0f
            ? -80f
            : Mathf.Max(-80f, Mathf.Log10(value) * 20f);

        if (!_mixer.SetFloat(parameter, decibels))
        {
            Debug.LogError(
                $"AudioManager: check exposed mixer parameter '{parameter}'.",
                this);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!IsReady || clip == null) return;

        CancelPendingMusicResume();
        PlayMusicInternal(clip, loop);
    }

    public void PlayMusicOnceThenResume(AudioClip clip)
    {
        if (!IsReady || clip == null) return;

        AudioClip previousClip = _musicSource.clip;
        bool previousWasPlaying = _musicSource.isPlaying;
        bool previousLoop = _musicSource.loop;
        float previousTime = _musicSource.time;

        CancelPendingMusicResume();
        PlayMusicInternal(clip, loop: false);

        if (previousClip != null && previousWasPlaying)
        {
            _musicResumeRoutine = StartCoroutine(
                ResumeMusicAfterClip(
                    clip,
                    previousClip,
                    previousLoop,
                    previousTime));
        }
    }

    private void PlayMusicInternal(
        AudioClip clip,
        bool loop,
        float startTime = 0f)
    {

        // Keep the current playback position across scene changes.
        if (_musicSource.clip == clip &&
            _musicSource.isPlaying &&
            _musicSource.loop == loop)
            return;

        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.time = Mathf.Clamp(startTime, 0f, clip.length);
        _musicSource.Play();
    }

    private IEnumerator ResumeMusicAfterClip(
        AudioClip temporaryClip,
        AudioClip previousClip,
        bool previousLoop,
        float previousTime)
    {
        while (_musicSource != null &&
               _musicSource.clip == temporaryClip &&
               _musicSource.isPlaying)
        {
            yield return null;
        }

        // A different system took over the music channel, so it decides what plays next.
        if (_musicSource == null || _musicSource.clip != temporaryClip)
            yield break;

        _musicResumeRoutine = null;
        PlayMusicInternal(previousClip, previousLoop, previousTime);
    }

    private void CancelPendingMusicResume()
    {
        if (_musicResumeRoutine == null) return;

        StopCoroutine(_musicResumeRoutine);
        _musicResumeRoutine = null;
    }

    public void StopMusic()
    {
        if (_musicSource == null) return;

        CancelPendingMusicResume();
        _musicSource.Stop();
        _musicSource.clip = null;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (!IsReady || clip == null) return;

        _sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    public void SaveSettings()
    {
        if (!_settingsDirty) return;

        PlayerPrefs.Save();
        _settingsDirty = false;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            SaveSettings();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SaveSettings();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        SaveSettings();
        Instance = null;
    }
}
