using System;
using System.Collections.Generic;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    [CreateAssetMenu(menuName = "DraasGames/UI/AddressablesViewContainer", fileName = "AddressablesViewContainer", order = 0)]
    public class AddressablesViewContainer : SerializedScriptableObject, IViewAssetReferenceProvider
    {
        [Serializable]
        public class ViewEntry
        {
            [OdinSerialize, LabelText("View Type"), HorizontalGroup("Split"), VerticalGroup("Split/Left"), PropertyOrder(0)]
            public Type ViewType;

            [OdinSerialize, LabelText("Prefab"), VerticalGroup("Split/Right"), PropertyOrder(1)]
            public AssetReferenceGameObject Prefab;
        }

        [Title("Entries", null, TitleAlignments.Left, true, true)]
        [OdinSerialize, BoxGroup("Entries")]
        private List<ViewEntry> _entries = new();

        // Legacy backing store kept for one-time migration from the old dictionary-based format.
        // Using FormerlySerializedAs to pick up previously serialized data named "_entries".
        [FormerlySerializedAs("_entries")]
        [OdinSerialize, HideInInspector]
        private Dictionary<Type, AssetReferenceGameObject> _legacyEntries = new();

        private Dictionary<Type, AssetReference> _viewMap;

        private void OnEnable()
        {
            TryMigrateLegacyEntries();
            RebuildMap();
        }

        public AssetReference GetAssetReference(Type type)
        {
            if (_viewMap == null)
            {
                RebuildMap();
            }

            if (type == null || _viewMap == null)
            {
                return null;
            }

            if (_viewMap.TryGetValue(type, out var exact))
            {
                return exact;
            }

            // Fallback: find the most specific assignable type (nearest ancestor/interface)
            AssetReference bestRef = null;
            int bestDepth = -1;
            foreach (var kv in _viewMap)
            {
                var keyType = kv.Key;
                var aref = kv.Value;
                if (keyType == null || aref == null) continue;
                if (keyType.IsAssignableFrom(type))
                {
                    var depth = GetTypeDepth(keyType);
                    if (depth > bestDepth)
                    {
                        bestDepth = depth;
                        bestRef = aref;
                    }
                }
            }

            return bestRef;
        }

        private void RebuildMap()
        {
            _viewMap = new Dictionary<Type, AssetReference>();

            if (_entries == null) return;
            for (int i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (e == null || e.ViewType == null || e.Prefab == null) continue;
                _viewMap[e.ViewType] = e.Prefab;
            }
        }

        private static int GetTypeDepth(Type t)
        {
            int depth = 0;
            var cur = t;
            while (cur != null)
            {
                depth++;
                cur = cur.BaseType;
            }
            return depth;
        }

        private void TryMigrateLegacyEntries()
        {
            if (_legacyEntries == null || _legacyEntries.Count == 0)
            {
                return;
            }

            if (_entries == null || _entries.Count == 0)
            {
                foreach (var kv in _legacyEntries)
                {
                    if (kv.Key == null || kv.Value == null) continue;
                    _entries.Add(new ViewEntry { ViewType = kv.Key, Prefab = kv.Value });
                }
#if UNITY_EDITOR
                _legacyEntries.Clear();
                EditorUtility.SetDirty(this);
#endif
            }
        }

#if UNITY_EDITOR
        [OnInspectorGUI, PropertyOrder(-100)]
        private void DrawHeaderAndToolbar()
        {
            // Header with icon and subtitle
            var rect = GUILayoutUtility.GetRect(0, 36, GUILayout.ExpandWidth(true));
            var icon = EditorGUIUtility.IconContent("Prefab Icon").image as Texture;
            if (icon != null)
            {
                var iconRect = new Rect(rect.x + 6, rect.y + 6, 20, 20);
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
            }
            var titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };
            var subStyle = new GUIStyle(EditorStyles.miniLabel);
            GUI.Label(new Rect(rect.x + 32, rect.y + 6, rect.width - 38, 18), "Addressables View Registry", titleStyle);
            GUI.Label(new Rect(rect.x + 32, rect.y + 22, rect.width - 38, 14), "Type → AssetReference (Prefab)", subStyle);
            EditorGUILayout.Space();
        }

        [Title("Add Single", null, TitleAlignments.Left, true, true)]
        [SerializeField, BoxGroup("Options")]
        private AssetReferenceGameObject _singleViewRef;

        [Title("Add Multiple", null, TitleAlignments.Left, true, true)]
        [SerializeField, BoxGroup("Options")]
        private List<AssetReferenceGameObject> _viewsToAdd = new();
        
        [Title("Import", null, TitleAlignments.Left, true, true)]
        [SerializeField, BoxGroup("Options"), FolderPath(AbsolutePath = false)]
        private string _folderToScan;

        
        private void OnValidate()
        {
            TryMigrateLegacyEntries();
            RebuildMap();
        }

        [Button]
        private void AddSingleFromField()
        {
            if (_singleViewRef != null)
            {
                AddView(_singleViewRef);
            }
        }

        [Button]
        private void AddView(AssetReferenceGameObject view)
        {
            if (view == null)
            {
                return;
            }

            var assetGo = view.editorAsset;
            if (assetGo == null)
            {
                return;
            }

            var component = assetGo.GetComponent<View>();
            if (component == null)
            {
                return;
            }

            var type = component.GetType();
            AddOrReplaceEntry(type, view);
            RebuildMap();
        }
        
        [Button]
        private void AddViews()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Add Multiple Views");
