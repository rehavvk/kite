using UnityEditor;
using UnityEditor.Callbacks;

namespace Rehawk.Kite
{
    public static class SequenceDoubleClickHandler
    {
        [OnOpenAsset(1)]
        public static bool OpenSequence(int instanceID, int line)
        {
            string assetPath = AssetDatabase.GetAssetPath(instanceID);
            var sequence = AssetDatabase.LoadAssetAtPath<Sequence>(assetPath);
            
            if (sequence)
            {
                SequenceEditorWindow.Open(sequence);
                return true;
            }
 
            return false;
        }
    }
}