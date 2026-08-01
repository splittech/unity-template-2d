using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Core
{
    public sealed class AddressablesLoader
    {
        public async UniTask<long> GetDownloadSizeAsync(
            string key,
            CancellationToken ct)
        {
            ValidateKey(key);

            AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(key);

            try
            {
                while (!handle.IsDone)
                {
                    ct.ThrowIfCancellationRequested();
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
                ThrowIfFailed(handle, $"get download size for '{key}'");
                return handle.Result;
            }
            finally
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }

        public async UniTask DownloadAsync(
            string key,
            IProgress<float> progress,
            CancellationToken ct)
        {
            ValidateKey(key);

            AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(
                key,
                autoReleaseHandle: false);

            try
            {
                while (!handle.IsDone)
                {
                    ct.ThrowIfCancellationRequested();
                    progress?.Report(handle.GetDownloadStatus().Percent);
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                ThrowIfFailed(handle, $"download dependencies for '{key}'");
                progress?.Report(1f);
            }
            finally
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }

        public async UniTask<LoadedAddressable<T>> LoadAsync<T>(
            AssetReference reference,
            IProgress<float> progress,
            CancellationToken ct)
            where T : UnityEngine.Object
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            if (!reference.RuntimeKeyIsValid())
                throw new InvalidOperationException("Addressable reference has no valid runtime key.");

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(reference);

            try
            {
                while (!handle.IsDone)
                {
                    ct.ThrowIfCancellationRequested();
                    progress?.Report(Mathf.Clamp01(handle.PercentComplete));
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                ThrowIfFailed(handle, $"load asset '{reference.RuntimeKey}'");
                progress?.Report(1f);

                return new LoadedAddressable<T>(handle);
            }
            catch
            {
                if (handle.IsValid())
                    Addressables.Release(handle);

                throw;
            }
        }

        private void ThrowIfFailed<T>(
            AsyncOperationHandle<T> handle,
            string operation)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                return;

            throw new InvalidOperationException($"Addressables failed to {operation}.", handle.OperationException);
        }

        private void ThrowIfFailed(
            AsyncOperationHandle handle,
            string operation)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                return;

            throw new InvalidOperationException($"Addressables failed to {operation}.", handle.OperationException);
        }

        private void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Addressables key or label cannot be empty.", nameof(key));
        }
    }
}
