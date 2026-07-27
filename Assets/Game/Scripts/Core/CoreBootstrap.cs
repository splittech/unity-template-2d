using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Game.Core
{
    public class CoreBootstrap : IInitializable
    {
        private readonly LoadingScreen _loadingScreen;
        private readonly SceneService _sceneService;

        public CoreBootstrap(SceneService sceneService, LoadingScreen loadingScreen)
        {
            _sceneService = sceneService;
            _loadingScreen = loadingScreen;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        public async UniTask InitializeAsync(CancellationToken ct = default)
        {
            _loadingScreen.SetProgressDescription("Startup game");
            _loadingScreen.ShowImmediate();

            await _sceneService.LoadInitialScenes(_loadingScreen.Progress, ct);
            await _loadingScreen.Hide();
        }
    }
}
