using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public class AddressablesService
    {
        private readonly IAddressablesLoader _addressablesLoader;

        public AddressablesService(IAddressablesLoader addressablesLoader)
        {
            _addressablesLoader = addressablesLoader;
        }

        public UniTask<long> GetPackDownloadSizeAsync(
            AddressablesPackDefinition definition,
            CancellationToken ct = default)
        {
            ValidateDefinition(definition);
            return _addressablesLoader.GetDownloadSizeAsync(definition.DownloadLabel, ct);
        }

        public UniTask DownloadPackAsync(
            AddressablesPackDefinition definition,
            IProgress<float> progress,
            CancellationToken ct = default)
        {
            ValidateDefinition(definition);
            return _addressablesLoader.DownloadAsync(definition.DownloadLabel, progress, ct);
        }

        public UniTask<LoadedAddressable<AddressablesPack>> LoadPackAsync(
            AddressablesPackDefinition definition,
            IProgress<float> progress,
            CancellationToken ct = default)
        {
            ValidateDefinition(definition);
            return _addressablesLoader.LoadAsync<AddressablesPack>(definition.Pack, progress, ct);
        }

        private void ValidateDefinition(AddressablesPackDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (definition.Pack == null || !definition.Pack.RuntimeKeyIsValid())
                throw new InvalidOperationException($"Content pack '{definition.Id}' has no valid pack reference.");
        }
    }
}
