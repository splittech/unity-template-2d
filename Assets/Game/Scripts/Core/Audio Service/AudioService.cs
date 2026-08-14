using UnityEngine;
using UnityEngine.Audio;

namespace Game.Core
{
    public class AudioService
    {
        private readonly AudioServiceConfig _config;
        private readonly MusicPlayer _musicPlayer;
        private readonly SoundPlayer _soundPlayer;
        private readonly AudioSettings _soundSettings;

        public AudioService(AudioServiceConfig config, MusicPlayer musicPlayer, SoundPlayer soundPlayer, AudioSettings soundSettings)
        {
            _config = config;
            _musicPlayer = musicPlayer;
            _soundPlayer = soundPlayer;
            _soundSettings = soundSettings;
        }

        public void PlayMusic(MusicMetaAsset musicMetaAsset)
        {
            if (_config.EnableLogger)
                GameLogger.Log($"Play music with MusicMetaAsset '{musicMetaAsset.name}'.");

            _musicPlayer.PlayMusic(musicMetaAsset);
        }

        public void StopMusic()
        {
            if (_config.EnableLogger)
                GameLogger.Log("Stop music.");

            _musicPlayer.StopMusic();
        }

        public SoundInstance PlaySound(SoundMetaAsset soundMetaAsset, bool autoStart = true)
        {
            if (_config.EnableLogger)
                GameLogger.Log($"Play sound with SoundMetaAsset '{soundMetaAsset.name}'.");

            return _soundPlayer.PlaySound(soundMetaAsset, autoStart);
        }

        public SoundInstance PlaySpatialSound(SoundMetaAsset soundMetaAsset, Vector3 position, bool autoStart = true)
        {
            if (_config.EnableLogger)
                GameLogger.Log($"Play spatial sound with SoundMetaAsset '{soundMetaAsset.name}'.");

            return _soundPlayer.PlaySpatialSound(soundMetaAsset, position, autoStart);
        }

        public void ApplyAllMixerGroupVolumes()
        {
            if (_config.EnableLogger)
                GameLogger.Log("Apply all mixer group volumes.");

            _soundSettings.ApplyAllMixerGroupVolumes();
        }

        public void SetMixerGroupVolume(AudioMixerGroup mixerGroup, float volume)
        {
            if (_config.EnableLogger)
                GameLogger.Log($"Set volume of AudioMixerGroup '{mixerGroup.name}'.");

            _soundSettings.SetMixerGroupVolume(mixerGroup, volume);
        }

        public float GetMixerGroupVolume(AudioMixerGroup mixerGroup)
        {
            if (_config.EnableLogger)
                GameLogger.Log($"Get volume of AudioMixerGroup '{mixerGroup.name}'.");

            return _soundSettings.GetMixerGroupVolume(mixerGroup);
        }
    }
}
