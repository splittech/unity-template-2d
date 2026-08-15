using Cysharp.Threading.Tasks;
using Game.Core;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.MainMenu
{
    public class ButtonAudio : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private SoundMetaAsset _hoverSound;
        [SerializeField] private SoundMetaAsset _clickSound;

        private AudioService _audioService;

        [Inject]
        public void Construct(AudioService audioService)
        {
            _audioService = audioService;

            _button.OnPointerEnterAsObservable()
                .Subscribe(_ => OnButtonHovered())
                .AddTo(this);

            _button.OnClickAsObservable()
                .Subscribe(_ => OnButtonClicked())
                .AddTo(this);
        }

        private void OnButtonHovered()
        {
            _audioService.PlaySound(_hoverSound);
        }

        public void OnButtonClicked()
        {
            _audioService.PlaySound(_clickSound);
        }
    }
}
