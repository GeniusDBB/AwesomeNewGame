using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private AudioManager _audio;
    private Coroutine _bindRoutine;

    private void OnEnable()
    {
        _bindRoutine = StartCoroutine(BindWhenReady());
    }

    private IEnumerator BindWhenReady()
    {
        if (_musicSlider == null || _sfxSlider == null)
        {
            Debug.LogError(
                "AudioSettingsUI: assign both sliders.",
                this);

            yield break;
        }

        while (AudioManager.Instance == null ||
               !AudioManager.Instance.IsReady)
        {
            yield return null;
        }

        _audio = AudioManager.Instance;

        RefreshSliders();

        _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        _sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);

        _audio.OnVolumeChanged += RefreshSliders;
    }

    private void RefreshSliders()
    {
        if (_audio == null) return;

        // Updating the display must not trigger another volume change.
        _musicSlider.SetValueWithoutNotify(_audio.MusicVolume);
        _sfxSlider.SetValueWithoutNotify(_audio.SFXVolume);
    }

    private void ChangeMusicVolume(float value)
    {
        if (_audio != null)
            _audio.SetMusicVolume(value);
    }

    private void ChangeSFXVolume(float value)
    {
        if (_audio != null)
            _audio.SetSFXVolume(value);
    }

    private void OnDisable()
    {
        if (_bindRoutine != null)
        {
            StopCoroutine(_bindRoutine);
            _bindRoutine = null;
        }

        if (_musicSlider != null)
            _musicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);

        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.RemoveListener(ChangeSFXVolume);

        if (_audio != null)
        {
            _audio.OnVolumeChanged -= RefreshSliders;
            _audio.SaveSettings();
            _audio = null;
        }
    }
}