using System;
using UnityEngine;

namespace Rehawk.Kite
{
    [Serializable]
    public abstract class ConditionBase
    {
        // Little hack to hide the label in lists.
        [HideInInspector]
        [SerializeField] private string name = " ";
        
        public abstract string Summary { get; }
            
        public abstract bool EvaluateCondition(Flow flow);
    }
}