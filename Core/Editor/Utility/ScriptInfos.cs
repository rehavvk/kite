using System;
using System.Collections.Generic;
using System.Linq;

namespace Rehawk.Kite
{
    public class ScriptInfos
    {
        public struct ScriptInfo
        {
            public Type type;
            public string name;
            public string category;

            public bool IsValid { get; }

            public ScriptInfo(Type type, string name, string category)
            {
                this.IsValid = true;
                
                this.type = type;
                this.name = name;
                this.category = category;
            }
        }

        private static Dictionary<Type, List<ScriptInfo>> cachedInfos;

        public static List<ScriptInfo> GetScriptInfosOfType(Type baseType)
        {
            if (cachedInfos == null)
            {
                cachedInfos = new Dictionary<Type, List<ScriptInfo>>();
            }

            if (cachedInfos.TryGetValue(baseType, out List<ScriptInfo> infosResult))
            {
                return infosResult.ToList();
            }

            infosResult = new List<ScriptInfo>();

            var subTypes = baseType.IsGenericTypeDefinition ? new Type[] { baseType } : ReflectionUtils.GetImplementationsOf(baseType);
            foreach (var subType in subTypes)
            {
                if (subType.IsAbstract)
                {
                    continue;
                }

                var scriptName = subType.Name.Replace("Node", string.Empty).SplitCamelCase();
                var scriptCategory = string.Empty;

                var nameAttribute = subType.GetAttribute<NameAttribute>(true);
                if (nameAttribute != null)
                {
                    scriptName = nameAttribute.name;
                }

                var categoryAttribute = subType.GetAttribute<CategoryAttribute>(true);
                if (categoryAttribute != null)
                {
                    scriptCategory = categoryAttribute.category;
                }

                var info = new ScriptInfo(subType, scriptName, scriptCategory);

                infosResult.Add(info);
            }

            infosResult = infosResult
                .Where(s => s.IsValid)
                .OrderBy(s => s.category)
                .ThenBy(s => s.name)
                .ToList();

            cachedInfos[baseType] = infosResult;

            return infosResult;
        }
    }
}