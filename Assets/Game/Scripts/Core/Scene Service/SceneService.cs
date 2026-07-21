using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public class SceneService
    {
        private readonly SceneServiceConfig config;
        private readonly SceneLoader sceneLoader;

        public SceneServiceConfig Config => config;

        public SceneService(SceneServiceConfig config, SceneLoader sceneLoader)
        {
            this.config = config;
            this.sceneLoader = sceneLoader;

            config.RecreateAllScenesList();
        }

        public async UniTask SwitchScene(SceneMetaAsset sceneAsset, IProgress<float> progress, CancellationToken ct = default)
        {
            SceneMetaAsset.Validate(sceneAsset);

            CompositeProgress compositeProgress = new(value => progress.Report(value));
            IProgress<float> unloadProgress = compositeProgress.CreateSubProgress();
            IProgress<float> loadProgress = compositeProgress.CreateSubProgress();

            await UnloadAllNonpersistentScenes(unloadProgress, ct);
            await sceneLoader.LoadSceneAsync(sceneAsset, loadProgress, ct);
            progress.Report(1f);
        }

        public async UniTask LoadInitialScenes(IProgress<float> progress, CancellationToken ct = default)
        {
            List<SceneMetaAsset> scenesToLoad = config.GetAllInitialScenes();
            List<UniTask> tasks = sceneLoader.LoadManyScenesAsync(scenesToLoad, progress, ct);

            await UniTask.WhenAll(tasks);
            progress.Report(1f);
        }

        private async UniTask UnloadAllNonpersistentScenes(IProgress<float> progress, CancellationToken ct = default)
        {
            List<SceneMetaAsset> scenesToUnload = config.GetAllNonPersistentScenes();
            List<UniTask> tasks = sceneLoader.UnloadManyScenesAsync(scenesToUnload, progress, ct);

            await UniTask.WhenAll(tasks);
            progress.Report(1f);
        }

        private async UniTask UnloadAllScenesExceptCore(IProgress<float> progress, CancellationToken ct = default)
        {
            List<SceneMetaAsset> scenesToUnload = config.GetAllScenesExceptCore();
            List<UniTask> tasks = sceneLoader.UnloadManyScenesAsync(scenesToUnload, progress, ct);

            await UniTask.WhenAll(tasks);
            progress.Report(1f);
        }
    }
}