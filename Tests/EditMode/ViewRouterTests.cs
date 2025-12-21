using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DraasGames;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace _Project.Scripts.DraasGames.Tests.EditMode
{
    [TestFixture]
    public class ViewRouterTests
    {
        private IViewFactory _viewFactoryMock;
        private ViewRouter _viewRouter;
        
        [SetUp]
        public void SetUp()
        {
            _viewFactoryMock = Substitute.For<IViewFactory>();
            _viewRouter = new ViewRouter(_viewFactoryMock);
        }
        
        [Test]
        public async Task Test_Show_ShouldDisplayView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            //myViewComponent.IsVisible = false; // Initial state

            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));

            // Subscribe to the OnViewShown event
            Type shownViewType = null;
            _viewRouter.OnViewShown += (type) => shownViewType = type;

            // Act
            _viewRouter.Show<MyView1>();

            // Assert
            //Assert.IsTrue(myViewComponent.IsVisible, "MyView should be visible after Show.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView.");
            //Assert.AreEqual(typeof(MyView), shownViewType, "OnViewShown should be triggered with MyView.");

            // Clean up
            UnityEngine.Object.DestroyImmediate(prefabGameObject);
        }
        
        [Test]
        public async Task Test_Show_MultipleShowShouldStillBeOnlySingleActiveView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            //myViewComponent.IsVisible = false; // Initial state

            var anotherGameObject = new GameObject("AnotherPrefab");
            var anotherViewComponent = anotherGameObject.AddComponent<View1>();

            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            _viewFactoryMock.Create<View1>().Returns(UniTask.FromResult(anotherViewComponent));

            // Subscribe to the OnViewShown event
            Type shownViewType = null;
            _viewRouter.OnViewShown += (type) => shownViewType = type;

            // Act
            _viewRouter.Show<MyView1>();
            _viewRouter.Show<View1>();

            // Assert
            //Assert.IsTrue(myViewComponent.IsVisible, "MyView should be visible after Show.");
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should not contain MyView.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(View1)), "ActiveViews should contain View1.");
            //Assert.AreEqual(typeof(MyView), shownViewType, "OnViewShown should be triggered with MyView.");

            // Clean up
            UnityEngine.Object.DestroyImmediate(prefabGameObject);
            UnityEngine.Object.DestroyImmediate(anotherGameObject);
        }

        [Test]
        public async Task Test_Hide_ShouldHideView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            //myViewComponent.IsVisible = false; // Initial state

            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));

            // Subscribe to the OnViewShown event
            Type shownViewType = null;
            _viewRouter.OnViewShown += (type) => shownViewType = type;

            // Act
            _viewRouter.Show<MyView1>();

            _viewRouter.Hide<MyView1>();
            // Assert
            //Assert.IsTrue(myViewComponent.IsVisible, "MyView should be visible after Show.");
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should not contain MyView.");
            //Assert.AreEqual(typeof(MyView), shownViewType, "OnViewShown should be triggered with MyView.");

            // Clean up
            UnityEngine.Object.DestroyImmediate(prefabGameObject);
        }

        [Test]
        public async Task Test_ShowModal_ShouldShowModalView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            //myViewComponent.IsVisible = false; // Initial state

            var modalGameObject = new GameObject("ModalPrefab");
            var modalViewComponent = modalGameObject.AddComponent<MyModalView>();
            
            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            _viewFactoryMock.Create<MyModalView>().Returns(UniTask.FromResult(modalViewComponent));
            
            
            // Subscribe to the OnViewShown event
            Type shownViewType = null;
            _viewRouter.OnViewShown += (type) => shownViewType = type;

            // Act
            _viewRouter.Show<MyView1>();
            _viewRouter.ShowModal<MyModalView>();
            // Assert
            //Assert.IsTrue(myViewComponent.IsVisible, "MyView should be visible after Show.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyModalView)), "ActiveViews should contain MyModalView.");
            //Assert.AreEqual(typeof(MyView), shownViewType, "OnViewShown should be triggered with MyView.");

            // Clean up
            UnityEngine.Object.DestroyImmediate(prefabGameObject);
        }

        [Test]
        public async Task Test_ShowModalMultiple_WithShowShouldBeMultipleActiveViews()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            //myViewComponent.IsVisible = false; // Initial state

            var modalGameObject = new GameObject("ModalPrefab");
            var modalViewComponent = modalGameObject.AddComponent<MyModalView>();
            
            var modalGameObject2 = new GameObject("ModalPrefab");
            var modalViewComponent2 = modalGameObject2.AddComponent<ModalView2>();
            
            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            _viewFactoryMock.Create<MyModalView>().Returns(UniTask.FromResult(modalViewComponent));
            _viewFactoryMock.Create<ModalView2>().Returns(UniTask.FromResult(modalViewComponent2));
            
            // Act
            _viewRouter.Show<MyView1>();
            _viewRouter.ShowModal<MyModalView>(false);
            _viewRouter.ShowModal<ModalView2>(false);
            
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyModalView)), "ActiveViews should contain MyModalView.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(ModalView2)), "ActiveViews should contain ModalView2.");
        }
        
        [Test]
        public async Task Test_ShowModal_WithCloseOtherModals_ShoulBeOnly1ModalView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            //myViewComponent.IsVisible = false; // Initial state

            var modalGameObject = new GameObject("ModalPrefab");
            var modalViewComponent = modalGameObject.AddComponent<MyModalView>();
            
            var modalGameObject2 = new GameObject("ModalPrefab");
            var modalViewComponent2 = modalGameObject2.AddComponent<ModalView2>();
            
            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            _viewFactoryMock.Create<MyModalView>().Returns(UniTask.FromResult(modalViewComponent));
            _viewFactoryMock.Create<ModalView2>().Returns(UniTask.FromResult(modalViewComponent2));
            
            // Act
            _viewRouter.Show<MyView1>();
            _viewRouter.ShowModal<MyModalView>();
            _viewRouter.ShowModal<ModalView2>();
            
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView.");
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(MyModalView)), "ActiveViews should not contain MyModalView.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(ModalView2)), "ActiveViews should contain ModalView2.");
        }

        [Test]
        public async Task Test_ShowPersistent_ShouldBeActiveAfterShow()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            //myViewComponent.IsVisible = false; // Initial state

            var modalGameObject = new GameObject("ModalPrefab");
            var modalViewComponent = modalGameObject.AddComponent<MyModalView>();
            
            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            _viewFactoryMock.Create<MyModalView>().Returns(UniTask.FromResult(modalViewComponent));
            
            // Act
            _viewRouter.ShowPersistent<MyView1>();
            _viewRouter.Show<MyModalView>();
            
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyModalView)), "ActiveViews should contain MyModalView.");
        }
        
        [Test]
        public async Task Test_ShowPersistent_ShouldBeActiveAfterShowDefault()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            
            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            
            // Act
            _viewRouter.ShowPersistent<MyView1>();
            
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView.");
        }
        
        [Test]
        public async Task Test_ShowMultiplePersistentViews_ShouldAllBeActive()
        {
            // Arrange
            var prefabGameObject1 = new GameObject("View1");
            var prefabGameObject2 = new GameObject("View2");

            var viewComponent1 = prefabGameObject1.AddComponent<View1>();
            var viewComponent2 = prefabGameObject2.AddComponent<View2>();

            _viewFactoryMock.Create<View1>().Returns(UniTask.FromResult(viewComponent1));
            _viewFactoryMock.Create<View2>().Returns(UniTask.FromResult(viewComponent2));

            // Act
            _viewRouter.ShowPersistent<View1>();
            _viewRouter.ShowPersistent<View2>();

            // Assert
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(View1)), "ActiveViews should contain View1.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(View2)), "ActiveViews should contain View2.");
        }

        [Test]
        public async Task Test_Return_ShouldNotReturnIfNoPreviousView()
        {
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            
            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            _viewFactoryMock.Create(typeof(MyView1)).Returns(UniTask.FromResult(myViewComponent as IViewBase));
            
            // Act
            _viewRouter.Show<MyView1>();
            _viewRouter.Return();
            
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView.");
        }
        
        [Test]
        public void Test_Return_ShouldNotCrashOnEmptyStack()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _viewRouter.Return(), "Return should not throw exceptions when the stack is empty.");
        }
        
        [Test]
        public async Task Test_Return_ShouldNotCloseModalsIfNotCloseSpecified()
        {
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            
            var modalGameObject = new GameObject("ModalPrefab");
            var modalViewComponent = modalGameObject.AddComponent<MyModalView>();
            
            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            _viewFactoryMock.Create(typeof(MyView1)).Returns(UniTask.FromResult(myViewComponent as IViewBase));
            _viewFactoryMock.Create<MyModalView>().Returns(UniTask.FromResult(modalViewComponent));
            
            // Act
            _viewRouter.Show<MyView1>();
            _viewRouter.Return(false);
            
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView.");
        }
        
        [Test]
        public async Task Test_Return_ShouldReturnToPreviousView()
        {
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            //myViewComponent.IsVisible = false; // Initial state

            var modalGameObject = new GameObject("ModalPrefab");
            var modalViewComponent = modalGameObject.AddComponent<MyModalView>();
            
            // Mock the factory to return the real MyView component
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            _viewFactoryMock.Create<MyModalView>().Returns(UniTask.FromResult(modalViewComponent));
            
            // Act
            _viewRouter.Show<MyView1>();
            _viewRouter.Show<MyModalView>();
            
            // Recreate MyView as it got destroyed after showing MyModalView
            prefabGameObject = new GameObject("MyViewPrefab");
            myViewComponent = prefabGameObject.AddComponent<MyView1>();
            _viewFactoryMock.Create(typeof(MyView1)).Returns(UniTask.FromResult(myViewComponent as IViewBase));
            
            _viewRouter.Return();
            
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView.");
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(MyModalView)), "ActiveViews should not contain MyModalView.");
        }
        
        [Test]
        public void Test_Hide_NotCached_ShouldDestroyView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            
            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            
            // Act
            _viewRouter.Show<MyView1>();
            _viewRouter.Hide<MyView1>();
            
            Assert.IsTrue(prefabGameObject == null);
        }

        [Test]
        public void Test_HideAllModal_ShouldDestroyView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();

            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));

            // Act
            _viewRouter.ShowModal<MyView1>();
            _viewRouter.HideAllModalViews();

            Assert.IsTrue(prefabGameObject == null);
        }
        
        [Test]
        public void Test_Destroy_NullView_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _viewRouter.Hide<MyView1>(), "Destroy should handle null references gracefully.");
        }

        [Test]
        public async Task Test_HideAsync_ShouldHideView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();

            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(myViewComponent));

            Type hiddenViewType = null;
            _viewRouter.OnViewHidden += (type) => hiddenViewType = type;

            // Act
            await _viewRouter.ShowAsync<MyView1>();
            await _viewRouter.HideAsync<MyView1>();

            // Assert
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should not contain MyView after HideAsync.");
            Assert.AreEqual(typeof(MyView1), hiddenViewType, "OnViewHidden should be triggered with MyView.");

            // Clean up
            if (prefabGameObject != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject);
        }

        [Test]
        public async Task Test_HideAllModalViewsAsync_ShouldHideAllModals()
        {
            // Arrange
            var modalGameObject1 = new GameObject("Modal1");
            var modalGameObject2 = new GameObject("Modal2");
            var modalViewComponent1 = modalGameObject1.AddComponent<MyModalView>();
            var modalViewComponent2 = modalGameObject2.AddComponent<ModalView2>();

            _viewFactoryMock.Create<MyModalView>().Returns(UniTask.FromResult(modalViewComponent1));
            _viewFactoryMock.Create<ModalView2>().Returns(UniTask.FromResult(modalViewComponent2));

            // Act
            await _viewRouter.ShowModalAsync<MyModalView>(false);
            await _viewRouter.ShowModalAsync<ModalView2>(false);
            await _viewRouter.HideAllModalViewsAsync();

            // Assert
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(MyModalView)), "ActiveViews should not contain MyModalView after HideAllModalViewsAsync.");
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(ModalView2)), "ActiveViews should not contain ModalView2 after HideAllModalViewsAsync.");

            // Clean up
            if (modalGameObject1 != null)
                UnityEngine.Object.DestroyImmediate(modalGameObject1);
            if (modalGameObject2 != null)
                UnityEngine.Object.DestroyImmediate(modalGameObject2);
        }

        [Test]
        public async Task Test_ReturnModalAsync_ShouldReturnToPreviousModal()
        {
            // Arrange
            var modalGameObject1 = new GameObject("Modal1");
            var modalGameObject2 = new GameObject("Modal2");
            var modalViewComponent1 = modalGameObject1.AddComponent<MyModalView>();
            var modalViewComponent2 = modalGameObject2.AddComponent<ModalView2>();

            _viewFactoryMock.Create<MyModalView>().Returns(UniTask.FromResult(modalViewComponent1));
            _viewFactoryMock.Create<ModalView2>().Returns(UniTask.FromResult(modalViewComponent2));

            // Act
            await _viewRouter.ShowModalAsync<MyModalView>();
            await _viewRouter.ShowModalAsync<ModalView2>(false);
            await _viewRouter.ReturnModalAsync();

            // Assert
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyModalView)), "ActiveViews should contain MyModalView after ReturnModalAsync.");
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(ModalView2)), "ActiveViews should not contain ModalView2 after ReturnModalAsync.");

            // Clean up
            if (modalGameObject1 != null)
                UnityEngine.Object.DestroyImmediate(modalGameObject1);
            if (modalGameObject2 != null)
                UnityEngine.Object.DestroyImmediate(modalGameObject2);
        }

        [Test]
        public async Task Test_ReturnAsync_ShouldReturnToPreviousView()
        {
            // Arrange
            var prefabGameObject1 = new GameObject("View1");
            var prefabGameObject2 = new GameObject("View2");
            var viewComponent1 = prefabGameObject1.AddComponent<MyView1>();
            var viewComponent2 = prefabGameObject2.AddComponent<View1>();

            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(viewComponent1));
            _viewFactoryMock.Create<View1>().Returns(UniTask.FromResult(viewComponent2));
            _viewFactoryMock.Create(typeof(MyView1)).Returns(UniTask.FromResult(viewComponent1 as IViewBase));

            // Act
            await _viewRouter.ShowAsync<MyView1>();
            await _viewRouter.ShowAsync<View1>();
            
            // Recreate MyView as it got destroyed after showing View1
            prefabGameObject1 = new GameObject("View1Recreated");
            viewComponent1 = prefabGameObject1.AddComponent<MyView1>();
            _viewFactoryMock.Create(typeof(MyView1)).Returns(UniTask.FromResult(viewComponent1 as IViewBase));
            
            await _viewRouter.ReturnAsync();

            // Assert
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView1 after ReturnAsync.");
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(View1)), "ActiveViews should not contain View1 after ReturnAsync.");

            // Clean up
            if (prefabGameObject1 != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject1);
            if (prefabGameObject2 != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject2);
        }

        [Test]
        public async Task Test_ShowAsync_SequentialMode_ShouldWorkAsDefault()
        {
            // Arrange
            var prefabGameObject1 = new GameObject("View1");
            var prefabGameObject2 = new GameObject("View2");
            var viewComponent1 = prefabGameObject1.AddComponent<MyView1>();
            var viewComponent2 = prefabGameObject2.AddComponent<View1>();

            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(viewComponent1));
            _viewFactoryMock.Create<View1>().Returns(UniTask.FromResult(viewComponent2));

            // Act
            await _viewRouter.ShowAsync<MyView1>(ViewTransitionMode.Sequential);
            await _viewRouter.ShowAsync<View1>(ViewTransitionMode.Sequential);

            // Assert
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should not contain MyView1 after sequential transition.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(View1)), "ActiveViews should contain View1 after sequential transition.");

            // Clean up
            if (prefabGameObject1 != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject1);
            if (prefabGameObject2 != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject2);
        }

        [Test]
        public async Task Test_ShowAsync_SimultaneousMode_ShouldHideAndShowSimultaneously()
        {
            // Arrange
            var prefabGameObject1 = new GameObject("View1");
            var prefabGameObject2 = new GameObject("View2");
            var viewComponent1 = prefabGameObject1.AddComponent<MyView1>();
            var viewComponent2 = prefabGameObject2.AddComponent<View1>();

            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(viewComponent1));
            _viewFactoryMock.Create<View1>().Returns(UniTask.FromResult(viewComponent2));

            // Act
            await _viewRouter.ShowAsync<MyView1>();
            await _viewRouter.ShowAsync<View1>(ViewTransitionMode.Simultaneous);

            // Assert
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should not contain MyView1 after simultaneous transition.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(View1)), "ActiveViews should contain View1 after simultaneous transition.");

            // Clean up
            if (prefabGameObject1 != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject1);
            if (prefabGameObject2 != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject2);
        }

        [Test]
        public async Task Test_ShowAsync_DefaultParameter_ShouldUseSequentialMode()
        {
            // Arrange
            var prefabGameObject1 = new GameObject("View1");
            var prefabGameObject2 = new GameObject("View2");
            var viewComponent1 = prefabGameObject1.AddComponent<MyView1>();
            var viewComponent2 = prefabGameObject2.AddComponent<View1>();

            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(viewComponent1));
            _viewFactoryMock.Create<View1>().Returns(UniTask.FromResult(viewComponent2));

            // Act - не указываем параметр, должен использоваться Sequential по умолчанию
            await _viewRouter.ShowAsync<MyView1>();
            await _viewRouter.ShowAsync<View1>();

            // Assert
            Assert.IsFalse(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should not contain MyView1 with default mode.");
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(View1)), "ActiveViews should contain View1 with default mode.");

            // Clean up
            if (prefabGameObject1 != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject1);
            if (prefabGameObject2 != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject2);
        }

        [Test]
        public async Task Test_ShowAsync_SimultaneousMode_WithNoCurrentView_ShouldWork()
        {
            // Arrange
            var prefabGameObject = new GameObject("View1");
            var viewComponent = prefabGameObject.AddComponent<MyView1>();

            _viewFactoryMock.Create<MyView1>().Returns(UniTask.FromResult(viewComponent));

            // Act - показываем первое окно с Simultaneous режимом (не должно быть ошибок)
            await _viewRouter.ShowAsync<MyView1>(ViewTransitionMode.Simultaneous);

            // Assert
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(MyView1)), "ActiveViews should contain MyView1 when shown first with simultaneous mode.");

            // Clean up
            if (prefabGameObject != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject);
        }

        [Test]
        public async Task Test_ShowAsync_WithParam_ShouldPassParam()
        {
            // Arrange
            var prefabGameObject = new GameObject("ParamStringView");
            var viewComponent = prefabGameObject.AddComponent<ParamStringView>();
            _viewFactoryMock.Create<ParamStringView>().Returns(UniTask.FromResult(viewComponent));

            const string expectedParam = "test-param";

            // Act
            await _viewRouter.ShowAsync<ParamStringView, string>(expectedParam);

            // Assert
            Assert.AreEqual(expectedParam, viewComponent.LastParam);

            // Clean up
            if (prefabGameObject != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject);
        }

        [Test]
        public async Task Test_ShowModalAsync_WithParam_ShouldPassParam()
        {
            // Arrange
            var prefabGameObject = new GameObject("ParamModalView");
            var viewComponent = prefabGameObject.AddComponent<ParamModalView>();
            _viewFactoryMock.Create<ParamModalView>().Returns(UniTask.FromResult(viewComponent));

            const string expectedParam = "modal-param";

            // Act
            await _viewRouter.ShowModalAsync<ParamModalView, string>(expectedParam);

            // Assert
            Assert.AreEqual(expectedParam, viewComponent.LastParam);

            // Clean up
            if (prefabGameObject != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject);
        }

        [Test]
        public async Task Test_ShowPersistentAsync_WithParam_ShouldPassParam()
        {
            // Arrange
            var prefabGameObject = new GameObject("ParamPersistentView");
            var viewComponent = prefabGameObject.AddComponent<ParamPersistentView>();
            _viewFactoryMock.Create<ParamPersistentView>().Returns(UniTask.FromResult(viewComponent));

            const string expectedParam = "persistent-param";

            // Act
            await _viewRouter.ShowPersistentAsync<ParamPersistentView, string>(expectedParam);

            // Assert
            Assert.AreEqual(expectedParam, viewComponent.LastParam);

            // Clean up
            if (prefabGameObject != null)
                UnityEngine.Object.DestroyImmediate(prefabGameObject);
        }

        [Test]
        public async Task Test_ReturnAsync_ShouldReplayParamViewParam()
        {
            // Arrange
            var firstParamGameObject = new GameObject("ParamStringView-First");
            var firstParamView = firstParamGameObject.AddComponent<ParamStringView>();
            var secondParamGameObject = new GameObject("ParamStringView-Second");
            var secondParamView = secondParamGameObject.AddComponent<ParamStringView>();
            var viewGameObject = new GameObject("View1");
            var viewComponent = viewGameObject.AddComponent<View1>();

            _viewFactoryMock.Create<ParamStringView>().Returns(UniTask.FromResult(firstParamView));
            _viewFactoryMock.Create(typeof(ParamStringView))
                .Returns(UniTask.FromResult(secondParamView as IViewBase));
            _viewFactoryMock.Create<View1>().Returns(UniTask.FromResult(viewComponent));

            const string expectedParam = "return-param";

            // Act
            await _viewRouter.ShowAsync<ParamStringView, string>(expectedParam);
            await _viewRouter.ShowAsync<View1>();
            await _viewRouter.ReturnAsync();

            // Assert
            Assert.AreEqual(expectedParam, secondParamView.LastParam);
            Assert.IsTrue(_viewRouter.ActiveViews.ContainsKey(typeof(ParamStringView)),
                "ActiveViews should contain ParamStringView after ReturnAsync.");

            // Clean up
            if (firstParamGameObject != null)
                UnityEngine.Object.DestroyImmediate(firstParamGameObject);
            if (secondParamGameObject != null)
                UnityEngine.Object.DestroyImmediate(secondParamGameObject);
            if (viewGameObject != null)
                UnityEngine.Object.DestroyImmediate(viewGameObject);
        }

        [Test]
        public async Task Test_ShowAsync_SimultaneousMode_WithParam_ShouldHideAndShow()
        {
            // Arrange
            var currentGameObject = new GameObject("HideTrackView");
            var currentView = currentGameObject.AddComponent<HideTrackView>();
            var nextGameObject = new GameObject("ParamIntView");
            var nextView = nextGameObject.AddComponent<ParamIntView>();

            _viewFactoryMock.Create<HideTrackView>().Returns(UniTask.FromResult(currentView));
            _viewFactoryMock.Create<ParamIntView>().Returns(UniTask.FromResult(nextView));

            const int expectedParam = 42;

            // Act
            await _viewRouter.ShowAsync<HideTrackView>();
            await _viewRouter.ShowAsync<ParamIntView, int>(expectedParam, ViewTransitionMode.Simultaneous);

            // Assert
            Assert.IsTrue(currentView.HideCalled, "HideAsync should be called for the previous view.");
            Assert.AreEqual(expectedParam, nextView.LastParam);

            // Clean up
            if (currentGameObject != null)
                UnityEngine.Object.DestroyImmediate(currentGameObject);
            if (nextGameObject != null)
                UnityEngine.Object.DestroyImmediate(nextGameObject);
        }
    }

    public class MyModalView : View
    {
    }
        
    public class ModalView2 : View
    {
    }
        
    public class View1 : View
    {
    }
        
    public class View2 : View
    {
    }

    public class ParamStringView : View<string>
    {
        public string LastParam { get; private set; }

        public override UniTask ShowAsync(string param)
        {
            LastParam = param;
            return base.ShowAsync(param);
        }
    }

    public class ParamModalView : View<string>
    {
        public string LastParam { get; private set; }

        public override UniTask ShowAsync(string param)
        {
            LastParam = param;
            return base.ShowAsync(param);
        }
    }

    public class ParamPersistentView : View<string>
    {
        public string LastParam { get; private set; }

        public override UniTask ShowAsync(string param)
        {
            LastParam = param;
            return base.ShowAsync(param);
        }
    }

    public class ParamIntView : View<int>
    {
        public int LastParam { get; private set; }

        public override UniTask ShowAsync(int param)
        {
            LastParam = param;
            return base.ShowAsync(param);
        }
    }

    public class HideTrackView : View
    {
        public bool HideCalled { get; private set; }

        public override UniTask HideAsync()
        {
            HideCalled = true;
            return base.HideAsync();
        }
    }
}
