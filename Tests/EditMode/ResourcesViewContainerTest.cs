using DraasGames;
using DraasGames.Core.Runtime.UI.Views;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using NSubstitute;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.DraasGames.Tests.EditMode
{
    [TestFixture]
    public class ResourcesViewContainerTest
    {
        [Test]
        public void Test_GetViewPath_ShouldReturnCorrectPathForValidView()
        {
            // Arrange
            var container = ScriptableObject.CreateInstance<ResourcesViewContainer>();
    
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            

            var expectedPath = "path/to/view";
            
            container.AddView(myViewComponent);
            // container._views.Add(myViewComponent);
            // container._viewPaths.Add(expectedPath);
            // container._viewPathsPair.Add(myViewComponent.GetType(), expectedPath);

            // Act
            var result = container.GetViewPath<MyView1>();

            // Assert
            Assert.AreEqual(expectedPath, result, "GetViewPath should return the correct path for a valid view.");
        }
        
        public void Test_OnValidate_ShouldPopulateViewPaths()
        {
            // Arrange
            var container = ScriptableObject.CreateInstance<ResourcesViewContainer>();
            var mockView1 = Substitute.For<IView>();
            var mockView2 = Substitute.For<IView>();

            // container._views.Add(mockView1);
            // container._views.Add(mockView2);

            var mockStrategy = Substitute.For<IViewPathRetrieveStrategy>();
            mockStrategy.RetrieveViewPath(mockView1).Returns("path/to/view1");
            mockStrategy.RetrieveViewPath(mockView2).Returns("path/to/view2");

            // Act
            //container.OnValidate(); // Simulate the editor-only call

            // Assert
            // Assert.AreEqual(2, container._viewPaths.Count, "OnValidate should populate the _viewPaths list.");
            // Assert.AreEqual("path/to/view1", container._viewPaths[0], "OnValidate should correctly map the first view path.");
            // Assert.AreEqual("path/to/view2", container._viewPaths[1], "OnValidate should correctly map the second view path.");
        }
        
        [Test]
        public void Test_EmptyViewsList_ShouldNotThrow()
        {
            // Arrange
            var container = ScriptableObject.CreateInstance<ResourcesViewContainer>();

            // Act & Assert
            //Assert.DoesNotThrow(() => container.OnValidate(), "OnValidate should not throw when _views is empty.");
            //Assert.IsEmpty(container._viewPaths, "_viewPaths should remain empty when no views are present.");
        }
    }

    public class MyView1 : View
    {
    }
}