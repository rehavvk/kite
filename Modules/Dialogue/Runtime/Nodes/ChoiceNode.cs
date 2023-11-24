using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    [Category("Dialogue")]
    public class ChoiceNode : NodeBase, IBreakableNode
    {
        [SerializeField] private bool autoChoose;
        // TODO: Dropdown would be nice.
        [ShowIf(nameof(autoChoose), true), Label("Option Index"), Indent(1)]
        [SerializeField] private int autoChooseOptionIndex;
        
        [TextArea(2, int.MaxValue)] 
        [SerializeField] private string meta;

        [HideInInspector] 
        [SerializeField] private int[] optionIndices;

        public sealed override IncreaseIndentLevelMode IncreasesIndentLevel
        {
            get { return IncreaseIndentLevelMode.Next; }
        }

        public override string Title
        {
            get { return "CHOICE"; }
        }

        void IBreakableNode.Break(Flow flow)
        {
            ContinueOnIndentLevel(flow, IndentLevel);
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            if (flow.Director.TryGetHostObject(out GameObject obj) && obj.TryGetComponent(out DialogueDirector dialogueDirector))
            {
                var optionArgs = new InternalOptionArgs[optionIndices.Length];
                for (int i = 0; i < optionIndices.Length; i++)
                {
                    int index = optionIndices[i];
                    
                    var optionNode = (OptionNode) flow.Sequence[index];
                    optionArgs[i] = new InternalOptionArgs
                    {
                        Uid = flow.Sequence.Guid + "_" + Guid,
                        Text = optionNode.Text,
                        Meta = optionNode.Meta,
                        ContinueCallback = () =>
                        {
                            ContinueWithIndex(flow, index);
                        }
                    };
                }
                
                dialogueDirector.DoChoice(new InternalChoiceArgs
                {
                    Uid = flow.Sequence.Guid + "_" + Guid,
                    Options = optionArgs,
                    AutoChoose = autoChoose,
                    AutoChooseOptionIndex = autoChooseOptionIndex,
                    Meta = meta,
                    ContinueCallback = () =>
                    {
                        Continue(flow);
                    }
                });
            }
            else
            {
                flow.Log(LogLevel.Error, "DialogueDirector couldn't be found. Node was skipped.", this);
                Continue(flow);
            }
        }

        protected override void OnValidate(Sequence sequence)
        {
            base.OnValidate(sequence);

            var optionIndices = new List<int>();

            for (int i = Index; i < sequence.Count; i++)
            {
                if (sequence[i].IndentLevel == IndentLevel + 1 && sequence[i].GetType() == typeof(OptionNode))
                {
                    optionIndices.Add(i);
                }

                if (sequence[i].IndentLevel == IndentLevel && sequence[i].GetType() == typeof(EndNode))
                {
                    break;
                }
            }

            this.optionIndices = optionIndices.ToArray();
        }
        
        public enum Mode
        {
            Random,
            Once
        }
    }
}