using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CinematicCaptionDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _captionGroup;
    [SerializeField] private TMP_Text _captionText;

    [TextArea(2, 5)]
    [SerializeField] private List<string> _captions = new();

    [SerializeField] private float _fadeDuration = 0.35f;

    private int _nextCaptionIndex;
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        SetVisibleImmediately(false);
    }

    public void ResetCaptions()
    {
        _nextCaptionIndex = 0;
        SetVisibleImmediately(false);
    }

    public void ShowNext()
    {
        if (_nextCaptionIndex >= _captions.Count)
        {
            Hide();
            return;
        }

        _captionText.text = _captions[_nextCaptionIndex];
        _nextCaptionIndex++;

        FadeTo(1f);
    }

    public void Hide()
    {
        FadeTo(0f);
    }

    private void FadeTo(float targetAlpha)
    {
        if (_captionGroup == null)
            return;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = _captionGroup.alpha;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _captionGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / _fadeDuration
            );

            yield return null;
        }

        _captionGroup.alpha = targetAlpha;
        _fadeRoutine = null;
    }

    private void SetVisibleImmediately(bool visible)
    {
        if (_captionGroup == null)
            return;

        _captionGroup.alpha = visible ? 1f : 0f;
        _captionGroup.interactable = false;
        _captionGroup.blocksRaycasts = false;
    }
}