using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    [CustomEditor(typeof(SequenceDirector), true)]
    public class SequenceDirectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var director = (SequenceDirector)target;


            if (director.Sequence)
            {
                if (GUILayout.Button("EDIT"))
                {
                    SequenceEditorWindow.Open(director.Sequence);
                }
                
                EditorGUILayout.Space();
            }
            
            SerializedProperty iterator = serializedObject.GetIterator();

            while (iterator.NextVisible(true))
            {
                if (iterator.name != "m_Script")
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}