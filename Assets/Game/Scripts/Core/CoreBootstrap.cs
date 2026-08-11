using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Game.Core
{
    public class CoreBootstrap : IAsyncStartable
    {
        private readonly LoadingScreen _loadingScreen;
        private readonly SceneService _sceneService;
        private readonly AudioService _audioService;

        public CoreBootstrap(
            SceneService sceneService,
            LoadingScreen loadingScreen,
            AudioService audioService)
        {
            _sceneService = sceneService;
            _loadingScreen = loadingScreen;
            _audioService = audioService;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            _loadingScreen.SetProgressDescription("Startup game");
            _loadingScreen.ShowImmediate();

            _audioService.ApplyAllMixerGroupVolumes();

            await _sceneService.LoadInitialScenes(_loadingScreen.Progress, cancellation);
            await _loadingScreen.Hide();
        }
    }
}
