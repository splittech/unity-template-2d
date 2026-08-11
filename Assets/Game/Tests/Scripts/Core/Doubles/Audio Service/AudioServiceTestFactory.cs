using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Core.Tests.Doubles
{
    public static class AudioServiceTestFactory
    {
        private const BindingFlags InstanceField =
            BindingFlags.Instance | BindingFlags.NonPublic;

        public static SoundPool CreateSoundPool(
            int size,
            ICollection<UnityEngine.Object> createdObjects,
            out IReadOnlyList<AudioSource> sources)
        {
            GameObject poolObject = new("Test Sound Pool");
            poolObject.SetActive(false);
            createdObjects.Add(poolObject);

            SoundPool pool = poolObject.AddComponent<SoundPool>();
            List<AudioSource> mutableSources = new(size);

            for (int i = 0; i < size; i++)
            {
                GameObject sourceObject = new($"Test Audio Source {i + 1}");
                sourceObject.transform.SetParent(poolObject.transform);
                sourceObject.SetActive(false);
                mutableSources.Add(sourceObject.AddComponent<AudioSource>());
            }

            SetField(pool, "_audioSources", mutableSources);
            poolObject.SetActive(true);

            if (!Application.isPlaying)
                Invoke(pool, "Awake");

            sources = mutableSources;
            return pool;
        }

        public static SoundMetaAsset CreateSoundAsset(
            AudioClip clip,
            float volume = 1f,
            bool looping = false,
            float maxDistance = 500f)
        {
            SoundMetaAsset asset = ScriptableObject.CreateInstance<SoundMetaAsset>();
            SetField(asset, "_audioClip", clip);
            SetField(asset, "_volume", volume);
            SetField(asset, "_isLooping", looping);
            SetField(asset, "_maxHearDistance", maxDistance);
            return asset;
        }

        public static MusicMetaAsset CreateMusicAsset(AudioClip clip, float volume = 1f)
        {
            MusicMetaAsset asset = ScriptableObject.CreateInstance<MusicMetaAsset>();
            SetField(asset, "_audioClip", clip);
            SetField(asset, "_volume", volume);
            return asset;
        }

        public static AudioClip CreateClip(string name, float durationSeconds = 0.25f)
        {
            const int frequency = 44100;
            int sampleCount = Mathf.Max(1, Mathf.CeilToInt(durationSeconds * frequency));
            AudioClip clip = AudioClip.Create(name, sampleCount, 1, frequency, false);
            clip.SetData(new float[sampleCount], 0);
            return clip;
        }

        public static void SetField<T>(object target, string fieldName, T value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, InstanceField);

            if (field == null)
                throw new MissingFieldException(target.GetType().FullName, fieldName);

            field.SetValue(target, value);
        }

        public static void Invoke(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, InstanceField);

            if (method == null)
                throw new MissingMethodException(target.GetType().FullName, methodName);

            method.Invoke(target, null);
        }
    }
}
