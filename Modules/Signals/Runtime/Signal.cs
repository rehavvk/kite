using System;
using UnityEngine;

namespace Rehawk.Kite.Signals
{
    [CreateAssetMenu(fileName = "Signal", menuName = "Kite/Signal", order = 800)]
    public class Signal : ScriptableObject
    {
        public event Action<Signal, SignalInvokeArgs> Invoked;

        public void Invoke(object sender, object payload = null)
        {
            Invoked?.Invoke(this, new SignalInvokeArgs
            {
                Sender = sender,
                Payload = payload
            });
        }
    }
}