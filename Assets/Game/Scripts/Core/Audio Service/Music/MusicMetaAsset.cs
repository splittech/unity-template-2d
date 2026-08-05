using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Music Meta Asset", menuName = "Game/Meta Assets/Audio Service/Music Meta Asset", order = 0)]
    public class MusicMetaAsset : ScriptableObject
    {
        [SerializeField] private AudioClip _audioClip;
        [Range(0f, 1f)][SerializeField] private float _volume;

        public AudioClip AudioClip => _audioClip;
        public float Volume => _volume;
    }
}
