using UnityEngine;
using UnityEditor;

namespace Rehawk.Kite
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        private ShowIfAttribute showIf;
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
            showIf = attribute as ShowIfAttribute;

            if (showIf != null && !string.IsNullOrEmpty(showIf.PropertyName))
            {
                string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, showIf.PropertyName) : showIf.PropertyName;
     
                comparedField = property.serializedObject.FindProperty(path);
     
                if (comparedField == null)
                {
                    Debug.LogError("Cannot find property with name: " + path);
                    return true;
                }
     
                switch (comparedField.type)
                {
                    case "int":
                        return comparedField.intValue.Equals((int) showIf.ComparedValue);
                    case "bool":
                        return comparedField.boolValue.Equals(showIf.ComparedValue);
                    case "Enum":
                        return comparedField.enumValueIndex.Equals((int) showIf.ComparedValue);
                    default:
                        Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                        return true;
                }
            }

            return true;
        }
    }
}