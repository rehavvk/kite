using System;
using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Flow")]
    [Name("Else")]
    public class ElseNode : NodeBase
    {
        [SerializeField] private ConditionEvaluationMode mode;
        [SerializeField] private bool invert;
        
        [SubclassSelector]
        [SerializeReference] private ConditionBase[] conditions = Array.Empty<ConditionBase>();

        public sealed override IncreaseIndentLevelMode IncreasesIndentLevel
        {
            get { return IncreaseIndentLevelMode.Next; }
        }

        public override DecreaseIndentLevelMode DecreasesIndentLevel
        {
            get { return DecreaseIndentLevelMode.Self; }
        }

        public override string Title
        {
            get
            {
                string title = string.Empty;

                title += "ELSE";

                if (conditions.Length > 0)
                {
                    title += " IF";
                }

                return title;
            }
        }

        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                if (conditions.Length > 0)
                {
                    summary += ConditionUtility.CreateSummary(conditions, mode, invert);
                }
                
                return summary;
            }
        }

        protected override void OnEnter(Flow flow)
        {
            if (flow.PreviousNode.IndentLevel == IndentLevel)
            {
                // We're coming from a condition or something. Evaluate.
                if (EvaluateCondition(flow))
                {
                    // Continue on next level
                    ContinueOnIndentLevel(flow, IndentLevel + 1);
                }
                else if (flow.Sequence.TryGetIndexByType<ElseNode>(Index + 1, IndentLevel, out int elseIndex))
                {
                    // Continue with next else
                    ContinueWithIndex(flow, elseIndex);
                }
            }
            else if (flow.Sequence.TryGetIndexByType<EndNode>(Index + 1, IndentLevel, out int endIndex))
            {
                // Continue after the group.
                ContinueWithIndex(flow, endIndex);
            }
        }
        
        private bool EvaluateCondition(Flow flow)
        {
            return ConditionUtility.Evaluate(flow, conditions, mode, invert);
        }
    }
}