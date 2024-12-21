using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace DraasGames.Core.Runtime.UI.Extensions.Editor
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