using UnityEngine;
using UnityEngine.Audio;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Audio Service Config", menuName = "Game/Configs/Audio Service/Audio Service Config", order = 0)]
    public class AudioServiceConfig : ScriptableObject
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private string _masterVolumeExposedName;
        [SerializeField] private string _musicVolumeExposedName;
        [SerializeField] private string _soundVolumeExposedName;

        [Header("Music")]
        [SerializeField] private float _musicFadeTime = 0.3f;

        public float MusicFadeTime => _musicFadeTime;
    }
}
