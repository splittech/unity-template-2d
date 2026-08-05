using DG.Tweening;
using UnityEngine;
using VContainer;

namespace Game.Core
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _firstAudioSource;
        [SerializeField] private AudioSource _secondAudioSource;

        private AudioServiceConfig _config;

        private AudioSource _currentAudioSource;
        private AudioSource _previousAudioSource;
        private Tween _fadeInTween;
        private Tween _fadeOutTween;

        [Inject]
        public void Construct(AudioServiceConfig config)
        {
            _config = config;
        }

        public void PlayMusic(MusicMetaAsset musicMetaAsset)
        {
            if (_currentAudioSource == null || _currentAudioSource == _secondAudioSource)
            {
                SwitchCurrentAudioSource(_firstAudioSource);
            }
            else
            {
                SwitchCurrentAudioSource(_secondAudioSource);
            }

            EnableAudioSource(_currentAudioSource, musicMetaAsset);
            FadeAudioSources(musicMetaAsset);
        }

        public void StopMusic()
        {
            SwitchCurrentAudioSource(null);
            FadeAudioSources(null);
        }

        private void SwitchCurrentAudioSource(AudioSource audioSource)
        {
            _previousAudioSource = _currentAudioSource;
            _currentAudioSource = audioSource;
        }

        public void FadeAudioSources(MusicMetaAsset musicMetaAsset)
        {
            _fadeInTween?.Kill();
            _fadeOutTween?.Kill();

            if (_currentAudioSource != null)
                _fadeInTween = _currentAudioSource.DOFade(musicMetaAsset.Volume, _config.MusicFadeTime);

            if (_previousAudioSource != null)
            {
                AudioSource audioSourceToDisable = _previousAudioSource;

                _fadeOutTween = _previousAudioSource
                    .DOFade(0f, _config.MusicFadeTime)
                    .OnComplete(() => DisableAudioSource(audioSourceToDisable))
                    .OnKill(() => DisableAudioSource(audioSourceToDisable));
            }
        }

        private void EnableAudioSource(AudioSource audioSource, MusicMetaAsset musicMetaAsset)
        {
            audioSource.clip = musicMetaAsset.AudioClip;
            audioSource.volume = 0;
            audioSource.gameObject.SetActive(true);
        }

        private void DisableAudioSource(AudioSource audioSource)
        {
            if (audioSource == null)
                return;

            audioSource.gameObject.SetActive(false);
        }
    }
}
