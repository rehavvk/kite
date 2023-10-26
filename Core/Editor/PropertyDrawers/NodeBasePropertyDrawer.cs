using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    [CustomPropertyDrawer(typeof(NodeBase), true)]
    public class NodeBasePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Dirty way to "hide" the foldout arrow.
            position.x -= 30;
            position.y -= 18;
            position.width += 30;

            EditorGUI.PropertyField(position, property, GUIContent.none, true);
        }
    }
}