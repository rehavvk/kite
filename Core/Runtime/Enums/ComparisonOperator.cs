using UnityEngine;

namespace Rehawk.Kite
{
    public enum ComparisonOperator
    {
        [InspectorName("==")]
        Equal,
        [InspectorName("!=")]
        NotEqual,
        [InspectorName("<")]
        Less,
        [InspectorName("<=")]
        LessOrEqual,
        [InspectorName(">")]
        Greater,
        [InspectorName(">=")]
        GreaterOrEqual
    }
}