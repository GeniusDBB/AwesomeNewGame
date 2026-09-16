using System.Collections;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip _music;

    [Tooltip("If enabled, this scene stops the current music.")]
    [SerializeField] private bool _stopMusic;

    private IEnumerator Start()
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