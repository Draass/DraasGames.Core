using System;
using System.Collections;
using System.Reflection;
using DraasGames.Core.Runtime.Infrastructure.Installers;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.DraasGames.Tests.EditMode
{
    [TestFixture]
    public class AddressablesViewContainerTests
    {
        private const string TempRoot = "Assets/AddressablesViewContainerTests";

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(TempRoot);
        }

        [Test]
        public void OnValidate_AddsQueuedViewsToRegistry()
        {
            var container = ScriptableObject.CreateInstance<AddressablesViewContainer>();
            var prefabReference = CreatePrefabReference<AddressablesViewContainerTestView>("QueuedView");

            AddToPrivateList(container, "_viewsToAdd", prefabReference);

            InvokePrivateMethod(container, "OnValidate");

            var registeredReference = InvokeGetAssetReference(container, typeof(AddressablesViewContainerTestView));
            Assert.NotNull(registeredReference);
            Assert.AreEqual(GetAssetGuid(prefabReference), GetAssetGuid(registeredReference));

            UnityEngine.Object.DestroyImmediate(container);
        }

        [Test]
        public void Container_DoesNotExposeSingleViewReference()
        {
            var singleViewRefField = typeof(AddressablesViewContainer).GetField(
                "_singleViewRef",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNull(singleViewRefField);
        }

        [Test]
        public void Container_DoesNotExposeLegacyMigrationState()
        {
            var legacyEntriesField = typeof(AddressablesViewContainer).GetField(
                "_legacyEntries",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var migrateMethod = typeof(AddressablesViewContainer).GetMethod(
                "TryMigrateLegacyEntries",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsNull(legacyEntriesField);
            Assert.IsNull(migrateMethod);
        }

        private static object CreatePrefabReference<TView>(string prefabName)
            where TView : View
        {
            if (!AssetDatabase.IsValidFolder(TempRoot))
            {
                AssetDatabase.CreateFolder("Assets", nameof(AddressablesViewContainerTests));
            }

            var go = new GameObject(prefabName);
            go.AddComponent<TView>();

            var prefabPath = $"{TempRoot}/{prefabName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            UnityEngine.Object.DestroyImmediate(go);

            var guid = AssetDatabase.AssetPathToGUID(prefabPath);
            var referenceType = Type.GetType(
                "UnityEngine.AddressableAssets.AssetReferenceGameObject, Unity.Addressables",
                true);

            return Activator.CreateInstance(referenceType!, guid);
        }

        private static void AddToPrivateList(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field, $"Field {fieldName} was not found.");

            var list = (IList)field!.GetValue(target);
            if (list == null)
            {
                list = (IList)Activator.CreateInstance(field.FieldType)!;
                field.SetValue(target, list);
            }

            list.Add(value);
        }

        private static void InvokePrivateMethod(object target, string methodName)
        {
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method, $"Method {methodName} was not found.");
            method!.Invoke(target, Array.Empty<object>());
        }

        private static object InvokeGetAssetReference(AddressablesViewContainer container, Type viewType)
        {
            var method = typeof(AddressablesViewContainer).GetMethod(
                nameof(AddressablesViewContainer.GetAssetReference),
                new[] { typeof(Type) });

            Assert.NotNull(method, "GetAssetReference(Type) was not found.");
            return method!.Invoke(container, new object[] { viewType });
        }

        private static string GetAssetGuid(object assetReference)
        {
            var property = assetReference.GetType().GetProperty("AssetGUID");
            Assert.NotNull(property, "AssetGUID property was not found.");
            return (string)property!.GetValue(assetReference);
        }
    }

    public class AddressablesViewContainerTestView : View
    {
    }
}
