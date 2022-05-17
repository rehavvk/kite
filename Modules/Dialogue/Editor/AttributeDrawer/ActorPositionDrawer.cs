using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Rehawk.Kite.Dialogue
{
    [CustomPropertyDrawer(typeof(ActorPositionAttribute))]
    public class ActorPositionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.Popup(position, label, property.intValue, KiteDialogueSettings.Positions.Select(p => new GUIContent(p)).ToArray());
            } 
            else 
            {
                base.OnGUI(position, property, label);
            }
        }
    }
}