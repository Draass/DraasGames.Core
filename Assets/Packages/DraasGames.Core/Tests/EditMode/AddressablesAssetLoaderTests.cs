#if DRAASGAMES_ADDRESSABLES_MODULE
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete;
using NSubstitute;
using NUnit.Framework;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Scripts.DraasGames.Tests.EditMode
{
    [TestFixture]
    public class AddressablesAssetLoaderTests
    {
        private AddressablesAssetLoader _loader;

        [SetUp]
        public void SetUp()
        {
            _loader = new AddressablesAssetLoader();
        }

        [TearDown]
        public void TearDown()
        {
            _loader.Dispose();
        }

        [Test]
        public void LifetimeDisposal_ShouldReleaseHandles()
        {
            var lifetime = new Lifetime();
            var handle = default(AsyncOperationHandle<object>);

            InvokeRegisterHandle(lifetime, handle);
            AssertHandlesCount(lifetime, 1);

            lifetime.Dispose();

            AssertHandlesCount(lifetime, 0);
        }

        [Test]
        public void LoaderDispose_ShouldDisposeAllLifetimes()
        {
            var lifetime1 = NSubstitute.Substitute.For<ILifetime>();
            var lifetime2 = NSubstitute.Substitute.For<ILifetime>();

            var handle = default(AsyncOperationHandle<object>);

            InvokeRegisterHandleSubstitute(lifetime1, handle);
            InvokeRegisterHandleSubstitute(lifetime2, handle);

            _loader.Dispose();

            lifetime1.Received(1).Dispose();
            lifetime2.Received(1).Dispose();
        }

        [Test]
        public void TrackAwaitWithProgress_NullLifetime_UsesProjectLifetime()
        {
            var handle = default(AsyncOperationHandle<object>);
            InvokeTrackAwaitWithProgress(null, handle);

            AssertHandlesCount(ProjectLifetimeHolder.ProjectLifeTime, 1);
        }

        [Test]
        public void LoadAsync_Key_PublicMethod_CallsInternalTracking()
        {
            var lifetime = new Lifetime();
            const string key = "test";

            _loader.LoadAsync<object>(key, lifetime);

            AssertHandlesCount(lifetime, 1);
        }

        [Test]
        public void LoadAsync_Reference_PublicMethod_CallsInternalTracking()
        {
            var lifetime = new Lifetime();
            AssetReference reference = null;

            _loader.LoadAsync<object>(reference, lifetime);

            AssertHandlesCount(lifetime, 1);
        }

        [Test]
        public void LoadWithComponentAsync_Key_PublicMethod_CallsInternalTracking()
        {
            var lifetime = new Lifetime();
            const string key = "test";

            _loader.LoadWithComponentAsync<Component>(key, lifetime);

            AssertHandlesCount(lifetime, 1);
        }

        [Test]
        public void LoadWithComponentAsync_Reference_PublicMethod_CallsInternalTracking()
        {
            var lifetime = new Lifetime();
            AssetReference reference = null;

            _loader.LoadWithComponentAsync<Component>(reference, lifetime);

            AssertHandlesCount(lifetime, 1);
        }

        private void InvokeRegisterHandle(ILifetime lt, AsyncOperationHandle<object> handle)
        {
            var method = typeof(AddressablesAssetLoader).GetMethod("RegisterHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            method = method.MakeGenericMethod(typeof(object));
            method.Invoke(_loader, new object[] { lt, handle });
        }

        private void InvokeRegisterHandleSubstitute(ILifetime lt, AsyncOperationHandle<object> handle)
        {
            var method = typeof(AddressablesAssetLoader).GetMethod("RegisterHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            method = method.MakeGenericMethod(typeof(object));
            method.Invoke(_loader, new object[] { lt, handle });
        }

        private void InvokeTrackAwaitWithProgress(ILifetime lt, AsyncOperationHandle<object> handle)
        {
            var method = typeof(AddressablesAssetLoader).GetMethod("TrackAwaitWithProgress", BindingFlags.NonPublic | BindingFlags.Instance);
            method = method.MakeGenericMethod(typeof(object));
            method.Invoke(_loader, new object[] { lt, handle, null });
        }

        private void AssertHandlesCount(ILifetime lt, int expected)
        {
            var field = typeof(AddressablesAssetLoader).GetField("_handles", BindingFlags.NonPublic | BindingFlags.Instance);
            var dict = (Dictionary<ILifetime, List<AsyncOperationHandle>>)field.GetValue(_loader);
            dict.TryGetValue(lt, out var list);
            var count = list?.Count ?? 0;
            Assert.AreEqual(expected, count);
        }
    }
}
#endif
