using UnityEngine;

namespace Rehawk.Kite
{
    public class NodeWrapper : ScriptableObject
    {
        [SerializeReference] public NodeBase node;
    }
}