using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Core
{
    public class SoundPool : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _audioMixerGroup;
        [SerializeField] private List<AudioSource> _audioSources;

        private Dictionary<AudioSource, SoundInstance> _sounds = new();
        private LinkedList<SoundInstance> _soundQueue = new();

        private void Awake()
        {
            foreach (var source in _audioSources)
                _sounds[source] = null;
        }

        private void Update()
        {
            foreach (var source in _audioSources)
            {
                SoundInstance sound = _sounds[source];

                if (sound == null)
                    continue;

                // Sound finished playing.
                if (!sound.IsPaused && !source.isPlaying)
                    sound.Release();
            }
        }

        public SoundInstance Get()
        {
            AudioSource freeAudioSource = _audioSources
                .FirstOrDefault(source => _sounds[source] == null);

            if (freeAudioSource == null)
            {
                SoundInstance oldestSound = _soundQueue.First.Value;
                oldestSound.Release();

                freeAudioSource = oldestSound.GetAudioSource();
            }

            return CreateSound(freeAudioSource);
        }

        public void Release(SoundInstance sound)
        {
            _soundQueue.Remove(sound);
            _sounds[sound.GetAudioSource()] = null;
        }

        private SoundInstance CreateSound(AudioSource source)
        {
            SoundInstance sound = new(source, this);

            _soundQueue.AddLast(sound);
            _sounds[source] = sound;

            return sound;
        }

        [BoxGroup("Create Audio Sources")]
        [Button]
        private void CreateAudioSources(int number)
        {
            _audioSources.ForEach(source =>
            {
                if (source != null)
                    DestroyImmediate(source.gameObject);
            });

            _audioSources.Clear();

            for (int i = 0; i < number; i++)
            {
                GameObject audioSourceObject = new($"{name}'s Audio Source ({i + 1})");
                audioSourceObject.transform.SetParent(transform);

                AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = _audioMixerGroup;

                _audioSources.Add(audioSource);
            }
        }
    }
}
