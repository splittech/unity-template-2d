using UnityEngine;
using UnityEngine.Audio;

namespace Game.Core
{
    public class AudioService
    {
        private readonly MusicPlayer _musicPlayer;
        private readonly SoundPlayer _soundPlayer;
        private readonly AudioSettings _soundSettings;

        public AudioService(MusicPlayer musicPlayer, SoundPlayer soundPlayer, AudioSettings soundSettings)
        {
            _musicPlayer = musicPlayer;
            _soundPlayer = soundPlayer;
            _soundSettings = soundSettings;
        }

        public void PlayMusic(MusicMetaAsset musicMetaAsset)
        {
            _musicPlayer.PlayMusic(musicMetaAsset);
        }

        public void StopMusic()
        {
            _musicPlayer.StopMusic();
        }

        public SoundInstance PlaySound(SoundMetaAsset soundMetaAsset, bool autoStart = true)
        {
            return _soundPlayer.PlaySound(soundMetaAsset, autoStart);
        }

        public SoundInstance PlaySpatialSound(SoundMetaAsset soundMetaAsset, Vector3 position, bool autoStart = true)
        {
            return _soundPlayer.PlaySpatialSound(soundMetaAsset, position, autoStart);
        }

        public void ApplyAllMixerGroupVolumes()
        {
            _soundSettings.ApplyAllMixerGroupVolumes();
        }

        public void SetMixerGroupVolume(AudioMixerGroup mixerGroup, float volume)
        {
            _soundSettings.SetMixerGroupVolume(mixerGroup, volume);
        }

        public float GetMixerGroupVolume(AudioMixerGroup mixerGroup)
        {
            return _soundSettings.GetMixerGroupVolume(mixerGroup);
        }
    }
}
