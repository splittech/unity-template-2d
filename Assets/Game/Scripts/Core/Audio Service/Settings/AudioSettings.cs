using UnityEngine.Audio;

namespace Game.Core
{
    public class AudioSettings
    {
        private readonly AudioServiceConfig _config;

        public AudioSettings(AudioServiceConfig config)
        {
            _config = config;
        }

        public void ApplyAllMixerGroupVolumes()
        {
            foreach (var mixerGroupSettings in _config.MixerGroupsSettings.Values)
            {
                MixerGroupSettings.Validate(mixerGroupSettings, _config);
                mixerGroupSettings.ApplyVolume(_config.Mixer);
            }
        }

        public void SetMixerGroupVolume(AudioMixerGroup mixerGroup, float volume)
        {
            var mixerGroupSettings = MixerGroupSettings.GetFrom(mixerGroup, _config);
            mixerGroupSettings.SetVolume(_config.Mixer, volume);
        }

        public float GetMixerGroupVolume(AudioMixerGroup mixerGroup)
        {
            var mixerGroupSettings = MixerGroupSettings.GetFrom(mixerGroup, _config);
            return mixerGroupSettings.Volume;
        }
    }
}
