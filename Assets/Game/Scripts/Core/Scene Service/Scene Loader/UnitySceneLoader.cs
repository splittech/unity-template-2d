using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public class UnitySceneLoader : SceneLoader
    {
        private const float SceneReadyProgress = 0.9f;

        public override async UniTask LoadSceneAsync(SceneMetaAsset sceneAsset, IProgress<float> progress, CancellationToken ct)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(
                sceneAsset.SceneReference.Name, LoadSceneMode.Additive);

            if (operation == null)
                throw new InvalidOperationException(
                    $"Failed to start loading scene '{sceneAsset.SceneReference.Name}'.");

            operation.allowSceneActivation = false;

            try
            {
                while (!operation.isDone)
                {
                    progress.Report(
                        Mathf.Clamp01(operation.progress / SceneReadyProgress));

                    if (operation.progress >= SceneReadyProgress)
                        operation.allowSceneActivation = true;

                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
            finally
            {
                operation.allowSceneActivation = true;
            }

            progress.Report(1f);
        }

        public override UniTask UnloadSceneAsync(SceneMetaAsset sceneAsset, IProgress<float> progress, CancellationToken ct)
        {
            Scene scene = SceneManager.GetSceneByName(sceneAsset.SceneReference.Name);

            if (!scene.IsValid() || !scene.isLoaded)
            {
                progress.Report(1f);
                return UniTask.CompletedTask;
            }

            AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);

            if (operation == null)
            {
                progress.Report(1f);
                return UniTask.CompletedTask;
            }

            return operation.ToUniTask(progress, PlayerLoopTiming.Update, ct);
        }

        public override List<UniTask> LoadManyScenesAsync(
            List<SceneMetaAsset> sceneAssets,
            IProgress<float> progress,
            CancellationToken ct)
        {
            CompositeProgress compositeProgress = new(value => progress.Report(value));

            return sceneAssets.Select(scene =>
            {
                IProgress<float> subProgress = compositeProgress.CreateSubProgress();
                return LoadSceneAsync(scene, subProgress, ct);
            }).ToList();
        }

        public override List<UniTask> UnloadManyScenesAsync(
            List<SceneMetaAsset> sceneAssets,
            IProgress<float> progress,
            CancellationToken ct)
        {
            CompositeProgress compositeProgress = new(value => progress.Report(value));

            return sceneAssets
                .Select(scene => UnloadSceneAsync(scene, compositeProgress.CreateSubProgress(), ct))
                .ToList();
        }
    }
}