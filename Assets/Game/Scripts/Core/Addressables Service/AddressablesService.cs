using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public class AddressablesService
    {
        private readonly AddressablesServiceConfig _config;
        private readonly IAddressablesLoader _addressablesLoader;

        public AddressablesService(AddressablesServiceConfig config, IAddressablesLoader addressablesLoader)
        {
            _config = config;
            _addressablesLoader = addressablesLoader;
        }

        public UniTask<long> GetPackDownloadSizeAsync(
            AddressablesPackDefinition definition,
            CancellationToken ct = default)
        {
            ValidateDefinition(definition);

            if (_config.EnableLogger)
                GameLogger.Log($"Get download size of AddressablesPackDefenition with id '{definition.Id}'.");

            return _addressablesLoader.GetDownloadSizeAsync(definition.DownloadLabel, ct);
        }

        public UniTask DownloadPackAsync(
            AddressablesPackDefinition definition,
            IProgress<float> progress,
            CancellationToken ct = default)
        {
            ValidateDefinition(definition);

            if (_config.EnableLogger)
                GameLogger.Log($"Download pack of AddressablesPackDefenition with id '{definition.Id}'.");

            return _addressablesLoader.DownloadAsync(definition.DownloadLabel, progress, ct);
        }

        public UniTask<LoadedAddressable<AddressablesPack>> LoadPackAsync(
            AddressablesPackDefinition definition,
            IProgress<float> progress,
            CancellationToken ct = default)
        {
            ValidateDefinition(definition);

            if (_config.EnableLogger)
                GameLogger.Log($"Load pack of AddressablesPackDefenition with id '{definition.Id}'.");

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
