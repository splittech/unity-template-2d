using Game.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.MainMenu
{
    public class PlayMusicButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private MusicMetaAsset _music;

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
            _audioService.PlayMusic(_music);
        }
    }
}