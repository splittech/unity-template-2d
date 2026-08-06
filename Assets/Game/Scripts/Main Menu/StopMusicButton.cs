using Game.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.MainMenu
{
    public class StopMusicButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private AudioService _audioService;

        [Inject]
        public void Construct(AudioService audioService)
        {
            _audioService = audioService;

            _button.OnClickAsObservable()
                .Subscribe(_ => OnButtonClicked())
                .AddTo(this);
        }

        private void OnButtonClicked()
        {
            _audioService.StopMusic();
        }
    }
}
