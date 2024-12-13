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
            _viewFactoryMock.Create(typeof(MyView1)).Returns(UniTask.FromResult(myViewComponent as IView));
            
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
            _viewFactoryMock.Create(typeof(MyView1)).Returns(UniTask.FromResult(myViewComponent as IView));
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
            _viewFactoryMock.Create(typeof(MyView1)).Returns(UniTask.FromResult(myViewComponent as IView));
            
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
}