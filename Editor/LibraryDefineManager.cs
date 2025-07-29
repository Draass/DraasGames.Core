using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Packages.DraasGames.Core.Editor
{
    [InitializeOnLoad ]
    public static class LibraryDefineManager
    {
        private static readonly (string assembly, string define)[] Libraries = {
            ("Unity.Addressables", EditorConstants.AddressablesModuleDefine),
            ("DOTween", EditorConstants.EffectsModuleDefine),
            ("R3", EditorConstants.R3ModuleDefine),
        };
    
        static LibraryDefineManager()
        {
            CheckAllLibraries();
        }
    
        private static void CheckAllLibraries()
        {
            var loadedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetName().Name)
                .ToHashSet();
            
            foreach (var (assemblyName, define) in Libraries)
            {
                if (loadedAssemblies.Contains(assemblyName))
                {
                    AddDefineSymbol(define);
                }
                else
                {
                    // addiional check for dotween
                    if (assemblyName == "DOTween")
                    {
                        try
                        {
                            Type doTweenType = Type.GetType("DG.Tweening.DOTween, DOTween");
                            if (doTweenType != null)
                            {
                                AddDefineSymbol(define);
                                continue;
                            }
                        }
                        catch { }
                    }
                
                    RemoveDefineSymbol(define);
                }
            }
        }
    
        private static void AddDefineSymbol(string define)
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
        
            if (!defines.Contains(define))
            {
                defines += ";" + define;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
                Debug.Log($"Added define symbol: {define}");
            }
        }
    
        private static void RemoveDefineSymbol(string define)
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
        
            if (defines.Contains(define))
            {
                defines = defines.Replace(define, "").Replace(";;", ";");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
                Debug.Log($"Removed define symbol: {define}");
            }
        }
    }
}
#endif