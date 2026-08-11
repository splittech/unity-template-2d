using System;
using System.Collections.Generic;
using Game.Core.Tests.Doubles;
using NUnit.Framework;
using UnityEngine;

namespace Game.Core.Tests.Editor
{
    public sealed class SoundInstanceTests
    {
        private readonly List<UnityEngine.Object> createdObjects = new();

        private SoundPool pool;
        private AudioSource source;

        [SetUp]
        public void SetUp()
        {
            pool = AudioServiceTestFactory.CreateSoundPool(
                1,
                createdObjects,
                out IReadOnlyList<AudioSource> sources);
            source = sources[0];
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = createdObjects.Count - 1; i >= 0; i--)
            {
                if (createdObjects[i] != null)
                    UnityEngine.Object.DestroyImmediate(createdObjects[i]);
            }

            createdObjects.Clear();
        }

        [Test]
        public void Play_FromPending_SetsPlayingState()
        {
            SoundInstance sound = pool.Get();

            sound.Play();

            Assert.That(sound.State, Is.EqualTo(SoundState.Playing));
        }

        [TestCase(SoundState.Playing)]
        [TestCase(SoundState.Paused)]
        public void Stop_FromPlayingOrPaused_SetsStoppedState(SoundState initialState)
        {
            SoundInstance sound = pool.Get();
            sound.Play();

            if (initialState == SoundState.Paused)
                sound.Pause();

            sound.Stop();

            Assert.That(sound.State, Is.EqualTo(SoundState.Stopped));
        }

        [Test]
        public void Release_SetsReleasedState()
        {
            SoundInstance sound = pool.Get();

            sound.Release();

            Assert.That(sound.State, Is.EqualTo(SoundState.Released));
            Assert.That(source.gameObject.activeSelf, Is.False);
        }

        [Test]
        public void Methods_AfterRelease_ThrowInvalidOperationException()
        {
            SoundInstance sound = pool.Get();
            sound.Release();

            Assert.Throws<InvalidOperationException>(() => sound.Play());
            Assert.Throws<InvalidOperationException>(() => sound.Stop());
            Assert.Throws<InvalidOperationException>(() => sound.Pause());
            Assert.Throws<InvalidOperationException>(() => sound.Unpause());
            Assert.Throws<InvalidOperationException>(() => sound.SetPosition(Vector3.one));
            Assert.Throws<InvalidOperationException>(() => sound.SetSpatial(true));
            Assert.Throws<InvalidOperationException>(() => sound.GetAudioSource());
        }

        [Test]
        public void ApplySettingsFromAsset_AppliesAllSettings()
        {
            AudioClip clip = AudioServiceTestFactory.CreateClip("Settings Clip");
            SoundMetaAsset asset = AudioServiceTestFactory.CreateSoundAsset(
                clip,
                volume: 0.35f,
                looping: true,
                maxDistance: 42f);
            createdObjects.Add(asset);
            createdObjects.Add(clip);
            SoundInstance sound = pool.Get();

            sound.ApplySettingsFromAsset(asset);

            Assert.That(source.clip, Is.SameAs(clip));
            Assert.That(source.volume, Is.EqualTo(0.35f));
            Assert.That(source.loop, Is.True);
            Assert.That(source.maxDistance, Is.EqualTo(42f));
        }
    }
}
