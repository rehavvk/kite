using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rehawk.Kite
{
    public class VariableContainer
    {
        private readonly Dictionary<string, Dictionary<string, object>> values;
        private readonly List<string> ownerKeys;

        public VariableContainer()
        {
            values = new Dictionary<string, Dictionary<string, object>>();
            ownerKeys = new List<string>();
        }

        public VariableContainer(VariableContainer origin)
        {
            values = new Dictionary<string, Dictionary<string, object>>();
            ownerKeys = new List<string>();

            foreach (KeyValuePair<string, Dictionary<string, object>> entry in origin.values)
            {
                var dictionary = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> subEntry in entry.Value)
                {
                    dictionary[subEntry.Key] = subEntry.Value;
                }

                values[entry.Key] = dictionary;
                if (!ownerKeys.Contains(entry.Key))
                {
                    ownerKeys.Add(entry.Key);
                }
            }
        }

        public IReadOnlyList<string> OwnerKeys
        {
            get { return new ReadOnlyCollection<string>(ownerKeys); }
        }

        public bool TryGetValuesOfOwnerKey(string ownerKey, out IReadOnlyDictionary<string, object> result)
        {
            result = null;
            
            if (values.TryGetValue(ownerKey, out Dictionary<string, object> valuesOfOwnerKey))
            {
                result = new ReadOnlyDictionary<string, object>(valuesOfOwnerKey);
                return true;
            }

            return false;
        }
        
        public void SetValue<T>(object owner, string key, T value)
        {
            string ownerKey = ToOwnerKey(owner);
            
            if (!values.ContainsKey(ownerKey))
            {
                values.Add(ownerKey, new Dictionary<string, object>());
            }

            values[ownerKey][key] = value;
            
            if (!ownerKeys.Contains(ownerKey))
            {
                ownerKeys.Add(ownerKey);
            }
        }

        public T GetValue<T>(object owner, string key, T fallback = default)
        {
            string ownerKey = ToOwnerKey(owner);

            if (values.ContainsKey(ownerKey) && values[ownerKey].TryGetValue(key, out object value) && value is T castedValue)
            {
                return castedValue;
            }

            return fallback;
        }

        public bool TryGetValue<T>(object owner, string key, out T result)
        {
            string ownerKey = ToOwnerKey(owner);

            if (values.ContainsKey(ownerKey) && values[ownerKey].TryGetValue(key, out object value) && value is T castedValue)
            {
                result = castedValue;
                return true;
            }

            result = default;
            return false;
        }

        private static string ToOwnerKey(object owner)
        {
            if (owner == null || owner is ISequenceDirector)
            {
                return "DIRECTOR";
            }
            
            if (owner is NodeBase node)
            {
                return node.Uid;
            }

            return owner.ToString();
        }
    }
}