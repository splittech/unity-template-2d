using UnityEngine;

namespace Game.Core
{
    public class AudioService
    {
        private readonly MusicPlayer _musicPlayer;
        private readonly SoundPlayer _soundPlayer;

        public AudioService(MusicPlayer musicPlayer, SoundPlayer soundPlayer, AudioServiceConfig config)
        {
            _musicPlayer = musicPlayer;
            _soundPlayer = soundPlayer;
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
    }
}
