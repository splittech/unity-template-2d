using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Tests.Doubles;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.TestTools;

namespace Game.Core.Tests.PlayMode
{
    public class AddressablesLoaderPlayModeTests
    {
        private const string AssetGuid = "1234567890abcdef1234567890abcdef";
        private const string LocatorId = "AddressablesLoaderPlayModeTests.Locator";

        private AddressablesLoader loader;
        private AddressablesPackReference reference;
        private FakeAddressableAsset asset;
        private TestAssetProvider provider;
        private TestResourceLocator locator;
        private LoadedAddressable<FakeAddressableAsset> loadedAddressable;
        private object addressablesImplementation;
        private FieldInfo hasStartedInitializationField;
        private bool initializationWasStarted;
        private readonly Dictionary<FieldInfo, object> initializationCallbacks = new();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // A custom locator still uses the real Addressables/ResourceManager pipeline,
            // but keeps this test independent from a built content catalog.
            loader = new AddressablesLoader();
            reference = new AddressablesPackReference(AssetGuid);
            asset = ScriptableObject.CreateInstance<FakeAddressableAsset>();
            provider = new TestAssetProvider(asset);
            locator = new TestResourceLocator(LocatorId, AssetGuid, provider.ProviderId);

            BypassCatalogInitialization();
            Addressables.ResourceManager.ResourceProviders.Add(provider);
            Addressables.AddResourceLocator(locator);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            loadedAddressable?.Dispose();
            loadedAddressable = null;

            if (locator != null)
                Addressables.RemoveResourceLocator(locator);

            if (provider != null)
                Addressables.ResourceManager.ResourceProviders.Remove(provider);

            RestoreInitializationState();

            if (asset != null)
                UnityEngine.Object.Destroy(asset);

            LogAssert.ignoreFailingMessages = false;
            yield return null;
        }

        private void BypassCatalogInitialization()
        {
            const BindingFlags staticField =
                BindingFlags.Static | BindingFlags.NonPublic;
            const BindingFlags instanceField =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            FieldInfo implementationField = typeof(Addressables).GetField(
                "m_AddressablesInstance",
                staticField);
            Assert.That(implementationField, Is.Not.Null);

            addressablesImplementation = implementationField.GetValue(null);
            Assert.That(addressablesImplementation, Is.Not.Null);

            hasStartedInitializationField = addressablesImplementation
                .GetType()
                .GetField("hasStartedInitialization", instanceField);
            Assert.That(hasStartedInitializationField, Is.Not.Null);

            initializationWasStarted =
                (bool)hasStartedInitializationField.GetValue(addressablesImplementation);

            SetInitializationCallback(
                "m_OnHandleCompleteAction",
                "OnHandleCompleted",
                instanceField);
            SetInitializationCallback(
                "m_OnSceneHandleCompleteAction",
                "OnSceneHandleCompleted",
                instanceField);
            SetInitializationCallback(
                "m_OnHandleDestroyedAction",
                "OnHandleDestroyed",
                instanceField);

            hasStartedInitializationField.SetValue(addressablesImplementation, true);
        }

        private void SetInitializationCallback(
            string fieldName,
            string methodName,
            BindingFlags flags)
        {
            Type implementationType = addressablesImplementation.GetType();
            FieldInfo callbackField = implementationType.GetField(fieldName, flags);
            MethodInfo callbackMethod = implementationType.GetMethod(methodName, flags);

            Assert.That(callbackField, Is.Not.Null);
            Assert.That(callbackMethod, Is.Not.Null);

            initializationCallbacks.Add(
                callbackField,
                callbackField.GetValue(addressablesImplementation));
            callbackField.SetValue(
                addressablesImplementation,
                Delegate.CreateDelegate(
                    callbackField.FieldType,
                    addressablesImplementation,
                    callbackMethod));
        }

        private void RestoreInitializationState()
        {
            if (addressablesImplementation == null ||
                hasStartedInitializationField == null)
            {
                return;
            }

            hasStartedInitializationField.SetValue(
                addressablesImplementation,
                initializationWasStarted);

            foreach (KeyValuePair<FieldInfo, object> callback in initializationCallbacks)
            {
                callback.Key.SetValue(addressablesImplementation, callback.Value);
            }

            initializationCallbacks.Clear();
            addressablesImplementation = null;
            hasStartedInitializationField = null;
        }

        [UnityTest]
        public IEnumerator LoadAsync_ValidReference_ReturnsExpectedAsset()
        {
            return UniTask.ToCoroutine(async () =>
            {
                loadedAddressable = await loader.LoadAsync<FakeAddressableAsset>(
                    reference,
                    progress: null,
                    CancellationToken.None);

                Assert.That(loadedAddressable, Is.Not.Null);
                Assert.That(loadedAddressable.Asset, Is.SameAs(asset));
            });
        }

        [UnityTest]
        public IEnumerator LoadAsync_Success_ReportsFinalProgress()
        {
            return UniTask.ToCoroutine(async () =>
            {
                float progressValue = -1f;
                IProgress<float> progress = new ImmediateProgress<float>(
                    value => progressValue = value);

                loadedAddressable = await loader.LoadAsync<FakeAddressableAsset>(
                    reference,
                    progress,
                    CancellationToken.None);

                Assert.That(progressValue, Is.EqualTo(1f).Within(0.001f));
            });
        }

        private class TestAssetProvider : ResourceProviderBase
        {
            private readonly FakeAddressableAsset asset;

            public TestAssetProvider(FakeAddressableAsset asset)
            {
                this.asset = asset;
                m_ProviderId = $"{GetType().FullName}.{Guid.NewGuid():N}";
            }

            public override void Provide(ProvideHandle provideHandle)
            {
                provideHandle.Complete(asset, true, null);
            }
        }

        private class TestResourceLocator : IResourceLocator
        {
            private readonly string key;
            private readonly IList<IResourceLocation> locations;

            public string LocatorId { get; }
            public IEnumerable<object> Keys => new object[] { key };
            public IEnumerable<IResourceLocation> AllLocations => locations;

            public TestResourceLocator(
                string locatorId,
                string key,
                string providerId)
            {
                LocatorId = locatorId;
                this.key = key;
                locations = new[]
                {
                    new ResourceLocationBase(
                        key,
                        key,
                        providerId,
                        typeof(FakeAddressableAsset))
                };
            }

            public bool Locate(
                object requestedKey,
                Type type,
                out IList<IResourceLocation> result)
            {
                if (Equals(requestedKey, key) &&
                    (type == null || type.IsAssignableFrom(typeof(FakeAddressableAsset))))
                {
                    result = locations;
                    return true;
                }

                result = null;
                return false;
            }
        }
    }
}
