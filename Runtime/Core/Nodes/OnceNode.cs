using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Flow")]
    [Description("Calls the following nodes only once.\n\nIf persistant is set to TRUE also for other flows of this director.")]
    [Icon("Once")]
    public class OnceNode : ConditionNodeBase
    {
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

            flow.SetValue(this, "was_invoked_before", true, persistant);
        }

        protected override bool EvaluateCondition(Flow flow)
        {
            return !flow.TryGetValue(this, "was_invoked_before", out bool wasInvokedBefore) || !wasInvokedBefore;
        }
    }
}