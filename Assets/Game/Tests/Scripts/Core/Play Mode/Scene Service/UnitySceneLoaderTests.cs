using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Tests.Common;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Core.Tests.PlayMode
{
    public class UnitySceneLoaderTests
    {
        private SceneMetaAsset testScene;
        private UnitySceneLoader loader;

        [SetUp]
        public void SetUp()
        {
            testScene = TestConfig.Get().SceneServiceConfig.CoreScene;

            Assert.That(testScene, Is.Not.Null);

            loader = new UnitySceneLoader();
        }

        [UnityTest]
        public IEnumerator LoadScene_LoadsSceneAdditively()
        {
            return UniTask.ToCoroutine(async () =>
            {
                float progressValue = -1f;

                ImmediateProgress<float> progress = new(value => progressValue = value);

                await loader.LoadSceneAsync(testScene, progress, CancellationToken.None);

                Scene loadedScene = SceneManager.GetSceneByName(testScene.SceneReference.Name);

                Assert.That(loadedScene.IsValid(), Is.True);
                Assert.That(loadedScene.isLoaded, Is.True);

                Assert.That(progressValue, Is.EqualTo(1f).Within(0.001f));
            });
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            return UniTask.ToCoroutine(async () =>
            {
                if (testScene == null)
                    return;

                Scene scene = SceneManager.GetSceneByName(
                    testScene.SceneReference.Name);

                if (!scene.IsValid() || !scene.isLoaded)
                    return;

                await SceneManager
                    .UnloadSceneAsync(scene)
                    .ToUniTask();
            });
        }
    }
}