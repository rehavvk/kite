using System;
using UnityEngine;

namespace Rehawk.Kite.Signals
{
    [CreateAssetMenu(fileName = "Signal", menuName = "Kite/Signal", order = 800)]
    public class Signal : ScriptableObject
    {
        public event EventHandler<SignalEventArgs> Invoked;

        public void Invoke(object sender, object payload = null)
        {
            Invoked?.Invoke(sender, new SignalEventArgs
            {
                Signal = this,
                Payload = payload
            });
        }
    }
}