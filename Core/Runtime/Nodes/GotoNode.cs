using System.Linq;
using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Flow")]
    public class GotoNode : NodeBase
    {
        // TODO: Dropdown would be nice.
        [SerializeField] private string label;

        [HideInInspector] 
        [SerializeField] private int targetIndex;

        public override string Summary
        {
            get { return string.IsNullOrEmpty(label) ? "-" : $"'{label}' ({targetIndex})"; }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            ContinueWithIndex(flow, targetIndex);
        }

        protected override void OnValidate(Sequence sequence)
        {
            base.OnValidate(sequence);

            NodeBase targetNode = sequence.FirstOrDefault(n => n is LabelNode labelNode && labelNode.LabelID == label);
            targetIndex = sequence.IndexOf(targetNode);
        }
    }
}