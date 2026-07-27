using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class SimpleLoadingScreen : LoadingScreen
{
    [Header("Components")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _progressDescription;
    [SerializeField] private Slider _progressSlider;

    [Header("Parameters")]
    [SerializeField] private float _minimumVisibleTime = 1f;
    [SerializeField] private float _progressAnimationTime = 0.3f;
    [SerializeField] private float _fadeInTime = 0.3f;
    [SerializeField] private float _fadeOutTime = 0.3f;

    private Tween _fadeTween;
    private Tween _progressTween;
    private double _shownAt;
    private float _targetProgress;

    public override async UniTask Show(CancellationToken ct = default)
    {
        PrepareToShow();

        await FadeTo(
            targetAlpha: 1f,
            duration: _fadeInTime,
            ct);
    }

    public override void ShowImmediate()
    {
        PrepareToShow();
        _canvasGroup.alpha = 1f;
    }

    public override async UniTask Hide(CancellationToken ct = default)
    {
        await WaitForMinimumShowTime(ct);
        await CompleteProgress(ct);

        _canvasGroup.blocksRaycasts = false;

        await FadeTo(
            targetAlpha: 0f,
            duration: _fadeOutTime,
            ct);

        _canvasGroup.gameObject.SetActive(false);
    }

    public override void HideImmediate()
    {
        KillAnimations();

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.gameObject.SetActive(false);

        _targetProgress = 0f;
        _progressSlider.value = 0f;
    }

    public override void SetProgressDescription(string description)
    {
        _progressDescription.text = description;
    }

    protected override void UpdateProgress(float progress)
    {
        float normalizedProgress = Mathf.Clamp01(progress);

        _targetProgress = Mathf.Max(_targetProgress, normalizedProgress);

        AnimateProgressTo(_targetProgress);
    }

    private void PrepareToShow()
    {
        KillAnimations();

        _shownAt = Time.realtimeSinceStartupAsDouble;
        _targetProgress = 0f;

        _progressSlider.value = 0f;

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.gameObject.SetActive(true);
    }

    private void AnimateProgressTo(float target)
    {
        _progressTween?.Kill();

        float distance = Mathf.Abs(target - _progressSlider.value);
        float duration = _progressAnimationTime * distance;

        if (duration <= 0f)
        {
            _progressSlider.value = target;
            return;
        }

        _progressTween = _progressSlider
            .DOValue(target, duration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);
    }

    private async UniTask CompleteProgress(CancellationToken ct)
    {
        _progressTween?.Kill();

        float distance = 1f - _progressSlider.value;
        float duration = _progressAnimationTime * distance;

        if (duration <= 0f)
        {
            _progressSlider.value = 1f;
            return;
        }

        Tween currentTween = _progressSlider
            .DOValue(1f, duration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);

        _progressTween = currentTween;

        try
        {
            await currentTween.ToUniTask(cancellationToken: ct);
        }
        finally
        {
            if (_progressTween == currentTween)
                _progressTween = null;
        }

        _targetProgress = 1f;
        _progressSlider.value = 1f;
    }

    private async UniTask WaitForMinimumShowTime(CancellationToken ct)
    {
        double elapsed =
            Time.realtimeSinceStartupAsDouble - _shownAt;

        double remaining =
            _minimumVisibleTime - elapsed;

        if (remaining <= 0)
            return;

        await UniTask.Delay(
            TimeSpan.FromSeconds(remaining),
            ignoreTimeScale: true,
            cancellationToken: ct);
    }

    private async UniTask FadeTo(
        float targetAlpha,
        float duration,
        CancellationToken ct)
    {
        _fadeTween?.Kill();

        if (duration <= 0f)
        {
            _canvasGroup.alpha = targetAlpha;
            return;
        }

        Tween currentTween = _canvasGroup
            .DOFade(targetAlpha, duration)
            .SetUpdate(true);

        _fadeTween = currentTween;

        try
        {
            await currentTween.ToUniTask(cancellationToken: ct);
        }
        finally
        {
            if (_fadeTween == currentTween)
                _fadeTween = null;
        }
    }

    private void KillAnimations()
    {
        _fadeTween?.Kill();
        _progressTween?.Kill();

        _fadeTween = null;
        _progressTween = null;
    }

    private void OnDestroy()
    {
        KillAnimations();
    }
}