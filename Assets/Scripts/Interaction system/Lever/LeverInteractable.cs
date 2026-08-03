using UnityEngine;
using Unity.Cinemachine;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineCamera _cutsceneCamera;
    [SerializeField] private FakeWall _wall;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    private bool _used;

    public void Interact()
    {
        if (_used) return;
        _used = true;

        PlayerMovement player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        StartCoroutine(CameraManager.instance.PlayLeverCinematic(_cutsceneCamera, _wall, _impulseSource, player));
    }
}