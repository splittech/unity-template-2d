using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public interface ISceneLoader
    {
        public UniTask LoadSceneAsync(SceneMetaAsset scene, IProgress<float> progress, CancellationToken ct);

        public UniTask UnloadSceneAsync(SceneMetaAsset scene, IProgress<float> progress, CancellationToken ct);

        public List<UniTask> LoadManyScenesAsync(
            List<SceneMetaAsset> sceneAssets,
            IProgress<float> progress,
            CancellationToken ct);

        public List<UniTask> UnloadManyScenesAsync(
            List<SceneMetaAsset> sceneAssets,
            IProgress<float> progress,
            CancellationToken ct);
    }
}
