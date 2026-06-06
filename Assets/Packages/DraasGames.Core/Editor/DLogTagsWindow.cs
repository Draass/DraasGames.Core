#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using DraasGames.Core.Runtime.Infrastructure.Logger;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.DraasGames.Core.Editor
{
    /// <summary>
    /// Standalone editor window for managing DLogger tags: a reorderable, add/remove list bound to
    /// <see cref="DLoggerSettings"/>, plus the generated namespace/path and a Generate button. Each row
    /// is validated live and highlighted when the tag is empty/invalid, reserved, or collapses to an
    /// identifier already used by an earlier tag (all of which the generator would skip). Uses plain
    /// UI Toolkit (no third-party inspector framework).
    /// </summary>
    internal sealed class DLogTagsWindow : EditorWindow
    {
        private static readonly Color IssueRowColor = new(0.65f, 0.35f, 0.1f, 0.25f);
        private static readonly Color ClearColor = new(0f, 0f, 0f, 0f);

        private SerializedObject _serializedObject;
        private SerializedProperty _tagsProperty;
        private ListView _list;
        private Texture _warnIcon;
        private string[] _issues = Array.Empty<string>();

        [MenuItem("DraasGames/Logger/Tags Editor")]
        public static void Open()
        {
            var window = GetWindow<DLogTagsWindow>();
            window.titleContent = new GUIContent("DLogger Tags");
            window.minSize = new Vector2(320f, 260f);
            window.Show();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.paddingLeft = 8;
            root.style.paddingRight = 8;
            root.style.paddingTop = 8;
            root.style.paddingBottom = 8;

            var settings = GetOrCreateSettings();
            if (settings == null)
            {
                root.Add(new HelpBox("Unable to load or create DLoggerSettings.", HelpBoxMessageType.Error));
                return;
            }

            _serializedObject = new SerializedObject(settings);
            _tagsProperty = _serializedObject.FindProperty("_tags");
            _warnIcon = EditorGUIUtility.IconContent("console.warnicon.sml")?.image;

            var title = new Label("DLogger Tags");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 4;
            root.Add(title);

            root.Add(new HelpBox(
                "Edit tags, then Generate to (re)build the DLogTags constants class for compile-safe usage:\n" +
                "DLogger.Log(\"msg\", this, DLogTags.UI);\n" +
                "Highlighted rows are empty, reserved (\"None\"), or duplicate identifiers — they are skipped.",
                HelpBoxMessageType.Info));

            _list = BuildTagsList();
            root.Add(_list);

            root.Add(new PropertyField(_serializedObject.FindProperty("_generatedTagsNamespace"), "Namespace"));
            root.Add(new PropertyField(_serializedObject.FindProperty("_generatedTagsPath"), "Output Path"));

            var generate = new Button(OnGenerateClicked) { text = "Generate Tags" };
            generate.style.height = 24;
            generate.style.marginTop = 6;
            root.Add(generate);

            _issues = ComputeIssues();
            root.Bind(_serializedObject);

            // Re-validate every row whenever any tag, the size, or the order changes.
            _list.TrackSerializedObjectValue(_serializedObject, _ => Revalidate());
        }

        private ListView BuildTagsList()
        {
            var list = new ListView
            {
                bindingPath = "_tags",
                headerTitle = "Tags",
                showFoldoutHeader = true,
                showBoundCollectionSize = true,
                showAddRemoveFooter = true,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = MakeTagRow,
                bindItem = BindTagRow
            };
            list.style.flexGrow = 1;
            list.style.minHeight = 120;
            return list;
        }

        private VisualElement MakeTagRow()
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.paddingLeft = 2;
            row.style.paddingRight = 2;

            var field = new TextField { name = "field" };
            field.style.flexGrow = 1;
            row.Add(field);

            var warn = new Image { name = "warn", scaleMode = ScaleMode.ScaleToFit };
            warn.style.width = 16;
            warn.style.height = 16;
            warn.style.marginLeft = 4;
            warn.style.flexShrink = 0;
            warn.image = _warnIcon;
            warn.style.display = DisplayStyle.None;
            row.Add(warn);

            return row;
        }

        private void BindTagRow(VisualElement element, int index)
        {
            var field = element.Q<TextField>("field");
            if (field != null && index >= 0 && index < _tagsProperty.arraySize)
            {
                field.BindProperty(_tagsProperty.GetArrayElementAtIndex(index));
            }

            var issue = index >= 0 && index < _issues.Length ? _issues[index] : null;
            var warn = element.Q<Image>("warn");
            if (warn != null)
            {
                warn.tooltip = issue ?? string.Empty;
                warn.style.display = string.IsNullOrEmpty(issue) ? DisplayStyle.None : DisplayStyle.Flex;
            }

            element.tooltip = issue ?? string.Empty;
            element.style.backgroundColor = string.IsNullOrEmpty(issue) ? ClearColor : IssueRowColor;
        }

        private void Revalidate()
        {
            _issues = ComputeIssues();
            _list?.RefreshItems();
        }

        private string[] ComputeIssues()
        {
            if (_tagsProperty == null)
            {
                return Array.Empty<string>();
            }

            var count = _tagsProperty.arraySize;
            var issues = new string[count];
            var usedIdentifiers = new HashSet<string>(StringComparer.Ordinal);

            for (var i = 0; i < count; i++)
            {
                var tag = _tagsProperty.GetArrayElementAtIndex(i).stringValue;

                if (DLogTagsGenerator.IsReservedTagName(tag))
                {
                    issues[i] = "Reserved name \"None\" — will be skipped.";
                    continue;
                }

                var identifier = DLogTagsGenerator.ToIdentifier(tag);
                if (identifier == null)
                {
                    issues[i] = "Empty or invalid tag — will be skipped.";
                    continue;
                }

                if (!usedIdentifiers.Add(identifier))
                {
                    issues[i] = $"Collapses to identifier '{identifier}', already used — will be skipped.";
                    continue;
                }

                issues[i] = null;
            }

            return issues;
        }

        private void OnGenerateClicked()
        {
            _serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_serializedObject.targetObject);
            AssetDatabase.SaveAssets();
            DLogTagsGenerator.GenerateFromSettings();
        }

        private static DLoggerSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<DLoggerSettings>(DLoggerSettings.DefaultAssetPath);
            if (settings != null)
            {
                return settings;
            }

            var directory = Path.GetDirectoryName(DLoggerSettings.DefaultAssetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            settings = CreateInstance<DLoggerSettings>();
            AssetDatabase.CreateAsset(settings, DLoggerSettings.DefaultAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }
    }
}
#endif
