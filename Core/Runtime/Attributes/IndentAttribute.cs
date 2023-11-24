using UnityEngine;

namespace Rehawk.Kite
{
    public class IndentAttribute : PropertyAttribute
    {
        public readonly int indent;

        public IndentAttribute(int indent)
        {
            this.indent = indent;
        }
    }
}