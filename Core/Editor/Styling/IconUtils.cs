using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Kite
{
    public static class IconUtils
    {
        private const string DEFAULT_TYPE_ICON_NAME = "System.Object";
        private const string IMPLICIT_ICONS_PATH = "TypeIcons/Implicit/";
        private const string EXPLICIT_ICONS_PATH = "TypeIcons/Explicit/";

        private static readonly Dictionary<string, Texture> typeIcons = new Dictionary<string, Texture>(StringComparer.OrdinalIgnoreCase);

        public static Texture GetTypeIcon(MemberInfo info, bool fallbackToDefault = true)
        {
            if (info == null)
            {
                return null;
            }

            var type = info is Type ? info as Type : info.ReflectedType;
            if (type == null)
            {
                return null;
            }

            if (typeIcons.TryGetValue(type.FullName, out Texture texture))
            {
                if (texture != null)
                {
                    if (texture.name != DEFAULT_TYPE_ICON_NAME || fallbackToDefault)
                    {
                        return texture;
                    }
                }

                return null;
            }

            if (typeof(Object).IsAssignableFrom(type))
            {
                texture = AssetPreview.GetMiniTypeThumbnail(type);
                
                if (texture == null && (typeof(MonoBehaviour).IsAssignableFrom(type) || typeof(ScriptableObject).IsAssignableFrom(type)))
                {
                    texture = EditorGUIUtility.ObjectContent(ReflectionUtils.MonoScriptFromType(type), null).image;
                }
            }

            if (texture == null)
            {
                texture = Resources.Load<Texture>(IMPLICIT_ICONS_PATH + type.FullName);
            }

            if (EditorGUIUtility.isProSkin)
            {
                if (texture == null)
                {
                    var iconAtt = type.GetAttribute<IconAttribute>(true);
                    if (iconAtt != null)
                    {
                        texture = GetTypeIcon(iconAtt, null);
                    }
                }
            }

            if (texture == null)
            {
                Type current = type.BaseType;
                while (current != null)
                {
                    texture = Resources.Load<Texture>(IMPLICIT_ICONS_PATH + current.FullName);
                    current = current.BaseType;
                    if (texture != null)
                    {
                        break;
                    }
                }
            }

            if (texture == null)
            {
                texture = Resources.Load<Texture>(IMPLICIT_ICONS_PATH + DEFAULT_TYPE_ICON_NAME);
            }

            typeIcons[type.FullName] = texture;

            if (texture != null)
            {
                //it should not be
                if (texture.name != DEFAULT_TYPE_ICON_NAME || fallbackToDefault)
                {
                    return texture;
                }
            }

            return null;
        }

        public static Texture GetTypeIcon(IconAttribute iconAttribute, object instance = null)
        {
            if (iconAttribute == null)
            {
                return null;
            }

            if (iconAttribute.fromType != null)
            {
                return GetTypeIcon(iconAttribute.fromType, true);
            }

            if (typeIcons.TryGetValue(iconAttribute.iconName, out Texture texture))
            {
                return texture;
            }

            if (!string.IsNullOrEmpty(iconAttribute.iconName))
            {
                texture = Resources.Load<Texture>(EXPLICIT_ICONS_PATH + iconAttribute.iconName);
                
                if (texture == null)
                {
                    texture = Resources.Load<Texture>(iconAttribute.iconName);
                }
            }

            return typeIcons[iconAttribute.iconName] = texture;
        }
    }
}