using System;

namespace Rehawk.Kite
{
    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        public readonly string description;

        public DescriptionAttribute(string description)
        {
            this.description = description;
        }
    }
}