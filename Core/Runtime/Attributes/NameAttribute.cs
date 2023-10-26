using System;

namespace Rehawk.Kite
{
    [AttributeUsage(AttributeTargets.All)]
    public class NameAttribute : Attribute
    {
        public readonly string name;

        public NameAttribute(string name)
        {
            this.name = name;
        }
    }
}