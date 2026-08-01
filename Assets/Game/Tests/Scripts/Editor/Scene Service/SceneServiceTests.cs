using System.Threading;
using System.Threading.Tasks;
using Game.Tests.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Game.Core.EditModeTests
{
    public class SceneServiceTests
    {
        private SceneServiceConfig sceneServiceConfig;

        [SetUp]
        public void SetUp()
        {
            sceneServiceConfig = TestConfig.Get().SceneServiceConfig;
        }

        [Test]
        public async Task SwitchScene_LoadsTargetScene()
        {
            MockSceneLoader loader = new();
            SceneService service = new(sceneServiceConfig, loader);

            await service.SwitchScene(
                sceneServiceConfig.MainMenuScene,
                new ImmediateProgress<float>(_ => { }),
                CancellationToken.None);

            Assert.That(loader.LoadedScenes, Does.Contain(sceneServiceConfig.MainMenuScene));
        }

        [Test]
        public async Task SwitchScene_UnloadsOtherScenes()
        {
            MockSceneLoader loader = new();
            SceneService service = new(sceneServiceConfig, loader);

            await service.SwitchScene(
                sceneServiceConfig.MainMenuScene,
                new ImmediateProgress<float>(_ => { }),
                CancellationToken.None);

            await service.SwitchScene(
                sceneServiceConfig.GameplayScene,
                new ImmediateProgress<float>(_ => { }),
                CancellationToken.None);

            Assert.That(loader.LoadedScenes, !Does.Contain(sceneServiceConfig.MainMenuScene));
        }
    }
}
