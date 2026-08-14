using System;
using System.Collections.Generic;
using Alchemy.Serialization;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Core
{
    [AlchemySerialize]
    [CreateAssetMenu(fileName = "Audio Service Config", menuName = "Game/Configs/Audio Service/Audio Service Config", order = 0)]
    public partial class AudioServiceConfig : ScriptableObject
    {
        [Header("Debug")]
        [SerializeField] private bool _enableLogger;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer _mixer;

        [AlchemySerializeField, NonSerialized]
        private Dictionary<AudioMixerGroup, MixerGroupSettings> _mixerGroupsSettings;

        [Header("Music")]
        [SerializeField] private float _musicFadeTime = 0.3f;

        public AudioMixer Mixer => _mixer;
        public Dictionary<AudioMixerGroup, MixerGroupSettings> MixerGroupsSettings => _mixerGroupsSettings;
        public float MusicFadeTime => _musicFadeTime;
        public bool EnableLogger => _enableLogger;
    }
}
