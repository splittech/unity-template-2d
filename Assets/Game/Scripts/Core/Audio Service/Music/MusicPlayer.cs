using Alchemy.Inspector;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace Game.Core
{
    public class MusicPlayer : MonoBehaviour
    {
        [Header("Mixer Group")]
        [SerializeField] private AudioMixerGroup _audioMixerGroup;

        [Header("Audio Sources")]
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

            FadeAudioSources(musicMetaAsset);
            EnableAudioSource(_currentAudioSource, musicMetaAsset);
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
            {
                _fadeInTween = _currentAudioSource
                    .DOFade(musicMetaAsset.Volume, _config.MusicFadeTime);
            }

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
            audioSource.Play();
        }

        private void DisableAudioSource(AudioSource audioSource)
        {
            if (audioSource == null)
                return;

            audioSource.Stop();
            audioSource.gameObject.SetActive(false);
        }

        [BoxGroup("Recreate Audio Sources")]
        [Button]
        private void RecreateAudioSources()
        {
            if (_firstAudioSource != null)
                DestroyImmediate(_firstAudioSource.gameObject);

            if (_secondAudioSource != null)
                DestroyImmediate(_secondAudioSource.gameObject);

            _firstAudioSource = CreateAudioSource(1);
            _secondAudioSource = CreateAudioSource(2);
        }

        private AudioSource CreateAudioSource(int number)
        {
            GameObject audioSourceObject = new($"{name}'s Audio Source ({number})");
            audioSourceObject.transform.SetParent(transform);
            audioSourceObject.SetActive(false);

            AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = _audioMixerGroup;
            audioSource.playOnAwake = false;
            audioSource.loop = true;

            return audioSource;
        }
    }
}
