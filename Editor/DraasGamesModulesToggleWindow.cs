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
        private const string ADDRESSABLES_MODULE_DEFINE = "DRAASGAMES_ADDRESSABLES_MODULE";
        private const string EFFECTS_MODULE_DEFINE = "DRAASGAMES_EFFECTS_MODULE";
    
        [MenuItem("Tools/DraasGames")]
        private static void OpenWindow() => 
            GetWindow<DraasGamesModulesToggleWindow>("DraasGames Modules Settings").Show();

        [InfoBox("Toggle your modules on/off and then click 'Apply Defines' to update scripting defines.")]
        [InfoBox("Enabling this will require DoTween in the project")]
        [SerializeField, ToggleLeft, LabelText("Enable Addressables Module")] 
        private bool _addressablesModuleEnabled;
    
        [Space(10)]
        [SerializeField, ToggleLeft, LabelText("Enable Effects Module")]
        [InfoBox("Enabling this will require DoTween in the project")]
        private bool _effectsModuleEnabled;

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 0.4f)]
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

            HandleDefineSymbol(defineList, ADDRESSABLES_MODULE_DEFINE, _addressablesModuleEnabled);
            HandleDefineSymbol(defineList, EFFECTS_MODULE_DEFINE, _effectsModuleEnabled);


            // Apply changes back
            string updatedDefines = string.Join(";", defineList);
            PlayerSettings.SetScriptingDefineSymbols(namedTargetGroup, updatedDefines);

            Debug.Log("Scripting defines updated: " + updatedDefines);
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
        public void RefreshFromCurrentSettings()
        {
            // Refresh toggles based on current defines
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var defineList = currentDefines.Split(';').Select(d => d.Trim()).ToList();

            _addressablesModuleEnabled = defineList.Contains("MODULE_A");
            _effectsModuleEnabled = defineList.Contains("MODULE_B");
            // If you had MODULE_C:
            // ModuleC = defineList.Contains("MODULE_C");

            Debug.Log("Refreshed from current defines: " + currentDefines);
        }
    }
}
#endif

