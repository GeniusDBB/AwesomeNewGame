using UnityEngine;
using System.Collections;

public class BenchCheckpoint : MonoBehaviour, IInteractable, IInteractionAvailability
{
    [SerializeField] private string _sceneName;
    [SerializeField] private Transform _sitPosition;
    [SerializeField] private float _sitDownDuration = 1f;
    [SerializeField] private float _getUpDuration = 1f;

    private bool _isSitting;
    private bool _isFullySeated;
    private bool _isOpeningSeat;
    private PlayerMovement _player;
    private PlayerAnimator _animator;

    public bool CanInteract => !_isSitting;
    public bool IsSitting => _isSitting;

    private void Update()
    {
        // The opening sequence seats the player before the Timeline starts.
        // Do not let horizontal input stand them up behind that sequence.
        if (_isOpeningSeat)
            return;

        if (DialogueManager.Instance != null &&
            !DialogueManager.Instance.IsDialogueFinished)
        {
            return;
        }

        if (_isSitting && _isFullySeated && Mathf.Abs(InputManager.Movement.x) > 0.5f)
        {
            GetUp();
        }
    }

    public void Interact()
    {
        if (_isOpeningSeat)
            return;

        if (_isSitting)
        {
            if (_isFullySeated) GetUp();
        }
        else
        {
            SitDown();
        }
    }

    private void SitDown()
    {
        BeginSitting(saveCheckpoint: true);
    }

    // Used by the opening sequence. It deliberately does not restore health,
    // write a checkpoint, or display the save icon.
    public void BeginOpeningSeat()
    {
        if (_isSitting) return;

        _isOpeningSeat = true;
        BeginSitting(saveCheckpoint: false);
    }

    /// <summary>
    /// Allows the player to get up once the new-game Timeline and dialogue
    /// have both completed.
    /// </summary>
    public void ReleaseOpeningSeat()
    {
        _isOpeningSeat = false;
    }

    private void BeginSitting(bool saveCheckpoint)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("Bench needs a Player to begin sitting.", this);
            return;
        }

        _player = playerObj.GetComponent<PlayerMovement>();
        _animator = playerObj.GetComponent<PlayerAnimator>();

        _isSitting = true;
        _isFullySeated = false;
        _player.transform.position = _sitPosition.position;
        // Benches may be placed above the floor, so do not let custom player gravity
        // pull the player away from the designer-defined sit position.
        _player.SetFrozen(true, ignoreGravity: true);
        _animator.OnSitDown();

        if (!saveCheckpoint)
        {
            StartCoroutine(SitDownCompleteRoutine());
            return;
        }

        PlayerHealth health = playerObj.GetComponent<PlayerHealth>();

        // Restore health before saving.
        health.RestoreFullHealth();

        // Include the restored health in the save data.
        SaveManager.Instance.Data.CurrentHealth = health.CurrentHealth;
        SaveManager.Instance.Data.MaxHealth = health.MaxHealth;

        SaveManager.Instance.SetCheckpoint(_sceneName, _sitPosition.position);
        UIManager.Instance.ShowSaveIcon();

        StartCoroutine(SitDownCompleteRoutine());
    }

    private IEnumerator SitDownCompleteRoutine()
    {
        yield return new WaitForSeconds(_sitDownDuration);
        _isFullySeated = true;
    }

    private void GetUp()
    {
        _isSitting = false;
        _isFullySeated = false;
        _animator.OnGetUp();
        StartCoroutine(GetUpRoutine());
    }

    private IEnumerator GetUpRoutine()
    {
        yield return new WaitForSeconds(_getUpDuration);
        _player.SetFrozen(false);
    }
}
