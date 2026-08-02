using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core.Tests.Doubles
{
    public enum SceneLoaderOperationType
    {
        Load,
        Unload
    }

    public readonly struct SceneLoaderCall
    {
        public SceneLoaderOperationType Type { get; }
        public SceneMetaAsset Scene { get; }

        public SceneLoaderCall(SceneLoaderOperationType type, SceneMetaAsset scene)
        {
            Type = type;
            Scene = scene;
        }
    }

    public class MockSceneLoader : ISceneLoader
    {
        public readonly List<SceneMetaAsset> LoadCalls = new();
        public readonly List<SceneMetaAsset> UnloadCalls = new();
        public readonly List<SceneLoaderCall> Calls = new();
        public readonly HashSet<SceneMetaAsset> LoadedScenes = new();

        public Exception LoadException { get; set; }
        public Exception UnloadException { get; set; }

        public UniTask LoadSceneAsync(
            SceneMetaAsset scene,
            IProgress<float> progress,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            Calls.Add(new SceneLoaderCall(SceneLoaderOperationType.Load, scene));

            if (LoadException != null)
                throw LoadException;

            LoadCalls.Add(scene);
            LoadedScenes.Add(scene);
            progress.Report(1f);
            return UniTask.CompletedTask;
        }

        public UniTask UnloadSceneAsync(
            SceneMetaAsset scene,
            IProgress<float> progress,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            Calls.Add(new SceneLoaderCall(SceneLoaderOperationType.Unload, scene));

            if (UnloadException != null)
                throw UnloadException;

            // Match SceneLoader: unloading an absent scene succeeds without work.
            if (LoadedScenes.Remove(scene))
                UnloadCalls.Add(scene);

            progress.Report(1f);
            return UniTask.CompletedTask;
        }

        public List<UniTask> LoadManyScenesAsync(
            List<SceneMetaAsset> scenes,
            IProgress<float> progress,
            CancellationToken ct)
        {
            CompositeProgress composite = new(progress.Report);
            return scenes
                .Select(scene => LoadSceneAsync(scene, composite.CreateSubProgress(), ct))
                .ToList();
        }

        public List<UniTask> UnloadManyScenesAsync(
            List<SceneMetaAsset> scenes,
            IProgress<float> progress,
            CancellationToken ct)
        {
            CompositeProgress composite = new(progress.Report);
            return scenes
                .Select(scene => UnloadSceneAsync(scene, composite.CreateSubProgress(), ct))
                .ToList();
        }
    }
}
