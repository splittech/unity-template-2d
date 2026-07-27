using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public abstract class SceneLoader
    {
        public abstract UniTask LoadSceneAsync(
            SceneMetaAsset scene,
            IProgress<float> progress,
            CancellationToken ct);

        public abstract UniTask UnloadSceneAsync(
            SceneMetaAsset scene,
            IProgress<float> progress,
            CancellationToken ct);

        public abstract List<UniTask> LoadManyScenesAsync(
            List<SceneMetaAsset> sceneAssets,
            IProgress<float> progress,
            CancellationToken ct);

        public abstract List<UniTask> UnloadManyScenesAsync(
            List<SceneMetaAsset> sceneAssets,
            IProgress<float> progress,
            CancellationToken ct);
    }
}
