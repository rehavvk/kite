using UnityEngine;
using UnityEditor;

namespace Rehawk.Kite
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}