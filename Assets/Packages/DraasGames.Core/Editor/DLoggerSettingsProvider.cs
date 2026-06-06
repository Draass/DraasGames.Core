#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Packages.DraasGames.Core.Editor
{
    internal static class DLoggerSettingsProvider
    {
        private const string SettingsPath = "Project/DraasGames";
        private const string SettingsAssetPath = "Assets/Resources/DraasGames/DLoggerSettings.asset";
        private const string SettingsTypeName = "DraasGames.Core.Runtime.Infrastructure.Logger.DLoggerSettings, DraasGames.Core";
        private const string LoggerTypeName = "DraasGames.Core.Runtime.Infrastructure.Logger.DLogger, DraasGames.Core";

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider(SettingsPath, SettingsScope.Project)
            {
                label = "DraasGames",
                guiHandler = DrawSettings,
                keywords = new HashSet<string>
                {
                    "draasgames",
                    "logger",
                    "logging",
                    "log level",
                    "minimum level",
                    "dlogger"
                }
            };
        }

        private static void DrawSettings(string searchContext)
        {
            DrawLoggerSettingsSection();
        }

        private static void DrawLoggerSettingsSection()
        {
            var settings = GetOrCreateSettingsAsset();
            if (settings == null)
            {
                EditorGUILayout.HelpBox("Unable to load DLogger settings asset.", MessageType.Error);
                return;
            }

            var serializedObject = new SerializedObject(settings);
            var minimumLevelProperty = serializedObject.FindProperty("_minimumLevel");

            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Logger", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Stores the logger configuration in the project at Assets/Resources/DraasGames/DLoggerSettings.asset so the package can be reused across projects.",
                    MessageType.Info);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(minimumLevelProperty, new GUIContent("Minimum Level"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssets();
                    ReloadLoggerSettings();
                }
            }

            DrawTagsSection(settings, serializedObject);
        }

        private static void DrawTagsSection(UnityEngine.Object settings, SerializedObject serializedObject)
        {
            var tagsProperty = serializedObject.FindProperty("_tags");
            var namespaceProperty = serializedObject.FindProperty("_generatedTagsNamespace");
            var pathProperty = serializedObject.FindProperty("_generatedTagsPath");
            if (tagsProperty == null)
            {
                return;
            }

            EditorGUILayout.Space(6f);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Define tag names, then generate the DLogTags constants class for compile-safe usage:\n" +
                    "DLogger.Log(\"msg\", this, DLogTags.UI);",
                    MessageType.Info);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(tagsProperty, new GUIContent("Tags"), true);
                EditorGUILayout.PropertyField(namespaceProperty, new GUIContent("Namespace"));
                EditorGUILayout.PropertyField(pathProperty, new GUIContent("Output Path"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssets();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Open Tags Editor"))
                    {
                        DLogTagsWindow.Open();
                    }

                    if (GUILayout.Button("Generate Tags"))
                    {
                        DLogTagsGenerator.GenerateFromSettings();
                    }
                }
            }
        }

        private static UnityEngine.Object GetOrCreateSettingsAsset()
        {
            var settingsType = Type.GetType(SettingsTypeName);
            if (settingsType == null)
            {
                return null;
            }

            var settings = AssetDatabase.LoadAssetAtPath(SettingsAssetPath, settingsType);
            if (settings != null)
            {
                return settings;
            }

            var directory = Path.GetDirectoryName(SettingsAssetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var instance = ScriptableObject.CreateInstance(settingsType);
            AssetDatabase.CreateAsset(instance, SettingsAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ReloadLoggerSettings();
            return instance;
        }

        private static void ReloadLoggerSettings()
        {
            var loggerType = Type.GetType(LoggerTypeName);
            var reloadMethod = loggerType?.GetMethod("ReloadSettings", BindingFlags.Public | BindingFlags.Static);
            reloadMethod?.Invoke(null, null);
        }
    }
}
#endif
