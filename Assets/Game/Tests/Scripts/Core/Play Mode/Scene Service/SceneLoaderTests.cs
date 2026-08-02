using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Game.Core.Tests.PlayMode
{
    public class SceneLoaderTests
    {
        private const string TestSceneGuid = "c69c75e945759e246893415ee802a415";

        private SceneMetaAsset testScene;
        private SceneLoader loader;

        [SetUp]
        public void SetUp()
        {
            testScene = ScriptableObject.CreateInstance<SceneMetaAsset>();

            FieldInfo sceneReferenceField = typeof(SceneMetaAsset).GetField(
                "_sceneReference",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(sceneReferenceField, Is.Not.Null);
            sceneReferenceField.SetValue(testScene, new SceneReference(TestSceneGuid));

            loader = new SceneLoader();
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

        [UnityTest]
        public IEnumerator LoadSceneAsync_ProgressIsBoundedAndEndsAtOne()
        {
            return UniTask.ToCoroutine(async () =>
            {
                List<float> progressValues = new();

                await loader.LoadSceneAsync(
                    testScene,
                    new ImmediateProgress<float>(progressValues.Add),
                    CancellationToken.None);

                Assert.That(progressValues, Is.Not.Empty);
                Assert.That(progressValues, Has.All.InRange(0f, 1f));
                Assert.That(progressValues[^1], Is.EqualTo(1f).Within(0.001f));

                for (int i = 1; i < progressValues.Count; i++)
                {
                    Assert.That(
                        progressValues[i],
                        Is.GreaterThanOrEqualTo(progressValues[i - 1]));
                }
            });
        }

        [UnityTest]
        public IEnumerator UnloadSceneAsync_LoadedScene_UnloadsScene()
        {
            return UniTask.ToCoroutine(async () =>
            {
                await loader.LoadSceneAsync(
                    testScene,
                    new ImmediateProgress<float>(_ => { }),
                    CancellationToken.None);

                await loader.UnloadSceneAsync(
                    testScene,
                    new ImmediateProgress<float>(_ => { }),
                    CancellationToken.None);

                Scene scene = SceneManager.GetSceneByName(testScene.SceneReference.Name);
                Assert.That(!scene.IsValid() || !scene.isLoaded, Is.True);
            });
        }

        [UnityTest]
        public IEnumerator UnloadSceneAsync_NotLoadedScene_CompletesAndReportsOne()
        {
            return UniTask.ToCoroutine(async () =>
            {
                Scene scene = SceneManager.GetSceneByName(testScene.SceneReference.Name);
                Assert.That(!scene.IsValid() || !scene.isLoaded, Is.True);

                float progressValue = -1f;

                await loader.UnloadSceneAsync(
                    testScene,
                    new ImmediateProgress<float>(value => progressValue = value),
                    CancellationToken.None);

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

                if (scene.IsValid() && scene.isLoaded)
                {
                    await SceneManager
                        .UnloadSceneAsync(scene)
                        .ToUniTask();
                }

                Object.Destroy(testScene);
                testScene = null;
            });
        }
    }
}
