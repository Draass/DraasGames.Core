using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Internal.UIToolkitIntegration;
using UnityEditor;

namespace DraasGames.Core.Runtime.UI.Extensions
{
    [CustomEditor(typeof(CustomButton))]
    public class CustomButtonOdinEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
    
    // Attribute Processor to show/hide inherited fields
}