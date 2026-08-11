using System.Collections;
using System.Collections.Generic;
using Game.Core.Tests.Doubles;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Core.Tests.PlayMode
{
    public sealed class MusicPlayerTests
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
        public IEnumerator MusicPlayer_CrossfadesAndDisablesPreviousSource()
        {
            const float fadeTime = 0.05f;
            MusicPlayer player = CreatePlayer(
                fadeTime,
                out AudioSource firstSource,
                out AudioSource secondSource);
            MusicMetaAsset firstMusic = CreateMusicAsset("First Music", 0.4f);
            MusicMetaAsset secondMusic = CreateMusicAsset("Second Music", 0.7f);

            player.PlayMusic(firstMusic);
            yield return new WaitForSecondsRealtime(fadeTime * 2f);

            player.PlayMusic(secondMusic);
            yield return new WaitForSecondsRealtime(fadeTime * 2f);

            Assert.That(firstSource.gameObject.activeSelf, Is.False);
            Assert.That(secondSource.gameObject.activeSelf, Is.True);
            Assert.That(secondSource.clip, Is.SameAs(secondMusic.AudioClip));
            Assert.That(secondSource.volume, Is.EqualTo(secondMusic.Volume).Within(0.02f));
            Assert.That(secondSource.isPlaying, Is.True);
        }

        private MusicPlayer CreatePlayer(
            float fadeTime,
            out AudioSource firstSource,
            out AudioSource secondSource)
        {
            GameObject playerObject = new("Test Music Player");
            createdObjects.Add(playerObject);
            MusicPlayer player = playerObject.AddComponent<MusicPlayer>();

            firstSource = CreateSource(playerObject.transform, "First Source");
            secondSource = CreateSource(playerObject.transform, "Second Source");
            AudioServiceTestFactory.SetField(player, "_firstAudioSource", firstSource);
            AudioServiceTestFactory.SetField(player, "_secondAudioSource", secondSource);

            AudioServiceConfig config = ScriptableObject.CreateInstance<AudioServiceConfig>();
            createdObjects.Add(config);
            AudioServiceTestFactory.SetField(config, "_musicFadeTime", fadeTime);
            player.Construct(config);
            return player;
        }

        private static AudioSource CreateSource(Transform parent, string name)
        {
            GameObject sourceObject = new(name);
            sourceObject.transform.SetParent(parent);
            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            sourceObject.SetActive(false);
            return source;
        }

        private MusicMetaAsset CreateMusicAsset(string name, float volume)
        {
            AudioClip clip = AudioServiceTestFactory.CreateClip(name, 1f);
            MusicMetaAsset asset = AudioServiceTestFactory.CreateMusicAsset(clip, volume);
            createdObjects.Add(clip);
            createdObjects.Add(asset);
            return asset;
        }
    }
}
