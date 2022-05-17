using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Flow")]
    public class BreakNode : NodeBase
    {
        [HideInInspector] 
        [SerializeField] private int breakableNodeIndex;

        public override DecreaseIndentLevelMode DecreasesIndentLevel
        {
            get { return DecreaseIndentLevelMode.Self; }
        }

        public override int LeftMargin
        {
            get { return 20; }
        }

        public override string Title
        {
            get { return "BREAK"; }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            NodeBase node = flow.Sequence[breakableNodeIndex];
            
            if (node is IBreakableNode breakableNode)
            {
                breakableNode.Break(flow);
            }
            else
            {
                flow.Log(LogLevel.Error, "Breakable node not implements IBreakableNode.", this);
            }
        }

        protected override void OnValidate(Sequence sequence)
        {
            base.OnValidate(sequence);

            for (int i = Index - 1; i >= 0; i--)
            {
                if (i < sequence.Count && sequence[i] is IBreakableNode)
                {
                    breakableNodeIndex = i;
                }
            }
        }
    }

    public interface IBreakableNode
    {
        void Break(Flow flow);
    }
}