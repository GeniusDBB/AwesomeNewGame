using UnityEngine;
using Unity.Cinemachine;

[DisallowMultipleComponent]
public class DoorCameraShake : CinemachineExtension
{
    [Tooltip("Zoom vibration strength. 0.008 means 0.8%.")]
    [SerializeField, Range(0f, 0.05f)]
    private float _amount = 0.008f;

    [Tooltip("How quickly the zoom vibrates.")]
    [SerializeField, Min(0.1f)]
    private float _frequency = 8f;

    [Tooltip("Time used to ease the shake in and out.")]
    [SerializeField, Min(0.01f)]
    private float _fadeTime = 0.15f;

    private bool _isPlaying;
    private float _startTime;
    private float _duration;

    public void PlayShake(float duration)
    {
        if (!isActiveAndEnabled || duration <= 0f)
            return;

        _startTime = Time.time;
        _duration = duration;
        _isPlaying = true;
    }

    public void StopShake()
    {
        _isPlaying = false;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Finalize ||
            !Application.isPlaying ||
            !_isPlaying)
        {
            return;
        }

        float elapsed = Time.time - _startTime;

        if (elapsed >= _duration)
        {
            _isPlaying = false;
            return;
        }

        float fade = Mathf.Min(
            Mathf.Max(0.01f, _fadeTime),
            _duration * 0.5f);

        float fadeIn = Mathf.Clamp01(elapsed / fade);
        float fadeOut = Mathf.Clamp01(
            (_duration - elapsed) / fade);

        float envelope = Mathf.SmoothStep(
            0f, 1f, Mathf.Min(fadeIn, fadeOut));

        float phase =
            elapsed * _frequency * Mathf.PI * 2f;

        float vibration =
            Mathf.Sin(phase) * 0.7f +
            Mathf.Sin(phase * 1.73f) * 0.3f;

        // Modify this frame's output, not the camera's saved size.
        var lens = state.Lens;

        lens.OrthographicSize *=
            1f + vibration * _amount * envelope;

        state.Lens = lens;
    }
}