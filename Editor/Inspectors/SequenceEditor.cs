using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    [CustomEditor(typeof(Sequence), true)]
    public class SequenceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                SequenceEditorWindow.Open((Sequence) target);
            }
        }
    }
}