using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Flow")]
    [Description("Calls the following nodes only once.")]
    [Icon("Once")]
    public class OnceNode : ConditionNodeBase
    {
        [Tooltip("If TRUE also later flows will remember the state of the once node.")]
        [SerializeField] private bool persistant = true;

        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                summary += "ONCE";

                if (persistant)
                {
                    summary += " PERSISTANT";
                }

                return summary;
            }
        }

        protected override void OnExit(Flow flow)
        {
            base.OnExit(flow);

            SetNodeValue(flow, "was_invoked_before", true, persistant);
        }

        protected override bool EvaluateCondition(Flow flow)
        {
            return !TryGetNodeValue(flow, "was_invoked_before", out bool wasInvokedBefore) || !wasInvokedBefore;
        }
    }
}