using System;
using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Flow")]
    public class SubSequenceNode : NodeBase
    {
        [SerializeField] private ContinueMode continueMode;
        [SerializeField] private Sequence sequence;

        public override string Title
        {
            get
            {
                switch (continueMode)
                {
                    case ContinueMode.Instant:
                        return "RUN Sequence";
                    case ContinueMode.WaitForCompletion:
                        return "WAIT FOR Sequence";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override string Summary
        {
            get { return sequence ? sequence.name : "-"; }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            Flow subFlow = Flow.Run(flow, sequence);

            if (continueMode == ContinueMode.Instant)
            {
                Continue(flow);
            }
            else if (continueMode == ContinueMode.WaitForCompletion)
            {
                subFlow.Completed += OnSubFlowCompleted;
            }
        }

        private void OnSubFlowCompleted(object sender, EventArgs e)
        {
            Flow subFlow = (Flow)sender;
            subFlow.Completed -= OnSubFlowCompleted;

            Continue(subFlow.ParentFlow);
        }

        private enum ContinueMode
        {
            Instant,
            WaitForCompletion
        }
    }
}