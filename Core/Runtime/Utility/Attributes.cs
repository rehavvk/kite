using System;
using UnityEngine;

namespace Rehawk.Kite
{
    [AttributeUsage(AttributeTargets.All)]
    public class HiddenAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class NameAttribute : Attribute
    {
        public readonly string name;

        public NameAttribute(string name)
        {
            this.name = name;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class CategoryAttribute : Attribute
    {
        public readonly string category;

        public CategoryAttribute(string category)
        {
            this.category = category;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        public readonly string description;

        public DescriptionAttribute(string description)
        {
            this.description = description;
        }
    }

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
    
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
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
    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
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