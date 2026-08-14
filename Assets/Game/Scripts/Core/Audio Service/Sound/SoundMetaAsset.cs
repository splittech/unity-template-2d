using System;
using Alchemy.Inspector;
using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Sound Meta Asset", menuName = "Game/Meta Assets/Audio Service/Sound Meta Asset", order = 0)]
    public class SoundMetaAsset : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private AudioClip _audioClip;
        [Range(0f, 1f)]
        [SerializeField] private float _volume = 1f;
        [SerializeField] private bool _isLooping;
        [SerializeField] private bool _randomPitch;
        [ShowIf("_randomPitch")]
        [Range(0f, 1f)]
        [SerializeField] private float _randomPitchRange = 0.1f;

        [Header("Spatial")]
        [SerializeField] private float _maxHearDistance = 500f;

        public AudioClip AudioClip => _audioClip;
        public float Volume => _volume;
        public bool IsLooping => _isLooping;
        public bool RandomPitch => _randomPitch;
        public float RandomPitchRange => _randomPitchRange;
        public float MaxHearDistance => _maxHearDistance;
    }
}
