using UnityEngine;
using UnityEditor;

namespace Rehawk.Kite
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is LabelAttribute labelAttribute)
            {
                label = new GUIContent(labelAttribute.label);
            }
            
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}