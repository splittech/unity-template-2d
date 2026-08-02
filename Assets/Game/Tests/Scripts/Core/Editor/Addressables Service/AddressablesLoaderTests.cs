using System;
using System.Threading;
using NUnit.Framework;
using UnityEngine.AddressableAssets;

namespace Game.Core.Tests.Editor
{
    public class AddressablesLoaderTests
    {
        private AddressablesLoader loader;

        [SetUp]
        public void SetUp()
        {
            loader = new AddressablesLoader();
        }

        [Test]
        public void LoadAsync_NullReference_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await loader.LoadAsync<AddressablesPack>(
                    null,
                    null,
                    CancellationToken.None));
        }

        [Test]
        public void LoadAsync_InvalidReference_ThrowsInvalidOperationException()
        {
            AssetReference reference = new AssetReference(string.Empty);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await loader.LoadAsync<AddressablesPack>(
                    reference,
                    null,
                    CancellationToken.None));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GetDownloadSizeAsync_InvalidKey_ThrowsArgumentException(string key)
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await loader.GetDownloadSizeAsync(key, CancellationToken.None));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void DownloadAsync_InvalidKey_ThrowsArgumentException(string key)
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await loader.DownloadAsync(key, null, CancellationToken.None));
        }
    }
}
