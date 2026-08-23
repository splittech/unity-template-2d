using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Game.Core.Tests.Doubles;
using NUnit.Framework;
using UnityEngine;

namespace Game.Core.Tests.Editor
{
    public sealed class SceneServiceTests
    {
        private readonly List<UnityEngine.Object> createdObjects = new();

        private SceneMetaAsset coreScene;
        private SceneMetaAsset initialPersistentScene;
        private SceneMetaAsset gameplayScene;
        private TestSceneServiceConfig config;
        private LoggingServiceConfig loggingConfig;
        private MockSceneLoader loader;
        private ILoggingService loggingService;
        private SceneService service;

        [SetUp]
        public void SetUp()
        {
            // Mark core as initial to verify that LoadInitialScenes explicitly excludes it.
            coreScene = CreateScene(initial: true, persistent: false);
            initialPersistentScene = CreateScene(initial: true, persistent: true);
            gameplayScene = CreateScene(initial: false, persistent: false);
            config = CreateConfig(coreScene, initialPersistentScene, gameplayScene);

            loader = new MockSceneLoader();

            loggingConfig = ScriptableObject.CreateInstance<LoggingServiceConfig>();
            loggingService = new MockLoggingService();
            service = new SceneService(config, loggingService, loader);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (UnityEngine.Object createdObject in createdObjects)
            {
                if (createdObject != null)
                    UnityEngine.Object.DestroyImmediate(createdObject);
            }

            createdObjects.Clear();
        }

        [Test]
        public async Task SwitchScene_LoadsTargetScene()
        {
            await service.SwitchScene(
                gameplayScene,
                IgnoreProgress(),
                CancellationToken.None);

            Assert.That(loader.LoadedScenes, Does.Contain(gameplayScene));
        }

        [Test]
        public async Task SwitchScene_UnloadsBeforeLoadingTarget()
        {
            loader.LoadedScenes.Add(gameplayScene);

            await service.SwitchScene(
                initialPersistentScene,
                IgnoreProgress(),
                CancellationToken.None);

            int unloadIndex = loader.Calls.FindIndex(call =>
                call.Type == SceneLoaderOperationType.Unload &&
                call.Scene == gameplayScene);
            int loadIndex = loader.Calls.FindIndex(call =>
                call.Type == SceneLoaderOperationType.Load &&
                call.Scene == initialPersistentScene);

            Assert.That(unloadIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(loadIndex, Is.GreaterThan(unloadIndex));
        }

        [Test]
        public async Task SwitchScene_DoesNotUnloadPersistentScene()
        {
            loader.LoadedScenes.Add(initialPersistentScene);

            await service.SwitchScene(
                gameplayScene,
                IgnoreProgress(),
                CancellationToken.None);

            Assert.That(loader.UnloadCalls, Has.No.Member(initialPersistentScene));
            Assert.That(loader.LoadedScenes, Does.Contain(initialPersistentScene));
        }

        [Test]
        public async Task SwitchScene_DoesNotUnloadCoreScene()
        {
            loader.LoadedScenes.Add(coreScene);

            await service.SwitchScene(
                gameplayScene,
                IgnoreProgress(),
                CancellationToken.None);

            Assert.That(loader.UnloadCalls, Has.No.Member(coreScene));
            Assert.That(loader.LoadedScenes, Does.Contain(coreScene));
        }

        [Test]
        public void SwitchScene_CancelledToken_DoesNotLoadTarget()
        {
            using CancellationTokenSource cts = new();
            cts.Cancel();

            Assert.CatchAsync<OperationCanceledException>(async () =>
                await service.SwitchScene(gameplayScene, IgnoreProgress(), cts.Token));

            Assert.That(loader.LoadCalls, Is.Empty);
        }

        [Test]
        public void SwitchScene_UnloadFails_DoesNotLoadTarget()
        {
            loader.LoadedScenes.Add(gameplayScene);
            loader.UnloadException = new InvalidOperationException("Unload failed.");

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.SwitchScene(
                    initialPersistentScene,
                    IgnoreProgress(),
                    CancellationToken.None));

            Assert.That(loader.LoadCalls, Has.No.Member(initialPersistentScene));
        }

        [Test]
        public async Task LoadInitialScenes_LoadsOnlyInitialNonCoreScenes()
        {
            await service.LoadInitialScenes(
                IgnoreProgress(),
                CancellationToken.None);

            Assert.That(loader.LoadCalls, Is.EquivalentTo(new[] { initialPersistentScene }));
        }

        private SceneMetaAsset CreateScene(bool initial, bool persistent)
        {
            SceneMetaAsset scene = SceneServiceTestData.CreateScene(initial, persistent);
            createdObjects.Add(scene);
            return scene;
        }

        private TestSceneServiceConfig CreateConfig(
            SceneMetaAsset core,
            params SceneMetaAsset[] scenes)
        {
            TestSceneServiceConfig result = SceneServiceTestData.CreateConfig(core, scenes);
            createdObjects.Add(result);
            return result;
        }

        private static IProgress<float> IgnoreProgress()
        {
            return new ImmediateProgress<float>(_ => { });
        }
    }
}
