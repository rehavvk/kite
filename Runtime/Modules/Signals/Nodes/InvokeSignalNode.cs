using UnityEngine;

namespace Rehawk.Kite.Signals
{
    [Category("Signals")]
    [Icon("InvokeSignal")]
    public class InvokeSignalNode : NodeBase
    {
        [SerializeField] private Signal signal;

        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                summary += "Signal:";
                
                if (signal)
                {
                    summary += $" <b>{signal.name}</b>";
                }
                
                return summary;
            }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            signal.Invoke(flow.Director, flow);

            Continue(flow);
        }
    }
}