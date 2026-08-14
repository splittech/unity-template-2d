using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public class SceneService
    {
        private readonly SceneServiceConfig _config;
        private readonly ISceneLoader _sceneLoader;

        public SceneServiceConfig Config => _config;

        public SceneService(SceneServiceConfig config, ISceneLoader sceneLoader)
        {
            _config = config;
            _sceneLoader = sceneLoader;

            config.RecreateAllScenesList();
        }

        public async UniTask SwitchScene(SceneMetaAsset sceneAsset, IProgress<float> progress, CancellationToken ct = default)
        {
            if (_config.EnableLogger)
                GameLogger.Log($"Switch scene to SceneMetaAsset '{sceneAsset.name}'.");

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
            if (_config.EnableLogger)
                GameLogger.Log($"Load initial scenes.");

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

        private async UniTask UnloadAllScenesExceptCore(IProgress<float> progress, CancellationToken ct = default)
        {
            List<SceneMetaAsset> scenesToUnload = _config.GetAllScenesExceptCore();
            List<UniTask> tasks = _sceneLoader.UnloadManyScenesAsync(scenesToUnload, progress, ct);

            await UniTask.WhenAll(tasks);
            progress.Report(1f);
        }
    }
}