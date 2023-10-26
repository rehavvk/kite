using System;
using UnityEngine;

namespace Rehawk.Kite
{
    [Serializable]
    [AddTypeMenu("Static")]
    public class StaticCondition : ConditionBase
    {
        [SerializeField] private bool value;

        public override string Summary
        {
            get { return value.ToStringAdvanced(); }
        }

        public override bool EvaluateCondition(Flow flow)
        {
            return value;
        }
    }
}