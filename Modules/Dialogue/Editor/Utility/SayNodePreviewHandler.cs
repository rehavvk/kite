using UnityEditor;
using Object = UnityEngine.Object;

namespace Rehawk.Kite.Dialogue
{
    [InitializeOnLoad]
    public static class SayNodePreviewHandler
    {
        static SayNodePreviewHandler()
        {
            InspectedNodeEvents.NodeChanged += OnInspectedNodeChanged;
            InspectedNodeEvents.NodeGotDirty += OnInspectedNodeChanged;
        }
        
        private static void OnInspectedNodeChanged(object sender, NodeBase node)
        {
            if (node is SayNode sayNode)
            {
                SayNodePreviewTargetBase[] previewTargets = Object.FindObjectsOfType<SayNodePreviewTargetBase>();

                string text = StringUtils.RemoveSpecificTags(sayNode.Text, KiteDialogueSettings.TagsToRemoveForPreview);
                
                for (int i = 0; i < previewTargets.Length; i++)
                {
                    previewTargets[i].DoTextLine(text, sayNode.Speaker);
                }
            }
        }
    }
}