using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Game.Core
{
    public class CoreBootstrap : IAsyncStartable
    {
        private readonly LoadingScreen _loadingScreen;
        private readonly SceneService _sceneService;

        public CoreBootstrap(SceneService sceneService, LoadingScreen loadingScreen)
        {
            _sceneService = sceneService;
            _loadingScreen = loadingScreen;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            _loadingScreen.SetProgressDescription("Startup game");
            _loadingScreen.ShowImmediate();

            await _sceneService.LoadInitialScenes(_loadingScreen.Progress, cancellation);
            await _loadingScreen.Hide();
        }
    }
}
