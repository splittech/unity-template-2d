using System;
using System.Collections;
using System.Collections.Generic;
using Game.Core.Tests.Doubles;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Core.Tests.PlayMode
{
    public sealed class SoundServicePlayModeTests
    {
        private readonly List<UnityEngine.Object> createdObjects = new();

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = createdObjects.Count - 1; i >= 0; i--)
            {
                if (createdObjects[i] != null)
                    UnityEngine.Object.Destroy(createdObjects[i]);
            }

            createdObjects.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator PauseAndUnpause_ContinuesPlayback()
        {
            SoundPool pool = CreatePool(1, out IReadOnlyList<AudioSource> sources);
            AudioSource source = sources[0];
            SoundInstance sound = pool.Get();
            AudioClip clip = CreateClip("Pause Clip", 1f);
            source.clip = clip;

            sound.Play();
            yield return new WaitForSecondsRealtime(0.1f);
            sound.Pause();
            float pausedTime = source.time;

            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(source.time, Is.EqualTo(pausedTime).Within(0.03f));

            sound.Unpause();
            yield return new WaitForSecondsRealtime(0.1f);

            Assert.That(sound.State, Is.EqualTo(SoundState.Playing));
            Assert.That(source.time, Is.GreaterThan(pausedTime + 0.03f));
        }

        [UnityTest]
        public IEnumerator PlaySound_AutoStartFalse_RemainsPending()
        {
            SoundPool pool = CreatePool(1, out _);
            SoundPlayer player = CreateSoundPlayer(pool);
            SoundMetaAsset asset = CreateSoundAsset("Pending Clip");

            SoundInstance sound = player.PlaySound(asset, autoStart: false);
            yield return null;

            Assert.That(sound.State, Is.EqualTo(SoundState.Pending));
        }

        [UnityTest]
        public IEnumerator PlaySpatialSound_AppliesSpatialSettingsAndPosition()
        {
            SoundPool pool = CreatePool(1, out IReadOnlyList<AudioSource> sources);
            SoundPlayer player = CreateSoundPlayer(pool);
            SoundMetaAsset asset = CreateSoundAsset("Spatial Clip");
            Vector3 position = new(3f, 4f, 5f);

            SoundInstance sound = player.PlaySpatialSound(asset, position, autoStart: false);
            yield return null;

            Assert.That(sound.State, Is.EqualTo(SoundState.Pending));
            Assert.That(sources[0].spatialBlend, Is.EqualTo(1f));
            Assert.That(sources[0].transform.position, Is.EqualTo(position));
        }

        [UnityTest]
        public IEnumerator Pool_ReleaseAllowsSourceReuse()
        {
            SoundPool pool = CreatePool(1, out IReadOnlyList<AudioSource> sources);
            SoundInstance first = pool.Get();
            AudioSource originalSource = sources[0];
            first.Release();

            SoundInstance second = pool.Get();
            yield return null;

            Assert.That(first.State, Is.EqualTo(SoundState.Released));
            Assert.That(second.GetAudioSource(), Is.SameAs(originalSource));
        }

        [UnityTest]
        public IEnumerator ReleasedInstance_CannotControlReusedSource()
        {
            SoundPool pool = CreatePool(1, out IReadOnlyList<AudioSource> sources);
            SoundInstance first = pool.Get();
            first.Release();
            SoundInstance second = pool.Get();
            second.SetPosition(Vector3.one);

            Assert.Throws<InvalidOperationException>(() => first.SetPosition(Vector3.zero));
            yield return null;

            Assert.That(sources[0].transform.position, Is.EqualTo(Vector3.one));
        }

        [UnityTest]
        public IEnumerator PoolExhaustion_EvictsOldestInstance()
        {
            SoundPool pool = CreatePool(2, out _);
            SoundInstance oldest = pool.Get();
            SoundInstance newer = pool.Get();

            SoundInstance replacement = pool.Get();
            yield return null;

            Assert.That(oldest.State, Is.EqualTo(SoundState.Released));
            Assert.That(newer.State, Is.EqualTo(SoundState.Pending));
            Assert.That(replacement.State, Is.EqualTo(SoundState.Pending));
        }

        [UnityTest]
        public IEnumerator FinishedNonLoopingSound_IsAutomaticallyReleased()
        {
            SoundPool pool = CreatePool(1, out IReadOnlyList<AudioSource> sources);
            SoundInstance sound = pool.Get();
            sources[0].clip = CreateClip("Short Clip", 0.05f);
            sound.Play();

            yield return new WaitForSecondsRealtime(0.2f);
            yield return null;

            Assert.That(sound.State, Is.EqualTo(SoundState.Released));
        }

        [UnityTest]
        public IEnumerator PendingAndPausedSounds_AreNotAutomaticallyReleased()
        {
            SoundPool pool = CreatePool(2, out IReadOnlyList<AudioSource> sources);
            SoundInstance pending = pool.Get();
            SoundInstance paused = pool.Get();
            sources[1].clip = CreateClip("Paused Clip", 1f);
            paused.Play();
            yield return null;
            paused.Pause();

            yield return new WaitForSecondsRealtime(0.1f);

            Assert.That(pending.State, Is.EqualTo(SoundState.Pending));
            Assert.That(paused.State, Is.EqualTo(SoundState.Paused));
        }

        private SoundPool CreatePool(int size, out IReadOnlyList<AudioSource> sources)
        {
            return AudioServiceTestFactory.CreateSoundPool(size, createdObjects, out sources);
        }

        private SoundPlayer CreateSoundPlayer(SoundPool pool)
        {
            GameObject playerObject = new("Test Sound Player");
            createdObjects.Add(playerObject);
            SoundPlayer player = playerObject.AddComponent<SoundPlayer>();
            AudioServiceTestFactory.SetField(player, "_soundPool", pool);
            return player;
        }

        private AudioClip CreateClip(string name, float duration = 0.25f)
        {
            AudioClip clip = AudioServiceTestFactory.CreateClip(name, duration);
            createdObjects.Add(clip);
            return clip;
        }

        private SoundMetaAsset CreateSoundAsset(string clipName)
        {
            SoundMetaAsset asset = AudioServiceTestFactory.CreateSoundAsset(CreateClip(clipName));
            createdObjects.Add(asset);
            return asset;
        }
    }
}