#endif
            foreach (var view in _viewsToAdd)
            {
                if (view == null) continue;
                var assetGo = view.editorAsset as GameObject;
                if (assetGo == null) continue;

                var component = assetGo.GetComponent<View>();
                if (component == null) continue;

                var type = component.GetType();
                AddOrReplaceEntry(type, view);
            }

            RebuildMap();
        }

        [Button]
        private void AddFromSelectionToList()
        {
            var objects = Selection.objects;
            int queued = 0;
            foreach (var obj in objects)
            {
                var go = obj as GameObject;
                if (go == null) continue;
                if (!TryGetViewTypeFromGameObject(go, out var _)) continue;
                var path = AssetDatabase.GetAssetPath(go);
                var guid = AssetDatabase.AssetPathToGUID(path);
                if (string.IsNullOrEmpty(guid)) continue;
                var aref = new AssetReferenceGameObject(guid);
#if UNITY_EDITOR
                Undo.RecordObject(this, "Queue Views To Add");
#endif
                _viewsToAdd.Add(aref);
                queued++;
            }

            if (queued > 0)
            {
                GUI.changed = true;
                Debug.Log($"Queued {queued} view(s) to list from selection");
            }
        }

        [OnInspectorGUI]
        private void DrawAddMultipleDropZone()
        {
            var e = Event.current;
            var rect = GUILayoutUtility.GetRect(0, 72, GUILayout.ExpandWidth(true));

            bool isHover = rect.Contains(e.mousePosition);
            bool isDragging = isHover && (e.type == EventType.DragUpdated || e.type == EventType.DragPerform);

            var bg = isDragging ? new Color(0.10f, 0.55f, 0.90f, 0.12f) : new Color(0.25f, 0.25f, 0.25f, 0.10f);
            var border = isDragging ? new Color(0.10f, 0.55f, 0.90f, 0.9f) : new Color(0.4f, 0.4f, 0.4f, 0.9f);
            int thickness = isDragging ? 3 : 1;

            // Background
            EditorGUI.DrawRect(rect, bg);

            // Border
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), border); // top
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), border); // bottom
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), border); // left
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), border); // right

            // Content
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12
            };
            var hintStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

            var titleRect = new Rect(rect.x, rect.y + 14, rect.width, 18);
            var hintRect = new Rect(rect.x, rect.y + 34, rect.width, 16);

            GUI.Label(titleRect, "Drop Prefabs Here", titleStyle);
            GUI.Label(hintRect, "Only prefabs with View component will be queued", hintStyle);

            // Drag handling
            if (isDragging)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    int queued = 0;
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        var go = obj as GameObject;
                        if (go == null) continue;
                        if (!TryGetViewTypeFromGameObject(go, out var _)) continue;
                        var path = AssetDatabase.GetAssetPath(go);
                        var guid = AssetDatabase.AssetPathToGUID(path);
                        if (string.IsNullOrEmpty(guid)) continue;
                        var aref = new AssetReferenceGameObject(guid);
#if UNITY_EDITOR
                        Undo.RecordObject(this, "Queue Views To Add (Drag & Drop)");
#endif
                        _viewsToAdd.Add(aref);
                        queued++;
                    }
                    if (queued > 0)
                    {
                        GUI.changed = true;
                        Debug.Log($"Queued {queued} view(s) to list via drag & drop");
                    }
                }
                e.Use();
            }
        }
        
        [Button]
        private void AddFromFolder()
        {
            if (string.IsNullOrEmpty(_folderToScan))
            {
                Debug.LogWarning("Folder path is empty");
                return;
            }

            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { _folderToScan });
            int added = 0;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (go == null) continue;
                if (!TryGetViewTypeFromGameObject(go, out var type)) continue;

                var aref = new AssetReferenceGameObject(guid);
                AddOrReplaceEntry(type, aref);
                added++;
            }

            if (added > 0)
            {
                RebuildMap();
                Debug.Log($"Added/updated {added} view(s) from folder");
            }
        }

        private void AddOrReplaceEntry(Type type, AssetReferenceGameObject prefab)
        {
            if (type == null || prefab == null) return;

#if UNITY_EDITOR
            Undo.RecordObject(this, "Add/Replace View Entry");
#endif

            if (_entries == null)
            {
                _entries = new List<ViewEntry>();
            }

            for (int i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (e != null && e.ViewType == type)
                {
                    e.Prefab = prefab;
                    _entries[i] = e;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                    return;
                }
            }

            _entries.Add(new ViewEntry { ViewType = type, Prefab = prefab });
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private static bool TryGetViewTypeFromGameObject(GameObject assetGo, out Type type)
        {
            type = null;
            if (assetGo == null) return false;
            var component = assetGo.GetComponent<View>();
            if (component == null) return false;
            type = component.GetType();
            return true;
        }

        [Button]
        private void Clear()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Clear View Entries");
#endif
            _entries.Clear();
            _viewMap?.Clear();
#if UNITY_EDITOR
            _legacyEntries?.Clear();
            EditorUtility.SetDirty(this);
#endif
        }
#endif
    }
}
