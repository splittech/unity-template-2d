using System;
using Game.Core.Tests.Doubles;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Core.Tests.Editor
{
    public class LoadedAddressableTests
    {
        private ScriptableObject asset;
        private AsyncOperationHandle<ScriptableObject> handle;
        private LoadedAddressable<ScriptableObject> loadedAddressable;

        [SetUp]
        public void SetUp()
        {
            asset = ScriptableObject.CreateInstance<FakeAddressableAsset>();
            handle = Addressables.ResourceManager.CreateCompletedOperation(asset, string.Empty);
            loadedAddressable = new LoadedAddressable<ScriptableObject>(handle);
        }

        [TearDown]
        public void TearDown()
        {
            loadedAddressable?.Dispose();

            if (asset != null)
                UnityEngine.Object.DestroyImmediate(asset);
        }

        [Test]
        public void Asset_BeforeDispose_ReturnsLoadedAsset()
        {
            Assert.That(loadedAddressable.Asset, Is.SameAs(asset));
        }

        [Test]
        public void Asset_AfterDispose_ThrowsObjectDisposedException()
        {
            loadedAddressable.Dispose();

            Assert.Throws<ObjectDisposedException>(() => _ = loadedAddressable.Asset);
        }

        [Test]
        public void Dispose_CalledTwice_DoesNotThrowAndReleasesHandle()
        {
            loadedAddressable.Dispose();

            Assert.DoesNotThrow(() => loadedAddressable.Dispose());
            Assert.That(handle.IsValid(), Is.False);
        }
    }
}
