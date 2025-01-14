#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace DraasGames.Core.Runtime.UI.Extensions.Editor
{
    [CustomEditor(typeof(CustomSelectable))]
    public class CustomSelectableOdinEditor : OdinEditor
    {
    }
}
#endif