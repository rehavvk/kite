using UnityEngine;
using UnityEditor;

namespace Rehawk.Kite
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is IndentAttribute indentAttribute)
            {
                float indent = indentAttribute.indent * 15f;
                position = new Rect(position.x + indent, position.y, position.width - indent, position.height);
            }
            
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}