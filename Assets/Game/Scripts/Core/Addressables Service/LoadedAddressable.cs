using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Core
{
    /// <summary>
    /// Owns an Addressables load handle. Dispose it when the asset is no longer used.
    /// </summary>
    public class LoadedAddressable<T> : IDisposable where T : UnityEngine.Object
    {
        private AsyncOperationHandle<T> _handle;
        private bool _isDisposed;

        public T Asset
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(nameof(LoadedAddressable<T>));

                return _handle.Result;
            }
        }

        public LoadedAddressable(AsyncOperationHandle<T> handle)
        {
            _handle = handle;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (_handle.IsValid())
                Addressables.Release(_handle);
        }
    }
}
