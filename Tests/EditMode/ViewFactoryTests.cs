using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DraasGames;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.DraasGames.Tests.EditMode
{
    [TestFixture]
    public class ViewFactoryTests
    {
        private IInstantiator _mockInstantiator;
        private IViewProviderAsync _mockViewProviderAsync;

        private ViewFactory _viewFactory;

        [SetUp]
        public void Setup()
        {
            _mockInstantiator = Substitute.For<IInstantiator>();
            _mockViewProviderAsync = Substitute.For<IViewProviderAsync>();
            
            _viewFactory = new ViewFactory(_mockInstantiator, _mockViewProviderAsync);
        }

        [Test]
        public async Task Test_CreateGeneric_ShouldCreateView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            _mockViewProviderAsync.GetViewAsync<MyView1>().Returns(UniTask.FromResult(myViewComponent));
            _mockInstantiator.InstantiatePrefabForComponent<MyView1>(myViewComponent).Returns(myViewComponent);

            // Act
            var result =  await _viewFactory.Create<MyView1>();

            // Assert
            Assert.AreEqual(myViewComponent, result);
        }

        [Test]
        public async Task Test_CreateType_ShouldCreateView()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            _mockViewProviderAsync.GetViewAsync(typeof(MyView1)).Returns(UniTask.FromResult(myViewComponent as IView));
            _mockInstantiator.InstantiatePrefabForComponent<IView>(myViewComponent).Returns(myViewComponent);

            // Act
            var result =  await _viewFactory.Create(typeof(MyView1)) as MyView1;

            // Assert
            Assert.AreEqual(myViewComponent, result);
        }

        [Test]
        public async Task Test_CreateType_ShouldThrowErrorIfNotInheritedFromIViewAndMonoBehaviour()
        {
            // Arrange
            var prefabGameObject = new GameObject("MyViewPrefab");
            var myViewComponent = prefabGameObject.AddComponent<MyView1>();
            _mockViewProviderAsync.GetViewAsync(typeof(MyView1)).Returns(UniTask.FromResult(myViewComponent as IView));
            _mockInstantiator.InstantiatePrefabForComponent<IView>(myViewComponent).Returns(myViewComponent);

            try
            {
                // Act
                var result =  await _viewFactory.Create(typeof(ScrollRect)) as MyView1;  
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual(typeof(ArgumentException), e.GetType());
                Assert.True(true);
            }
        }
    }
}