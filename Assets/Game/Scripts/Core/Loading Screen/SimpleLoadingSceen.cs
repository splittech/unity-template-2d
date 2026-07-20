using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleLoadingScreen : LoadingScreen
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text progressDescription;
    [SerializeField] private Slider progressSlider;

    [Header("Parameters")]
    [SerializeField] private float minimumShowTime = 1f;
    [SerializeField] private float fadeInTime = 0.3f;
    [SerializeField] private float fadeOutTime = 0.3f;

    private Tween fadeTween;

    private float showTime;

    private bool isFullyShown;

    void Update()
    {
        if (isFullyShown)
            showTime += Time.deltaTime;
    }

    public override async UniTask Show(CancellationToken ct = default)
    {
        fadeTween?.Kill();

        canvasGroup.gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        isFullyShown = false;

        Tween currentTween = canvasGroup.DOFade(1f, fadeInTime).SetUpdate(true);
        fadeTween = currentTween;

        try
        {
            await currentTween.ToUniTask(cancellationToken: ct);
            canvasGroup.interactable = true;
            isFullyShown = true;
        }
        finally
        {
            if (fadeTween == currentTween)
                fadeTween = null;
        }
    }

    public override async UniTask Hide(CancellationToken ct = default)
    {
        fadeTween?.Kill();

        canvasGroup.interactable = false;
        isFullyShown = false;

        Tween currentTween = canvasGroup.DOFade(0f, fadeOutTime).SetUpdate(true);
        fadeTween = currentTween;

        try
        {
            await currentTween.ToUniTask(cancellationToken: ct);

            if (showTime < minimumShowTime)
                await UniTask.Yield(ct);

            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(false);
        }
        finally
        {
            if (fadeTween == currentTween)
                fadeTween = null;

            showTime = 0f;
        }
    }

    public override void ShowImmediate()
    {
        fadeTween?.Kill();
        fadeTween = null;

        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        isFullyShown = true;
    }

    public override void HideImmediate()
    {
        fadeTween?.Kill();
        fadeTween = null;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.gameObject.SetActive(false);

        isFullyShown = false;
        showTime = 0f;
    }

    public override void SetProgressDescription(string description)
    {
        progressDescription.text = description;
    }

    protected override void SetProgress(float progress)
    {
        progressSlider.value = progress;
    }
}