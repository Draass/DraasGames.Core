#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Packages.DraasGames.Core.Editor
{
    public class DraasGamesModulesToggleWindow : OdinEditorWindow
    {
        [MenuItem("Tools/DraasGames")]
        private static void OpenWindow()
        {
            GetWindow<DraasGamesModulesToggleWindow>("DraasGames Modules Settings").Show();
        }

        [InfoBox("Toggle your modules on/off and then click 'Apply Defines' to update scripting defines.")]
        [InfoBox("Enabling this will require Addressables in the project")]
        [SerializeField, ToggleLeft, LabelText("Enable Addressables Module")] 
        [BoxGroup("Defines")]
        private bool _addressablesModuleEnabled;
    
        [Space(10)]
        [SerializeField, ToggleLeft, LabelText("Enable Effects Module")]
        [InfoBox("Enabling this will require DoTween in the project")]
        [BoxGroup("Defines")]
        private bool _effectsModuleEnabled;

        [Space(10)]
        [SerializeField, ToggleLeft, LabelText("Display start game button")]
        [InfoBox("Enabling this will draw a StartGameButton in the toolbar. State might be updated after editor restart" +
                 " in case you disabled this")]
        [OnValueChanged(nameof(OnStartButtonStateChanged))]
        [BoxGroup("Options")]
        private bool _startGameButtonEnabled;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            RefreshFromCurrentSettings();
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 0.4f)]
        [BoxGroup("Defines")]
        public void ApplyDefines()
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            NamedBuildTarget namedTargetGroup = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
        
            if (namedTargetGroup == NamedBuildTarget.Unknown)
            {
                Debug.LogError("Could not determine a valid Build Target Group.");
                return;
            }

            string currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedTargetGroup);
        
            var defineList = currentDefines
                .Split(';')
                .Select(d => d.Trim())
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();

            HandleDefineSymbol(defineList, EditorConstants.AddressablesModuleDefine, _addressablesModuleEnabled);
            HandleDefineSymbol(defineList, EditorConstants.EffectsModuleDefine, _effectsModuleEnabled);
            
            string updatedDefines = string.Join(";", defineList);
            PlayerSettings.SetScriptingDefineSymbols(namedTargetGroup, updatedDefines);
            
            Debug.Log("Scripting defines updated: " + updatedDefines);
        }
        
        private void OnStartButtonStateChanged(bool state)
        {
            EditorPrefs.SetBool(EditorConstants.StartGameButtonPrefsKey, state);
        }

        private void HandleDefineSymbol(List<string> defineList, string symbol, bool shouldEnable)
        {
            bool currentlyPresent = defineList.Contains(symbol);

            if (shouldEnable && !currentlyPresent)
            {
                defineList.Add(symbol);
            }
            else if (!shouldEnable && currentlyPresent)
            {
                defineList.Remove(symbol);
            }
        }

        [Button(ButtonSizes.Medium), GUIColor(0.6f, 0.6f, 1f)]
        [BoxGroup("Defines")]
        public void RefreshFromCurrentSettings()
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var defineList = currentDefines.Split(';').Select(d => d.Trim()).ToList();

            _addressablesModuleEnabled = defineList.Contains(EditorConstants.AddressablesModuleDefine);
            _effectsModuleEnabled = defineList.Contains(EditorConstants.EffectsModuleDefine);
            _startGameButtonEnabled =
                EditorPrefs.GetBool(EditorConstants.StartGameButtonPrefsKey, false);
            
            // Debug.Log("Refreshed from current defines: " + currentDefines);
        }
    }
}
#endif

