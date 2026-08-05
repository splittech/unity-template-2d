using System;
using UnityEngine;

namespace Game.Core
{
    public class SoundInstance
    {
        private readonly AudioSource _audioSource;
        private readonly SoundPool _pool;

        private bool _isValid;
        private bool _isPaused;

        public bool IsValid => _isValid;
        public bool IsPaused => _isPaused;

        public event Action OnFinishedPlaying;

        public SoundInstance(AudioSource audioSource, SoundPool pool)
        {
            _audioSource = audioSource;
            _pool = pool;
        }

        public void Play()
        {
            CheckIsValid();
            _audioSource.Play();
            _isPaused = false;
        }

        public void Stop()
        {
            CheckIsValid();
            _audioSource.Stop();
            _isPaused = true;
        }

        public void Pause()
        {
            CheckIsValid();
            _audioSource.Pause();
            _isPaused = true;
        }

        public void Unpause()
        {
            CheckIsValid();
            _audioSource.UnPause();
            _isPaused = false;
        }

        public void Release()
        {
            if (!_isValid)
                return;

            _audioSource.Stop();
            _audioSource.gameObject.SetActive(false);

            _pool.Release(this);

            _isValid = false;
        }

        public void ApplySettings(SoundMetaAsset soundMetaAsset)
        {
            _audioSource.clip = soundMetaAsset.AudioClip;
            _audioSource.volume = soundMetaAsset.Volume;
            _audioSource.loop = soundMetaAsset.IsLooping;
            _audioSource.maxDistance = soundMetaAsset.MaxHearDistance;

            SetSpatial(soundMetaAsset.IsSpatial);
        }

        public void SetPosition(Vector3 position)
        {
            _audioSource.transform.position = position;
        }

        public void SetSpatial(bool spatial)
        {
            _audioSource.spatialBlend = spatial ? 1f : 0f;
        }

        public AudioSource GetAudioSource()
        {
            CheckIsValid();
            return _audioSource;
        }

        private void CheckIsValid()
        {
            if (_isValid)
                throw new InvalidOperationException($"SoundInstance was not valid. Check IsValid before calling a method.");
        }
    }
}
