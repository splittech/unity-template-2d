using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.Exceptions;

namespace Game.Core
{
    [Serializable]
    public class MixerGroupSettings
    {
        [SerializeField] private string _volumeExposedName;
        [Range(0f, 1f)][SerializeField] private float _volume;

        public float Volume => _volume;

        public void SetVolume(AudioMixer mixer, float volume)
        {
            _volume = Mathf.Clamp01(volume);
            ApplyVolume(mixer);
        }

        public void ApplyVolume(AudioMixer mixer)
        {
            float volumeInHz = Mathf.Log10(Mathf.Clamp(_volume, 0.0001f, 1f)) * 20f;
            bool success = mixer.SetFloat(_volumeExposedName, volumeInHz);

            if (!success)
            {
                throw new OperationException(
                    "Setting mixer group volume was not successful. There may be invalid exposed name in AudioServiceConfig.");
            }
        }

        public static void Validate(MixerGroupSettings mixerGroupSettings, AudioServiceConfig config)
        {
            if (mixerGroupSettings == null)
            {
                throw new InvalidOperationException(
                    $"AudioServiceConfig '{config.name}': MixerGroupSettings is null.");
            }

            if (string.IsNullOrEmpty(mixerGroupSettings._volumeExposedName))
            {
                throw new ArgumentException(
                    "AudioServiceConfig '{config.name}': MixerGroup volume exposed name was null or empty.");
            }
        }

        public static MixerGroupSettings GetFrom(AudioMixerGroup mixerGroup, AudioServiceConfig config)
        {
            if (mixerGroup == null)
                throw new ArgumentNullException("MixerGroup cannot be null.");

            if (!config.MixerGroupsSettings.TryGetValue(mixerGroup, out var mixerGroupSettings))
                throw new ArgumentException($"AudioServiceConfig '{config.name}': MixerGroup is not registered.");

            return mixerGroupSettings;
        }
    }
}
