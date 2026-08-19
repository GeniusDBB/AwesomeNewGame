using UnityEngine;
using System.Collections;

public class BenchCheckpoint : MonoBehaviour, IInteractable
{
    [SerializeField] private string _sceneName;
    [SerializeField] private Transform _sitPosition;
    [SerializeField] private float _sitDownDuration = 1f;
    [SerializeField] private float _getUpDuration = 1f;

    private bool _isSitting;
    private bool _isFullySeated;
    private PlayerMovement _player;
    private PlayerAnimator _animator;

    private void Update()
    {
        if (_isSitting && _isFullySeated && Mathf.Abs(InputManager.Movement.x) > 0.5f)
        {
            GetUp();
        }
    }

    public void Interact()
    {
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
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        _player = playerObj.GetComponent<PlayerMovement>();
        _animator = playerObj.GetComponent<PlayerAnimator>();

        _isSitting = true;
        _isFullySeated = false;
        _player.transform.position = _sitPosition.position;
        _player.SetFrozen(true);
        _animator.OnSitDown();

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