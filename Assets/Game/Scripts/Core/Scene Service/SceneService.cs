using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public class SceneService
    {
        private readonly SceneServiceConfig _config;
        private readonly ISceneLoader _sceneLoader;
        private readonly GameLogger _logger;

        public SceneServiceConfig Config => _config;

        public SceneService(SceneServiceConfig config, ILoggingService loggingService, ISceneLoader sceneLoader)
        {
            _config = config;
            _sceneLoader = sceneLoader;

            _logger = loggingService.GetLogger(LoggingChannel.SceneService);

            config.RecreateAllScenesList();
            ValidateAllScenesList();
        }

        public async UniTask SwitchScene(SceneMetaAsset sceneAsset, IProgress<float> progress, CancellationToken ct = default)
        {
            _logger.Log($"Switch scene to SceneMetaAsset '{sceneAsset.name}'.");

            SceneMetaAsset.Validate(sceneAsset);

            CompositeProgress compositeProgress = new(value => progress.Report(value));
            IProgress<float> unloadProgress = compositeProgress.CreateSubProgress();
            IProgress<float> loadProgress = compositeProgress.CreateSubProgress();

            await UnloadAllNonpersistentScenes(unloadProgress, ct);
            await _sceneLoader.LoadSceneAsync(sceneAsset, loadProgress, ct);
            progress.Report(1f);
        }

        public async UniTask LoadInitialScenes(IProgress<float> progress, CancellationToken ct = default)
        {
            _logger.Log($"Load initial scenes.");

            List<SceneMetaAsset> scenesToLoad = _config.GetAllInitialScenes();
            List<UniTask> tasks = _sceneLoader.LoadManyScenesAsync(scenesToLoad, progress, ct);

            await UniTask.WhenAll(tasks);
            progress.Report(1f);
        }

        private async UniTask UnloadAllNonpersistentScenes(IProgress<float> progress, CancellationToken ct = default)
        {
            List<SceneMetaAsset> scenesToUnload = _config.GetAllNonPersistentScenes();
            List<UniTask> tasks = _sceneLoader.UnloadManyScenesAsync(scenesToUnload, progress, ct);

            await UniTask.WhenAll(tasks);
            progress.Report(1f);
        }

        private void ValidateAllScenesList()
        {
            _config.AllScenes.ForEach(scene => SceneMetaAsset.Validate(scene));

            if (_config.AllScenes.Distinct().Count() != _config.AllScenes.Count)
                throw new InvalidOperationException("Scene list contains duplicates.");
        }
    }
}