using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    [CustomEditor(typeof(Sequence), true)]
    public class SequenceEditor : Editor
    {
        protected override void OnHeaderGUI()
        {
            var sequence = (Sequence) target;
            
            if (sequence != null)
            {
                GUILayout.Box(sequence.GetType().Name, Styles.InspectorTitle);
                if (GUILayout.Button(sequence.Guid, Styles.NodeInspectorUid))
                {
                    GUIUtility.systemCopyBuffer = sequence.Guid;
                    Debug.Log($"Copied {sequence.GetType().Name.ToLower()} guid '{sequence.Guid}' to clipboard.");
                }

                EditorGUILayout.Space(8);
            }
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                SequenceEditorWindow.Open((Sequence) target);
            }
            
            serializedObject.Update();
            
            SerializedProperty property = serializedObject.GetIterator();
            
            if (property.NextVisible(true)) 
            {
                do 
                {
                    if (property.name != "m_Script")
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(property.name), true);
                    }
                }
                while (property.NextVisible(false));
            }
		
            serializedObject.ApplyModifiedProperties();
        }
    }
}