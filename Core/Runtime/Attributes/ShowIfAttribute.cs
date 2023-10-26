using UnityEngine;

namespace Rehawk.Kite
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public string PropertyName { get; }
        public object ComparedValue { get; }

        public ShowIfAttribute(string propertyName, object comparedValue)
        {
            PropertyName = propertyName;
            ComparedValue = comparedValue;
        }
    }
}