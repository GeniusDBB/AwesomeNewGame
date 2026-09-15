using UnityEngine;
using System.Collections;

public class PlayerVisualEffects : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _bodySprites;
    [SerializeField, Min(0.01f)] private float _flashDuration = 0.15f;

    private static readonly int FlashAmountId =
        Shader.PropertyToID("_FlashAmount");

    private MaterialPropertyBlock _propertyBlock;
    private Coroutine _flashRoutine;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
    }

    public void PlayHurtFlash()
    {
        if (!isActiveAndEnabled) return;

        ResetFlash();
        _flashRoutine = StartCoroutine(HurtFlashRoutine());
    }

    private IEnumerator HurtFlashRoutine()
    {
        SetFlashAmount(1f);

        // Keep the first white frame visible.
        yield return null;

        float elapsed = 0f;

        while (elapsed < _flashDuration)
        {
            elapsed += Time.deltaTime;

            float amount = 1f - Mathf.Clamp01(
                elapsed / _flashDuration
            );

            SetFlashAmount(amount);
            yield return null;
        }

        SetFlashAmount(0f);
        _flashRoutine = null;
    }

    private void SetFlashAmount(float amount)
    {
        if (_propertyBlock == null) return;

        foreach (SpriteRenderer sprite in _bodySprites)
        {
            if (sprite == null) continue;

            // Preserve any other property overrides on this sprite.
            sprite.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(FlashAmountId, amount);
            sprite.SetPropertyBlock(_propertyBlock);
        }
    }

    public void ResetFlash()
    {
        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
            _flashRoutine = null;
        }

        SetFlashAmount(0f);
    }

    private void OnDisable()
    {
        ResetFlash();
    }
}