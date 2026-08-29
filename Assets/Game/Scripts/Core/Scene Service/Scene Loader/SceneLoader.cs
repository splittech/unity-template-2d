using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public class SceneLoader : ISceneLoader
    {
        /// <summary>
        /// Loading scene in unity is considered done at 90%.
        /// </summary>
        private const float UnitySceneReadyProgress = 0.9f;

        public async UniTask LoadSceneAsync(SceneMetaAsset sceneAsset, IProgress<float> progress, CancellationToken ct)
        {
            // Unity scene loading cannot be cancelled after it starts.
            // Check cancellation only before creating AsyncOperation.
            ct.ThrowIfCancellationRequested();

            progress ??= EmptyProgress<float>.Instance;

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
                        Mathf.Clamp01(operation.progress / UnitySceneReadyProgress));

                    if (operation.progress >= UnitySceneReadyProgress)
                        operation.allowSceneActivation = true;

                    await UniTask.Yield(PlayerLoopTiming.Update, CancellationToken.None);
                }
            }
            finally
            {
                operation.allowSceneActivation = true;
            }

            progress.Report(1f);
        }

        public UniTask UnloadSceneAsync(SceneMetaAsset sceneAsset, IProgress<float> progress, CancellationToken ct)
        {
            // Unity scene unloading cannot be cancelled after it starts.
            // Check cancellation only before creating AsyncOperation.
            ct.ThrowIfCancellationRequested();

            progress ??= EmptyProgress<float>.Instance;

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

            return operation.ToUniTask(progress, PlayerLoopTiming.Update, CancellationToken.None);
        }
    }
}