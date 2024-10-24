using UnityEngine;

namespace Rehawk.Kite
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public string MemberName { get; }
        public object ComparedValue { get; }

        public ShowIfAttribute(string memberName, object comparedValue)
        {
            MemberName = memberName;
            ComparedValue = comparedValue;
        }
    }
}