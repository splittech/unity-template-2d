using DG.Tweening;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.MainMenu
{
    public class ButtonAnimation : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] private Button _button;

        [Header("Hover Animation")]
        [SerializeField] private float _hoverAnimationScale = 1.1f;
        [SerializeField] private float _hoverAnimationDuration = 0.3f;
        [SerializeField] private Ease _hoverAnimationEase;

        [Header("Click Down Animation")]
        [SerializeField] private float _clickAnimationScale = 1.2f;
        [SerializeField] private float _clickAnimationDuration = 0.3f;
        [SerializeField] private Ease _clickAnimationEase;

        private Tween _currentAnimation;

        private Vector3 _defaultScale;

        private void OnEnable()
        {
            transform.localScale = _defaultScale;
        }

        [Inject]
        public void Construct()
        {
            _defaultScale = transform.localScale;

            _button.OnPointerEnterAsObservable()
                .Subscribe(_ => OnButtonHovered())
                .AddTo(this);

            _button.OnPointerExitAsObservable()
                .Subscribe(_ => OnButtonUnhovered())
                .AddTo(this);

            _button.OnPointerDownAsObservable()
                .Subscribe(_ => OnButtonClickedDown())
                .AddTo(this);
        }

        private void OnButtonHovered()
        {
            _currentAnimation?.Kill();

            _currentAnimation = transform
                .DOScale(_defaultScale * _hoverAnimationScale, _hoverAnimationDuration)
                .SetEase(_hoverAnimationEase)
                .SetLink(gameObject);
        }

        public void OnButtonUnhovered()
        {
            _currentAnimation?.Kill();

            _currentAnimation = transform
                .DOScale(_defaultScale, _hoverAnimationDuration)
                .SetEase(_hoverAnimationEase)
                .SetLink(gameObject);
        }

        private void OnButtonClickedDown()
        {
            _currentAnimation?.Kill();

            _currentAnimation = transform
                .DOScale(_defaultScale * _clickAnimationScale, _clickAnimationDuration)
                .SetEase(_clickAnimationEase)
                .SetLink(gameObject);
        }
    }
}
