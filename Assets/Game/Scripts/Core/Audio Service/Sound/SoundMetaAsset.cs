using System;
using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Sound Meta Asset", menuName = "Game/Meta Assets/Audio Service/Sound Meta Asset", order = 0)]
    public class SoundMetaAsset : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private AudioClip _audioClip;
        [Range(0f, 1f)][SerializeField] private float _volume = 1f;
        [SerializeField] private bool _isLooping;

        [Header("Spatial")]
        [SerializeField] private float _maxHearDistance = 500f;

        public AudioClip AudioClip => _audioClip;
        public float Volume => _volume;
        public bool IsLooping => _isLooping;
        public float MaxHearDistance => _maxHearDistance;
    }
}
