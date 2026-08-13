using UnityEngine;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private CinematicPlayer _cinematic;
    private bool _used;

    public void Interact()
    {
        if (_used) return;
        _used = true;
        _cinematic.Play();
    }
}