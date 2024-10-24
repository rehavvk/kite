using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Rehawk.Kite
{
    public static class ReflectionUtils
    {
        private static Assembly[] loadedAssemblies;
        private static Type[] allTypes;
        private static readonly Dictionary<Type, Type[]> subTypesMap = new Dictionary<Type, Type[]>();

        private static Assembly[] LoadedAssemblies
        {
            get { return loadedAssemblies != null ? loadedAssemblies : loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies(); }
        }

        public static bool HasAttribute<T>(this Type type, bool inherited) where T : Attribute
        {
            return type.HasAttribute(typeof(T), inherited);
        }

        public static bool HasAttribute(this Type type, Type attributeType, bool inherited)
        {
            return inherited ? type.GetAttribute(attributeType, inherited) != null : type.IsDefined(attributeType, false);
        }

        public static object[] GetAllAttributes(this Type type)
        {
            return type.GetCustomAttributes(true);
        }

        public static T GetAttribute<T>(this Type type, bool inherited) where T : Attribute
        {
            return (T)type.GetAttribute(typeof(T), inherited);
        }

        public static Attribute GetAttribute(this Type type, Type attributeType, bool inherited)
        {
            object[] attributes = GetAllAttributes(type);
            for (int i = 0; i < attributes.Length; i++)
            {
                Attribute att = (Attribute)attributes[i];
                Type attType = att.GetType();

                if (attType.IsAssignableFrom(attributeType))
                {
                    if (inherited || type.IsDefined(attType, false))
                    {
                        return att;
                    }
                }
            }

            return null;
        }

        public static Type[] GetAllTypes(bool includeObsolete)
        {
            if (allTypes != null)
            {
                return allTypes;
            }

            List<Type> result = new List<Type>();

            for (int i = 0; i < LoadedAssemblies.Length; i++)
            {
                Assembly asm = LoadedAssemblies[i];
                try
                {
                    result.AddRange(asm.GetExportedTypes().Where(t => (includeObsolete == true || !t.HasAttribute<ObsoleteAttribute>(false)) && !t.HasAttribute<HiddenAttribute>(false)));
                }
                catch
                {
                }
            }

            return allTypes = result.OrderBy(t => t.Namespace).ThenBy(t => t.Name).ToArray();
        }

        public static Type[] GetImplementationsOf(Type baseType)
        {
            if (subTypesMap.TryGetValue(baseType, out Type[] result))
            {
                return result;
            }

            List<Type> temp = new List<Type>();
            Type[] allTypes = GetAllTypes(false);

            for (int i = 0; i < allTypes.Length; i++)
            {
                Type type = allTypes[i];
                if (baseType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    temp.Add(type);
                }
            }

            return subTypesMap[baseType] = temp.ToArray();
        }
        
        public static MethodInfo GetMethod(object target, string methodName)
        {
            return target.GetType()
                         .GetMethod(methodName,
                                    BindingFlags.Instance | BindingFlags.Static |
                                    BindingFlags.NonPublic | BindingFlags.Public);
        }

        public static FieldInfo GetField(object target, string fieldName)
        {
            return GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static PropertyInfo GetProperty(object target, string propertyName)
        {
            return GetAllProperties(target, p => p.Name.Equals(propertyName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

#if UNITY_EDITOR
        public static UnityEditor.MonoScript MonoScriptFromType(Type targetType) 
        {
            if (targetType == null) 
                return null;
            
            var typeName = targetType.Name;
            
            if (targetType.IsGenericType) 
            {
                targetType = targetType.GetGenericTypeDefinition();
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
            }
            
            return UnityEditor.AssetDatabase.FindAssets(string.Format("{0} t:MonoScript", typeName))
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>)
                .FirstOrDefault(m => m != null && m.GetClass() == targetType);
        }
#endif    
        
        public static Func<T, TResult> GetFieldGetter<T, TResult>(FieldInfo info)
        {
#if !NET_STANDARD_2_0 && (UNITY_EDITOR || (!ENABLE_IL2CPP && (UNITY_STANDALONE || UNITY_ANDROID || UNITY_WSA)))
            string name = string.Format("__get_field_{0}_", info.Name);
            DynamicMethod fieldGetter = new DynamicMethod(name, typeof(TResult), new[] { typeof(T) }, typeof(T));
            ILGenerator il = fieldGetter.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, info);
            il.Emit(OpCodes.Ret);
            return (Func<T, TResult>)fieldGetter.CreateDelegate(typeof(Func<T, TResult>));
#else
            return (T instance) => { return (TResult)info.GetValue(instance); };
#endif
        }
        
        private static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
        {
            var types = new List<Type>
                        {
                            target.GetType()
                        };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (var i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<FieldInfo> fieldInfos = types[i]
                                                    .GetFields(BindingFlags.Instance | BindingFlags.Static |
                                                               BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                    .Where(predicate);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    yield return fieldInfo;
                }
            }
        }

        private static IEnumerable<PropertyInfo> GetAllProperties(object target, Func<PropertyInfo, bool> predicate)
        {
            var types = new List<Type>
                        {
                            target.GetType()
                        };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (var i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<PropertyInfo> propertyInfos = types[i]
                                                          .GetProperties(BindingFlags.Instance | BindingFlags.Static | 
                                                                         BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                                          .Where(predicate);

                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    yield return propertyInfo;
                }
            }
        }
    }
}