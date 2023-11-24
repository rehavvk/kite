using UnityEditor;

namespace Rehawk.Kite.Dialogue
{
    [InitializeOnLoad]
    public static class NodeOperationProcessors
    {
        static NodeOperationProcessors()
        {
            Kite.NodeOperationProcessors.Add(typeof(ChoiceNode), new ChoiceNodeOperationProcessor());
            Kite.NodeOperationProcessors.Add(typeof(OptionNode), new OptionNodeOperationProcessor());
        }
    }
}