using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core.EditModeTests
{
    public sealed class MockSceneLoader : SceneLoader
    {
        public readonly List<SceneMetaAsset> LoadCalls = new();
        public readonly List<SceneMetaAsset> UnloadCalls = new();

        public readonly HashSet<SceneMetaAsset> LoadedScenes = new();

        public override UniTask LoadSceneAsync(
            SceneMetaAsset scene,
            IProgress<float> progress,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            LoadCalls.Add(scene);
            LoadedScenes.Add(scene);

            progress.Report(1f);

            return UniTask.CompletedTask;
        }

        public override UniTask UnloadSceneAsync(
            SceneMetaAsset scene,
            IProgress<float> progress,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            // Имитируем UnitySceneLoader:
            // незагруженные сцены просто пропускаются.
            if (LoadedScenes.Remove(scene))
                UnloadCalls.Add(scene);

            progress.Report(1f);

            return UniTask.CompletedTask;
        }

        public override List<UniTask> LoadManyScenesAsync(
            List<SceneMetaAsset> scenes,
            IProgress<float> progress,
            CancellationToken ct)
        {
            CompositeProgress composite =
                new(progress.Report);

            return scenes
                .Select(scene => LoadSceneAsync(scene, composite.CreateSubProgress(), ct))
                .ToList();
        }

        public override List<UniTask> UnloadManyScenesAsync(
            List<SceneMetaAsset> scenes,
            IProgress<float> progress,
            CancellationToken ct)
        {
            CompositeProgress composite =
                new(progress.Report);

            return scenes
                .Select(scene => UnloadSceneAsync(scene, composite.CreateSubProgress(), ct))
                .ToList();
        }
    }
}