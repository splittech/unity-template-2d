using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Game.Core
{
    public class CoreBootstrap : IInitializable
    {
        private readonly LoadingScreen loadingScreen;
        private readonly SceneService sceneService;

        public CoreBootstrap(SceneService sceneService, LoadingScreen loadingScreen)
        {
            this.sceneService = sceneService;
            this.loadingScreen = loadingScreen;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        public async UniTask InitializeAsync(CancellationToken ct = default)
        {
            loadingScreen.SetProgressDescription("Startup game");
            loadingScreen.ShowImmediate();

            await sceneService.LoadOnlyInitialScenes(loadingScreen.Progress, ct);
            await loadingScreen.Hide();
        }
    }
}
