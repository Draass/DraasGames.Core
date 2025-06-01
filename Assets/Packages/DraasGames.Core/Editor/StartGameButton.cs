#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Packages.DraasGames.Core.Editor
{
    [InitializeOnLoad]
    public static class StartGameButton
    {
        private const string PrevSceneKey = "StartGameButton_PrevScene";
        private const string TargetSceneKey = "StartGameButton_TargetScene";
        private const string DefaultTargetScene = "";
 
        static StartGameButton()
        {
            EditorApplication.update += AddToolbarButton;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
 
        static void AddToolbarButton()
        {
            var buttonEnabled = EditorPrefs.GetBool(EditorConstants.StartGameButtonPrefsKey);
            
            if (!buttonEnabled)
            {
                return;
            }
            
            var toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
            if (toolbars.Length == 0) return;
            var toolbar = toolbars[0];
 
            var rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            var rootVE = (VisualElement) rootField.GetValue(toolbar);
 
            var playModeContainer = rootVE.Q<VisualElement>("PlayMode");
            if (playModeContainer == null) return;
 
            var toolbarRoot = playModeContainer.Children().First();
            if (toolbarRoot.Q<Button>("StartGameButton") != null) return;
 
            var button = new Button(OnButtonClicked)
            {
                name = "StartGameButton",
                tooltip = "Play Start Scene"
            };
 
            // Use only Unity's toolbar button class
            button.AddToClassList("unity-toolbar-button");
            button.style.width = 68;
            button.style.height = 20;
            button.style.flexDirection = FlexDirection.Row;
            button.style.alignItems = Align.Center;
            button.style.paddingLeft = 4;
            button.style.paddingRight = 4;
 
            // Icon
            var iconContent = EditorGUIUtility.IconContent("PlayButton");
            var icon = new Image
            {
                image = iconContent.image,
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = 16,
                    height = 16,
                    marginRight = 2,
                }
            };
 
            // Label
            var label = new Label("Play")
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft,
                    fontSize = 12,
                    marginLeft = 2,
                    marginRight = 2,
                }
            };
 
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.Add(icon);
            container.Add(label);
 
            button.Add(container);
 
            // Right-click context menu
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (evt.button == 1)
                {
                    ShowContextMenu();
                    evt.StopPropagation();
                }
            });
 
            toolbarRoot.Add(button);
        }
 
 
        static void OnButtonClicked()
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            EditorPrefs.SetString(PrevSceneKey, activeScene.path);
 
            var targetScene = EditorPrefs.GetString(TargetSceneKey, DefaultTargetScene);
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(targetScene);
                EditorApplication.isPlaying = true;
            }
        }
 
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (EditorPrefs.HasKey(PrevSceneKey))
                {
                    var prevScenePath = EditorPrefs.GetString(PrevSceneKey);
                    EditorPrefs.DeleteKey(PrevSceneKey);
 
                    if (!string.IsNullOrEmpty(prevScenePath) && System.IO.File.Exists(prevScenePath))
                    {
                        EditorSceneManager.OpenScene(prevScenePath);
                    }
                }
            }
        }
 
        static void ShowContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Set Target Scene..."), false, PickTargetScene);
            menu.ShowAsContext();
        }
 
        static void PickTargetScene()
        {
            var scenePath = EditorUtility.OpenFilePanel("Select Target Scene", "Assets", "unity");
            if (!string.IsNullOrEmpty(scenePath))
            {
                if (scenePath.StartsWith(Application.dataPath))
                {
                    var relativePath = "Assets" + scenePath.Substring(Application.dataPath.Length);
                    EditorPrefs.SetString(TargetSceneKey, relativePath);
                }
            }
        }
    }
}
#endif