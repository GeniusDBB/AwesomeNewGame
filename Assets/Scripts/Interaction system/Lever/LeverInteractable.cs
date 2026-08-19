using UnityEngine;

public class LeverInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private CinematicPlayer _cinematic;

    [SerializeField] private string _saveId;
    [SerializeField] private FakeWall _wall;
    private bool _used;

    private void Start()
    {
        if (SaveManager.Instance.HasFlag(_saveId))
        {
            _used = true;
            _wall.SnapOpen();
        }
    }

    public void Interact()
    {
        if (_used) return;
        _used = true;
        SaveManager.Instance.SetFlag(_saveId);
        _cinematic.Play();
    }
}