using System;

namespace Rehawk.Kite
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IconAttribute : Attribute
    {
        public readonly string iconName;
        public readonly Type fromType;
        
        public IconAttribute(string iconName = "") 
        {
            this.iconName = iconName;
        }
        
        public IconAttribute(Type fromType) 
        {
            this.fromType = fromType;
        }
    }
}