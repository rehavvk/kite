using UnityEngine;

namespace Rehawk.Kite
{
    public class HideIfAttribute : PropertyAttribute
    {
        public string PropertyName { get; }
        public object ComparedValue { get; }

        public HideIfAttribute(string propertyName, object comparedValue)
        {
            PropertyName = propertyName;
            ComparedValue = comparedValue;
        }
    }
}