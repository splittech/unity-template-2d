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
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text progressDescription;
    [SerializeField] private Slider progressSlider;

    [Header("Parameters")]
    [SerializeField] private float minimumVisibleTime = 1f;
    [SerializeField] private float progressAnimationTime = 0.3f;
    [SerializeField] private float fadeInTime = 0.3f;
    [SerializeField] private float fadeOutTime = 0.3f;

    private Tween fadeTween;
    private Tween progressTween;
    private double shownAt;
    private float targetProgress;

    public override async UniTask Show(CancellationToken ct = default)
    {
        PrepareToShow();

        await FadeTo(
            targetAlpha: 1f,
            duration: fadeInTime,
            ct);
    }

    public override void ShowImmediate()
    {
        PrepareToShow();
        canvasGroup.alpha = 1f;
    }

    public override async UniTask Hide(CancellationToken ct = default)
    {
        await WaitForMinimumShowTime(ct);
        await CompleteProgress(ct);

        canvasGroup.blocksRaycasts = false;

        await FadeTo(
            targetAlpha: 0f,
            duration: fadeOutTime,
            ct);

        canvasGroup.gameObject.SetActive(false);
    }

    public override void HideImmediate()
    {
        KillAnimations();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.gameObject.SetActive(false);

        targetProgress = 0f;
        progressSlider.value = 0f;
    }

    public override void SetProgressDescription(string description)
    {
        progressDescription.text = description;
    }

    protected override void UpdateProgress(float progress)
    {
        float normalizedProgress = Mathf.Clamp01(progress);

        targetProgress = Mathf.Max(targetProgress, normalizedProgress);

        AnimateProgressTo(targetProgress);
    }

    private void PrepareToShow()
    {
        KillAnimations();

        shownAt = Time.realtimeSinceStartupAsDouble;
        targetProgress = 0f;

        progressSlider.value = 0f;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.gameObject.SetActive(true);
    }

    private void AnimateProgressTo(float target)
    {
        progressTween?.Kill();

        float distance = Mathf.Abs(target - progressSlider.value);
        float duration = progressAnimationTime * distance;

        if (duration <= 0f)
        {
            progressSlider.value = target;
            return;
        }

        progressTween = progressSlider
            .DOValue(target, duration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);
    }

    private async UniTask CompleteProgress(CancellationToken ct)
    {
        progressTween?.Kill();

        float distance = 1f - progressSlider.value;
        float duration = progressAnimationTime * distance;

        if (duration <= 0f)
        {
            progressSlider.value = 1f;
            return;
        }

        Tween currentTween = progressSlider
            .DOValue(1f, duration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);

        progressTween = currentTween;

        try
        {
            await currentTween.ToUniTask(cancellationToken: ct);
        }
        finally
        {
            if (progressTween == currentTween)
                progressTween = null;
        }

        targetProgress = 1f;
        progressSlider.value = 1f;
    }

    private async UniTask WaitForMinimumShowTime(CancellationToken ct)
    {
        double elapsed =
            Time.realtimeSinceStartupAsDouble - shownAt;

        double remaining =
            minimumVisibleTime - elapsed;

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
        fadeTween?.Kill();

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            return;
        }

        Tween currentTween = canvasGroup
            .DOFade(targetAlpha, duration)
            .SetUpdate(true);

        fadeTween = currentTween;

        try
        {
            await currentTween.ToUniTask(cancellationToken: ct);
        }
        finally
        {
            if (fadeTween == currentTween)
                fadeTween = null;
        }
    }

    private void KillAnimations()
    {
        fadeTween?.Kill();
        progressTween?.Kill();

        fadeTween = null;
        progressTween = null;
    }

    private void OnDestroy()
    {
        KillAnimations();
    }
}