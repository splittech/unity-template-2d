using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Game.Core.Tests.Doubles
{
    public class MockAddressablesLoader : IAddressablesLoader
    {
        public string DownloadSizeKey { get; private set; }
        public CancellationToken DownloadSizeCancellationToken { get; private set; }

        public string DownloadKey { get; private set; }
        public IProgress<float> DownloadProgress { get; private set; }
        public CancellationToken DownloadCancellationToken { get; private set; }

        public AssetReference LoadReference { get; private set; }
        public IProgress<float> LoadProgress { get; private set; }
        public CancellationToken LoadCancellationToken { get; private set; }

        public long DownloadSizeResult { get; set; }

        public UniTask<long> GetDownloadSizeAsync(string key, CancellationToken ct)
        {
            DownloadSizeKey = key;
            DownloadSizeCancellationToken = ct;
            return UniTask.FromResult(DownloadSizeResult);
        }

        public UniTask DownloadAsync(
            string key,
            IProgress<float> progress,
            CancellationToken ct)
        {
            DownloadKey = key;
            DownloadProgress = progress;
            DownloadCancellationToken = ct;
            return UniTask.CompletedTask;
        }

        public UniTask<LoadedAddressable<T>> LoadAsync<T>(
            AssetReference reference,
            IProgress<float> progress,
            CancellationToken ct)
            where T : UnityEngine.Object
        {
            LoadReference = reference;
            LoadProgress = progress;
            LoadCancellationToken = ct;
            return UniTask.FromResult<LoadedAddressable<T>>(null);
        }
    }
}
