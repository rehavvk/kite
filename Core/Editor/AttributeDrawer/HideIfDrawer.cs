using UnityEngine;
using UnityEditor;

namespace Rehawk.Kite
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {
        private HideIfAttribute hideIf;
        private SerializedProperty comparedField;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!Evaluate(property))
                return -EditorGUIUtility.standardVerticalSpacing;
       
            return base.GetPropertyHeight(property, label);
        }
     
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Evaluate(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        
        protected bool Evaluate(SerializedProperty property)
        {
            hideIf = attribute as HideIfAttribute;

            if (hideIf != null && !string.IsNullOrEmpty(hideIf.PropertyName))
            {
                string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, hideIf.PropertyName) : hideIf.PropertyName;
     
                comparedField = property.serializedObject.FindProperty(path);
     
                if (comparedField == null)
                {
                    Debug.LogError("Cannot find property with name: " + path);
                    return true;
                }
     
                switch (comparedField.type)
                {
                    case "bool":
                        return !comparedField.boolValue.Equals(hideIf.ComparedValue);
                    case "Enum":
                        return !comparedField.enumValueIndex.Equals((int) hideIf.ComparedValue);
                    default:
                        Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                        return true;
                }
            }

            return true;
        }
    }
}