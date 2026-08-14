using Game.Core;
using R3;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using VContainer;

namespace Game.MainMenu
{
    [RequireComponent(typeof(Slider))]
    public class MixerGroupVolumeSlider : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _audioMixerGroup;

        private AudioService _audioService;
        private Slider _slider;

        [Inject]
        public void Construct(AudioService audioService)
        {
            _audioService = audioService;

            _slider = GetComponent<Slider>();

            _slider.value = _audioService.GetMixerGroupVolume(_audioMixerGroup);

            _slider.OnValueChangedAsObservable()
                .Subscribe(OnSliderValueChanged)
                .AddTo(this);
        }

        private void OnSliderValueChanged(float value)
        {
            _audioService.SetMixerGroupVolume(_audioMixerGroup, value);
        }
    }
}
