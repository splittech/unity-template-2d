using System;
using System.Threading;
using System.Threading.Tasks;
using Game.Core.Tests.Mocks;
using NUnit.Framework;
using UnityEngine.AddressableAssets;

namespace Game.Core.Tests.Editor
{
    public class AddressablesServiceTests
    {
        private const string PackId = "test-pack";
        private const string DownloadLabel = "test-pack-label";
        private const string PackGuid = "0123456789abcdef0123456789abcdef";

        private MockAddressablesLoader loader;
        private AddressablesService service;
        private AddressablesPackReference packReference;
        private AddressablesPackDefinition definition;

        [SetUp]
        public void SetUp()
        {
            loader = new MockAddressablesLoader();
            service = new AddressablesService(loader);
            packReference = new AddressablesPackReference(PackGuid);
            definition = new AddressablesPackDefinition(
                PackId,
                new AssetLabelReference { labelString = DownloadLabel },
                packReference);
        }

        [Test]
        public void GetPackDownloadSizeAsync_NullDefinition_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await service.GetPackDownloadSizeAsync(null));
        }

        [Test]
        public async Task GetPackDownloadSizeAsync_ValidDefinition_PassesLabelAndTokenToLoader()
        {
            loader.DownloadSizeResult = 1234L;
            using CancellationTokenSource cts = new();

            long result = await service.GetPackDownloadSizeAsync(definition, cts.Token);

            Assert.That(result, Is.EqualTo(1234L));
            Assert.That(loader.DownloadSizeKey, Is.EqualTo(DownloadLabel));
            Assert.That(loader.DownloadSizeCancellationToken, Is.EqualTo(cts.Token));
        }

        [Test]
        public async Task DownloadPackAsync_ValidDefinition_PassesArgumentsToLoader()
        {
            IProgress<float> progress = new Progress<float>();
            using CancellationTokenSource cts = new();

            await service.DownloadPackAsync(definition, progress, cts.Token);

            Assert.That(loader.DownloadKey, Is.EqualTo(DownloadLabel));
            Assert.That(loader.DownloadProgress, Is.SameAs(progress));
            Assert.That(loader.DownloadCancellationToken, Is.EqualTo(cts.Token));
        }

        [Test]
        public void LoadPackAsync_InvalidReference_ThrowsInvalidOperationException()
        {
            AddressablesPackDefinition invalidDefinition = new(
                PackId,
                new AssetLabelReference { labelString = DownloadLabel },
                new AddressablesPackReference(string.Empty));

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.LoadPackAsync(invalidDefinition, null));
        }

        [Test]
        public async Task LoadPackAsync_ValidDefinition_PassesArgumentsToLoader()
        {
            IProgress<float> progress = new Progress<float>();
            using CancellationTokenSource cts = new();

            await service.LoadPackAsync(definition, progress, cts.Token);

            Assert.That(loader.LoadReference, Is.SameAs(packReference));
            Assert.That(loader.LoadProgress, Is.SameAs(progress));
            Assert.That(loader.LoadCancellationToken, Is.EqualTo(cts.Token));
        }
    }
}
