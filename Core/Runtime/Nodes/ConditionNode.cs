using System;
using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Flow")]
    [Name("If")]
    public class ConditionNode : ConditionNodeBase
    {
        [SerializeField] private ConditionEvaluationMode mode;
        [SerializeField] private bool invert;
        
        [SubclassSelector]
        [SerializeReference] private ConditionBase[] conditions = Array.Empty<ConditionBase>();

        public override string Title
        {
            get { return "IF"; }
        }

        public override string Summary
        {
            get { return ConditionUtility.CreateSummary(conditions, mode, invert); }
        }

        protected override bool EvaluateCondition(Flow flow)
        {
            return ConditionUtility.Evaluate(flow, conditions, mode, invert);
        }
    }
}