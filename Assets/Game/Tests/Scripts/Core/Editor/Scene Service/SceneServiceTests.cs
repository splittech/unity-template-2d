using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Core.Tests.Doubles;
using NUnit.Framework;

namespace Game.Core.Tests.Editor
{
    public sealed class SceneServiceTests
    {
        [Test]
        public async Task LoadScene_NoSceneLoaded_SceneAddedToLoadedScenes()
        {
            // Arrange.
            SceneMetaAsset sceneToLoad = Create.SceneMetaAsset();

            SceneService sceneService = Setup.SceneService();

            // Act.
            await sceneService.LoadScene(sceneToLoad);

            // Assert.
            Assert.That(sceneService.LoadedScenes, Has.Member(sceneToLoad));
        }

        [Test]
        public async Task UnloadScene_SceneLoaded_SceneRemovedFromLoadedScenes()
        {
            // Arrange.
            SceneMetaAsset sceneToUnload = Create.SceneMetaAsset();

            SceneService sceneService = Setup.SceneService();

            await sceneService.LoadScene(sceneToUnload);

            // Act.
            await sceneService.UnloadScene(sceneToUnload);

            // Assert.
            Assert.That(sceneService.LoadedScenes, Has.No.Member(sceneToUnload));
        }

        [Test]
        public async Task LoadSceneList_NoSceneLoaded_ScenesAddedToLoadedScenes()
        {
            // Arrange.
            SceneMetaAsset firstScene = Create.SceneMetaAsset();
            SceneMetaAsset secondScene = Create.SceneMetaAsset();

            SceneService sceneService = Setup.SceneService();

            // Act.
            await sceneService.LoadSceneList(
                new List<SceneMetaAsset> { firstScene, secondScene }
            );

            // Assert.
            Assert.That(sceneService.LoadedScenes, Has.Member(firstScene));
            Assert.That(sceneService.LoadedScenes, Has.Member(secondScene));
        }

        [Test]
        public async Task UnloadSceneList_ScenesLoaded_ScenesRemovedFromLoadedScenes()
        {
            // Arrange.
            SceneMetaAsset firstScene = Create.SceneMetaAsset();
            SceneMetaAsset secondScene = Create.SceneMetaAsset();

            SceneService sceneService = Setup.SceneService();

            await sceneService.LoadSceneList(
                new List<SceneMetaAsset> { firstScene, secondScene }
            );

            // Act.
            await sceneService.UnloadSceneList(
                new List<SceneMetaAsset> { firstScene, secondScene }
            );

            // Assert.
            Assert.That(sceneService.LoadedScenes, Has.No.Member(firstScene));
            Assert.That(sceneService.LoadedScenes, Has.No.Member(secondScene));
        }

        [Test]
        public async Task LoadInitialScenes_NoSceneLoaded_InitialScenesAddedToLoadedScenes()
        {
            // Arrange.
            SceneMetaAsset firstInitialScene = Create.SceneMetaAsset(initial: true);
            SceneMetaAsset secondInitialScene = Create.SceneMetaAsset(initial: true);

            SceneServiceConfig config = Create.SceneServiceConfig(
                otherScenes: new List<SceneMetaAsset> { firstInitialScene, secondInitialScene }
            );

            SceneService sceneService = Setup.SceneService(config);

            // Act.
            await sceneService.LoadInitialScenes();

            // Assert.
            Assert.That(sceneService.LoadedScenes, Has.Member(firstInitialScene));
            Assert.That(sceneService.LoadedScenes, Has.Member(secondInitialScene));
        }

        [Test]
        public async Task UnloadNonPersistentScenes_NonPersistentScenesLoaded_NonPersistentScenesRemovedFromLoadedScenes()
        {
            // Arrange.
            SceneMetaAsset firstNonPersistentScene = Create.SceneMetaAsset(persistent: false);
            SceneMetaAsset secondNonPersistentScene = Create.SceneMetaAsset(persistent: false);

            SceneServiceConfig config = Create.SceneServiceConfig(
                otherScenes: new List<SceneMetaAsset> { firstNonPersistentScene, secondNonPersistentScene }
            );

            SceneService sceneService = Setup.SceneService(config);

            await sceneService.LoadSceneList(
                new List<SceneMetaAsset> { firstNonPersistentScene, secondNonPersistentScene }
            );

            // Act.
            await sceneService.UnloadNonPersistentScenes();

            // Assert.
            Assert.That(sceneService.LoadedScenes, Has.No.Member(firstNonPersistentScene));
            Assert.That(sceneService.LoadedScenes, Has.No.Member(secondNonPersistentScene));
        }
    }
}
