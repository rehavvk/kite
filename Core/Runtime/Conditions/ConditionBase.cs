using System;

namespace Rehawk.Kite
{
    [Serializable]
    public abstract class ConditionBase
    {
        public abstract string Summary { get; }
            
        public abstract bool EvaluateCondition(Flow flow);
    }
}