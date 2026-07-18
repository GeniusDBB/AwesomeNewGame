using UnityEngine;
using System.Collections.Generic;

public class SpiritCompanion : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform _player;

    [Header("Follow Feel")]
    [SerializeField] private float _followDelay = 0.15f; // higher = more lag behind player
    [SerializeField] private float _followSpeed = 8f;    // how snappily it catches up to its target trail point

    [Header("Floating Idle Motion")]
    [SerializeField] private float _bobAmplitude = 0.15f;
    [SerializeField] private float _bobFrequency = 2f;
    [SerializeField] private Vector2 _baseOffset = new Vector2(-0.5f, 0.8f); // resting position relative to player

    private readonly List<(Vector2 pos, float time)> _positionHistory = new();
    private float _bobTimer;
    private PlayerMovement _playerMovement;

    private void Awake()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        _playerMovement = _player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        RecordPlayerPosition();
        FollowPlayer();
        ApplyIdleBob();
    }

    private void RecordPlayerPosition()
    {
        _positionHistory.Add((_player.position, Time.time));

        // trim old entries we no longer need
        while (_positionHistory.Count > 0 && Time.time - _positionHistory[0].time > _followDelay + 0.5f)
        {
            _positionHistory.RemoveAt(0);
        }
    }

    private void FollowPlayer()
    {
        Vector2 facingAdjustedOffset = _baseOffset;
        facingAdjustedOffset.x = _playerMovement.IsFacingRight ? -Mathf.Abs(_baseOffset.x) : Mathf.Abs(_baseOffset.x);

        Vector2 targetPoint = GetDelayedPosition(_followDelay) + facingAdjustedOffset;
        transform.position = Vector2.Lerp(transform.position, targetPoint, _followSpeed * Time.deltaTime);

        float xScale = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(_playerMovement.IsFacingRight ? xScale : -xScale, transform.localScale.y, 1f);
    }

    private Vector2 GetDelayedPosition(float delay)
    {
        float targetTime = Time.time - delay;

        if (_positionHistory.Count == 0) return _player.position;

        // find the two recorded points surrounding targetTime and interpolate between them
        for (int i = _positionHistory.Count - 1; i > 0; i--)
        {
            if (_positionHistory[i - 1].time <= targetTime)
            {
                float t = Mathf.InverseLerp(_positionHistory[i - 1].time, _positionHistory[i].time, targetTime);
                return Vector2.Lerp(_positionHistory[i - 1].pos, _positionHistory[i].pos, t);
            }
        }

        return _positionHistory[0].pos;
    }

    private void ApplyIdleBob()
    {
        _bobTimer += Time.deltaTime * _bobFrequency;
        float bobOffset = Mathf.Sin(_bobTimer) * _bobAmplitude;
        transform.position += new Vector3(0f, bobOffset * Time.deltaTime, 0f);
    }

    //Call SnapToPlayer() right after any teleport/respawn/scene load logic in your game.
    public void SnapToPlayer()
    {
        _positionHistory.Clear();
        transform.position = (Vector2)_player.position + _baseOffset;
    }
}