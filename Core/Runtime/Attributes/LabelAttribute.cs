using System;
using UnityEngine;

namespace Rehawk.Kite
{
    public class LabelAttribute : PropertyAttribute
    {
        public readonly string label;

        public LabelAttribute(string label)
        {
            this.label = label;
        }
    }
}