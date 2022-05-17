using System;
using UnityEngine;

namespace Rehawk.Kite.Signals
{
    [Category("Signals")]
    [Description("Waits for the invocation of the set signal when the set mode matches.\n\nDirector = Reacts only when invoked by this director\nSelf = Reacts only when invoked for this flow\nGlobal = Reacts in any case\n\nIf reactToOld is set to TRUE also signals invoked before the node was entered are counting but only once.")]
    [Icon("WaitForSignal")]
    public class WaitForSignalNode : NodeBase
    {
        public enum Mode
        {
            Director,
            Self,
            Global
        }

        [SerializeField] private Mode mode;
        [SerializeField] private readonly bool reactToOld = false;
        [SerializeField] private Signal signal;

        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                switch (mode)
                {
                    case Mode.Self:
                        summary += "Own Signal:";
                        break;
                    case Mode.Director:
                        summary += "Director Signal:";
                        break;
                    case Mode.Global:
                        summary += "Global Signal:";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (signal)
                {
                    summary += $" <b>{signal.name}</b>";
                }
                
                return summary;
            }
        }

        protected override void OnFlowStarted(Flow flow)
        {
            base.OnFlowStarted(flow);

            if (reactToOld)
            {
                RegisterListener(flow, false);
            }
        }

        protected override void OnFlowStopped(Flow flow)
        {
            base.OnFlowStopped(flow);

            if (flow.TryGetValue(this, "signal_listener", out EventHandler<SignalEventArgs> signalListener))
            {
                signal.Invoked -= signalListener;
            }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            if (reactToOld && flow.TryGetValue(this, "was_invoked_before", out bool wasInvokedBefore) && wasInvokedBefore)
            {
                // Continue directly if we listened to old and were invoked before.

                // But reset for next execution.
                flow.SetValue(this, "was_invoked_before", false);

                Continue(flow);
            }
            else
            {
                RegisterListener(flow, true);
            }
        }

        private void RegisterListener(Flow flow, bool continueOnSignal)
        {
            // Unregister old listeners first.
            if (flow.TryGetValue(this, "signal_listener", out EventHandler<SignalEventArgs> signalListener))
            {
                signal.Invoked -= signalListener;
            }

            signalListener = (sender, e) =>
            {
                // This code will be called when the signal is invoked.

                if (mode == Mode.Self && e.Payload is Flow signalFlow && signalFlow == flow ||
                    mode == Mode.Director && sender is ISequenceDirector signalDirector && signalDirector == flow.Director ||
                    mode == Mode.Global
                )
                {
                    flow.SetValue(this, "was_invoked_before", true);

                    EventHandler<SignalEventArgs> signalListenerToUnlisten = flow.GetValue<EventHandler<SignalEventArgs>>(this, "signal_listener");

                    signal.Invoked -= signalListenerToUnlisten;

                    if (continueOnSignal)
                    {
                        Continue(flow);
                    }
                }
            };

            flow.SetValue(this, "signal_listener", signalListener);

            signal.Invoked += signalListener;
        }
    }
}