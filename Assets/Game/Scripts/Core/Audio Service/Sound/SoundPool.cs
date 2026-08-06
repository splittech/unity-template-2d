using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Core
{
    public class SoundPool : MonoBehaviour
    {
        [Header("Mixer Group")]
        [SerializeField] private AudioMixerGroup _audioMixerGroup;

        [Header("Audio Sources")]
        [SerializeField] private List<AudioSource> _audioSources;

        private Dictionary<AudioSource, SoundInstance> _sounds;
        private LinkedList<SoundInstance> _soundQueue;

        private void Awake()
        {
            _sounds = new Dictionary<AudioSource, SoundInstance>();
            _soundQueue = new LinkedList<SoundInstance>();

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
                freeAudioSource = oldestSound.GetAudioSource();
                oldestSound.Release();
            }

            return CreateSound(freeAudioSource);
        }

        public void Release(SoundInstance sound)
        {
            AudioSource source = sound.GetAudioSource();

            _soundQueue.Remove(sound);
            _sounds[source] = null;

            source.gameObject.SetActive(false);
        }

        private SoundInstance CreateSound(AudioSource source)
        {
            SoundInstance sound = new(source, this);

            _soundQueue.AddLast(sound);
            _sounds[source] = sound;

            source.gameObject.SetActive(true);

            return sound;
        }

        [BoxGroup("Recreate Audio Sources")]
        [Button]
        private void RecreateAudioSources(int number)
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
                audioSourceObject.SetActive(false);

                AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = _audioMixerGroup;
                audioSource.playOnAwake = false;

                _audioSources.Add(audioSource);
            }
        }
    }
}
