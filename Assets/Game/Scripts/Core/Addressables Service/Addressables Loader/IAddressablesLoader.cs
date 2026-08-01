using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Game.Core
{
    public interface IAddressablesLoader
    {
        UniTask DownloadAsync(string key, IProgress<float> progress, CancellationToken ct);
        UniTask<long> GetDownloadSizeAsync(string key, CancellationToken ct);
        UniTask<LoadedAddressable<T>> LoadAsync<T>(AssetReference reference, IProgress<float> progress, CancellationToken ct) where T : UnityEngine.Object;
    }
}
