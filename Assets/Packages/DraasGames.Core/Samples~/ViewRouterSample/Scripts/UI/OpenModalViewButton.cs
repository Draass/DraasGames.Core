using System;
using System.Linq;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DraasGames.Core.Samples.Scripts
{
    [RequireComponent(typeof(Button))]
    public class OpenModalViewButton : SerializedMonoBehaviour
    {
        private IViewRouter _viewRouter;
        
        [SerializeField, ValueDropdown(nameof(GetDerivedTypes))] 
        private Type _type;

        private Button _button;
        
        private void Start()
        {
            //_button.onClick.AddListener(() => _viewRouter.Show(Typ));
        }
        
        private Type[] GetDerivedTypes()
        {
            // Assume ViewBase is your base type
            Type baseType = typeof(ViewBase);
        
            // Get all derived types from the current assembly
            var derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract)
                .ToArray();

            return derivedTypes;
        }
    }
}