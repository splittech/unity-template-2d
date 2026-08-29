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

        private readonly List<SceneMetaAsset> _loadedScenes = new();

        public IReadOnlyList<SceneMetaAsset> LoadedScenes => _loadedScenes;

        public SceneService(SceneServiceConfig config, ILoggingService loggingService, ISceneLoader sceneLoader)
        {
            _config = config;
            _sceneLoader = sceneLoader;

            _logger = loggingService.GetLogger(LoggingChannel.SceneService);

            SceneServiceValidator.ValidateSceneList(_config);
        }

        public async UniTask LoadScene(
            SceneMetaAsset sceneToLoad,
            IProgress<float> progress = null,
            CancellationToken ct = default)
        {
            SceneServiceValidator.ValidateSceneMetaAsset(sceneToLoad);

            if (_loadedScenes.Contains(sceneToLoad))
                throw new ArgumentException($"Scene with SceneMetaAsset '{sceneToLoad.name}' is already loaded.");

            progress ??= new ImmediateProgress<float>(_ => { });

            _logger.Log($"Load scene with SceneMetaAsset '{sceneToLoad.name}'.");

            await _sceneLoader.LoadSceneAsync(sceneToLoad, progress, ct);
            _loadedScenes.Add(sceneToLoad);

            progress?.Report(1f);
        }

        public async UniTask UnloadScene(
            SceneMetaAsset sceneToUnload,
            IProgress<float> progress = null,
            CancellationToken ct = default)
        {
            SceneServiceValidator.ValidateSceneMetaAsset(sceneToUnload);

            if (!_loadedScenes.Contains(sceneToUnload))
                throw new ArgumentException($"Scene with SceneMetaAsset '{sceneToUnload.name}' is not loaded.");

            progress ??= new ImmediateProgress<float>(_ => { });

            _logger.Log($"Unload scene with SceneMetaAsset '{sceneToUnload.name}'.");

            await _sceneLoader.UnloadSceneAsync(sceneToUnload, progress, ct);
            _loadedScenes.Remove(sceneToUnload);

            progress?.Report(1f);
        }

        public async UniTask LoadSceneList(
            List<SceneMetaAsset> scenesToLoad,
            IProgress<float> progress = null,
            CancellationToken ct = default)
        {
            _logger.Log("Load scene list.");

            CompositeProgress compositeProgress = new(value => progress?.Report(value));

            List<UniTask> tasks = scenesToLoad
                .Select(scene => LoadScene(scene, compositeProgress.CreateSubProgress(), ct))
                .ToList();

            compositeProgress.EnableUpdate();
            await UniTask.WhenAll(tasks);

            progress?.Report(1f);
        }

        public async UniTask UnloadSceneList(
            List<SceneMetaAsset> scenesToUnload,
            IProgress<float> progress = null,
            CancellationToken ct = default)
        {
            _logger.Log("Unload scene list.");

            CompositeProgress compositeProgress = new(value => progress?.Report(value));

            List<UniTask> tasks = scenesToUnload
                .Select(scene => UnloadScene(scene, compositeProgress.CreateSubProgress(), ct))
                .ToList();

            compositeProgress.EnableUpdate();
            await UniTask.WhenAll(tasks);

            progress?.Report(1f);
        }

        public UniTask LoadInitialScenes(IProgress<float> progress = null, CancellationToken ct = default)
        {
            _logger.Log("Load initial scenes.");

            List<SceneMetaAsset> scenesToLoad = _config.InitialScenes;

            return LoadSceneList(scenesToLoad, progress, ct);
        }

        public UniTask UnloadNonPersistentScenes(IProgress<float> progress = null, CancellationToken ct = default)
        {
            _logger.Log("Unload nonpersistent scenes.");

            List<SceneMetaAsset> scenesToUnload = _config.NonPersistentScenes
                .Where(scene => _loadedScenes.Contains(scene))
                .ToList();

            return UnloadSceneList(scenesToUnload, progress, ct);
        }
    }
}