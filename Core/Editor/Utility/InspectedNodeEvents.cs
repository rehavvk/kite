using System;
using UnityEngine;

namespace Rehawk.Kite
{
    public static class InspectedNodeEvents
    {
        public static event EventHandler<NodeBase> NodeChanged;
        public static event EventHandler<NodeBase> NodeGotDirty;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            NodeChanged = default;
            NodeGotDirty = default;
        }

        public static void InvokeNodeChanged(object sender, NodeBase node)
        {
            NodeChanged?.Invoke(sender, node);
        }
        
        public static void InvokeNodeGotDirty(object sender, NodeBase node)
        {
            NodeGotDirty?.Invoke(sender, node);
        }
    }
}