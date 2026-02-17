using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace _Project.Scripts.DraasGames.Tests.EditMode
{
    [TestFixture]
    public class ViewRouterSpamClickTests
    {
        [Test]
        public async Task SpamClick_ShowAsync_SameView_ShouldCreateOnce_AndNotThrow()
        {
            var viewFactoryMock = Substitute.For<IViewFactory>();
            var viewRouter = new ViewRouter(viewFactoryMock);

            var createGate = new UniTaskCompletionSource();
            var createdObjects = new List<GameObject>();
            var createCallCount = 0;

            async UniTask<SpamClickViewA> CreateViewDelayed()
            {
                Interlocked.Increment(ref createCallCount);
                await createGate.Task;
                var go = new GameObject($"SpamClickViewA-{createCallCount}");
                createdObjects.Add(go);
                return go.AddComponent<SpamClickViewA>();
            }

            viewFactoryMock.Create<SpamClickViewA>().Returns(_ => CreateViewDelayed());

            // Start a bunch of concurrent "open view" requests like rapid button clicks.
            var tasks = new Task<SpamClickViewA>[20];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = viewRouter.ShowAsync<SpamClickViewA>().AsTask();
            }

            // Allow the first creation to complete.
            createGate.TrySetResult();

            SpamClickViewA[] results = null;
            try
            {
                results = await Task.WhenAll(tasks);
            }
            finally
            {
                // Cleanup: destroy whatever got created, even if the test fails.
                foreach (var go in createdObjects)
                {
                    if (go != null)
                    {
                        UnityEngine.Object.DestroyImmediate(go);
                    }
                }
            }

            Assert.AreEqual(1, createCallCount, "Factory.Create should be called only once for the same view type.");
            Assert.IsTrue(viewRouter.ActiveViews.ContainsKey(typeof(SpamClickViewA)), "View should be active after spam clicking.");

            // All calls should return the same active instance.
            for (var i = 1; i < results.Length; i++)
            {
                Assert.AreSame(results[0], results[i], "All concurrent ShowAsync calls should resolve to the same view instance.");
            }
        }

        [Test]
        public async Task SpamClick_ShowAsync_TwoDifferentViews_ShouldNotLeaveBothActive()
        {
            var viewFactoryMock = Substitute.For<IViewFactory>();
            var viewRouter = new ViewRouter(viewFactoryMock);

            var createGateA = new UniTaskCompletionSource();
            var createGateB = new UniTaskCompletionSource();
            var createdObjects = new List<GameObject>();

            async UniTask<SpamClickViewA> CreateA()
            {
                await createGateA.Task;
                var go = new GameObject("SpamClickViewA");
                createdObjects.Add(go);
                return go.AddComponent<SpamClickViewA>();
            }

            async UniTask<SpamClickViewB> CreateB()
            {
                await createGateB.Task;
                var go = new GameObject("SpamClickViewB");
                createdObjects.Add(go);
                return go.AddComponent<SpamClickViewB>();
            }

            viewFactoryMock.Create<SpamClickViewA>().Returns(_ => CreateA());
            viewFactoryMock.Create<SpamClickViewB>().Returns(_ => CreateB());

            var showATask = viewRouter.ShowAsync<SpamClickViewA>().AsTask();
            var showBTask = viewRouter.ShowAsync<SpamClickViewB>().AsTask();

            // Let A complete first, then B. With proper serialization, B should end up being the only active regular view.
            createGateA.TrySetResult();
            await showATask;

            createGateB.TrySetResult();
            await showBTask;

            try
            {
                Assert.IsFalse(viewRouter.ActiveViews.ContainsKey(typeof(SpamClickViewA)),
                    "First view should not remain active after a second regular view is shown.");
                Assert.IsTrue(viewRouter.ActiveViews.ContainsKey(typeof(SpamClickViewB)),
                    "Second view should be active after being shown.");
            }
            finally
            {
                foreach (var go in createdObjects)
                {
                    if (go != null)
                    {
                        UnityEngine.Object.DestroyImmediate(go);
                    }
                }
            }
        }

        [Test]
        public async Task SpamClick_Show_VoidApi_ShouldNotPublishUnobservedExceptions()
        {
            var viewFactoryMock = Substitute.For<IViewFactory>();
            var viewRouter = new ViewRouter(viewFactoryMock);

            var createGate = new UniTaskCompletionSource();
            var createdObjects = new List<GameObject>();

            async UniTask<SpamClickViewA> CreateViewDelayed()
            {
                await createGate.Task;
                var go = new GameObject("SpamClickViewA");
                createdObjects.Add(go);
                return go.AddComponent<SpamClickViewA>();
            }

            viewFactoryMock.Create<SpamClickViewA>().Returns(_ => CreateViewDelayed());

            Exception unobserved = null;
            void Handler(Exception ex)
            {
                if (unobserved == null)
                {
                    unobserved = ex;
                }
            }

            UniTaskScheduler.UnobservedTaskException += Handler;
            try
            {
                for (var i = 0; i < 20; i++)
                {
                    viewRouter.Show<SpamClickViewA>();
                }

                createGate.TrySetResult();

                // Let scheduled continuations run.
                for (var i = 0; i < 50; i++)
                {
                    await UniTask.Yield();
                }
            }
            finally
            {
                UniTaskScheduler.UnobservedTaskException -= Handler;

                foreach (var go in createdObjects)
                {
                    if (go != null)
                    {
                        UnityEngine.Object.DestroyImmediate(go);
                    }
                }
            }

            Assert.IsNull(unobserved, $"No unobserved exceptions should be published, but got: {unobserved}");
            Assert.IsTrue(viewRouter.ActiveViews.ContainsKey(typeof(SpamClickViewA)));
        }

        public sealed class SpamClickViewA : View
        {
        }

        public sealed class SpamClickViewB : View
        {
        }
    }
}
