using UnityEngine;

namespace Rehawk.Kite
{
    public enum OperationOperator
    {
        [InspectorName("=")]
        Set,
        [InspectorName("+")]
        Add,
        [InspectorName("-")]
        Subtract,
        [InspectorName("*")]
        Multiply,
        [InspectorName("/")]
        Divide,
        [InspectorName("%")]
        Modulo,
    }
}