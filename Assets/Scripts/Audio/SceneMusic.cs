using System.Collections;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip _music;

    [Tooltip("Disable this when another sequence should decide when the scene music begins.")]
    [SerializeField] private bool _playOnSceneStart = true;

    [Tooltip("If enabled, this scene stops the current music.")]
    [SerializeField] private bool _stopMusic;

    private bool _playRequested;

    private void Start()
    {
        if (_playOnSceneStart)
            Play();
    }

    public void Play()
    {
        if (_playRequested) return;

        _playRequested = true;
        StartCoroutine(PlayWhenAudioIsReady());
    }

    private IEnumerator PlayWhenAudioIsReady()
    {
        while (AudioManager.Instance == null ||
               !AudioManager.Instance.IsReady)
        {
            yield return null;
        }

        if (_stopMusic)
        {
            AudioManager.Instance.StopMusic();
        }
        else if (_music != null)
        {
            AudioManager.Instance.PlayMusic(_music);
        }
    }
}
