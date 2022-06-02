using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Rehawk.Kite.Dialogue
{
    [CustomPropertyDrawer(typeof(ActorEmotionAttribute))]
    public class ActorEmotionDrawer : ShowIfDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Evaluate(property))
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                {
                    property.intValue = EditorGUI.Popup(position, label, property.intValue, KiteDialogueSettings.Emotions.Select(e => new GUIContent(e)).ToArray());
                } 
                else 
                {
                    base.OnGUI(position, property, label);
                }
            }
        }
    }
}