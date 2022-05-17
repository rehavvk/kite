using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Kite.Dialogue
{
    [InitializeOnLoad]
    public static class SayNodePreviewHandler
    {
        static SayNodePreviewHandler()
        {
            NodeWrapperEditor.NodeChanged += OnInspectedNodeChanged;
            NodeWrapperEditor.NodeGotDirty += OnInspectedNodeChanged;
        }
        
        private static void OnInspectedNodeChanged(object sender, NodeBase node)
        {
            if (node is SayNode sayNode)
            {
                var previewTarget = Object.FindObjectOfType<SayNodePreviewTarget>();
                if (previewTarget != null)
                {
                    previewTarget.SetText(StringUtils.RemoveTags(sayNode.Text));
                }
                else
                {
                    Debug.LogError("No object with SayNodePreviewTarget component found.");
                }
            }
        }
    }
}