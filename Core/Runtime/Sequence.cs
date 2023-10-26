using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rehawk.Kite
{
    [CreateAssetMenu(menuName = "Kite/Sequence", order = 800)]
    public class Sequence : ScriptableObject, IList<NodeBase>
    {
        [HideInInspector] 
        [SerializeField] private string guid;

        [HideInInspector] 
        [SerializeReference] private List<NodeBase> nodes = new List<NodeBase>();

        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        public bool TryGetNodeByGuid(string guid, out NodeBase node)
        {
            node = nodes.FirstOrDefault(n => n.Guid == guid);
            return node != null;
        }

        public bool TryGetIndexByGuid(string guid, out int index)
        {
            index = -1;
            
            NodeBase node = nodes.FirstOrDefault(n => n.Guid == guid);
            if (node != null)
            {
                index = IndexOf(node);
                return true;
            }

            return false;
        }

        public bool TryGetIndexByIndentLevel(int startingIndex, int indentLevel, out int result)
        {
            for (int i = startingIndex; i < nodes.Count; i++)
            {
                if (nodes[i].IndentLevel == indentLevel)
                {
                    result = i;
                    return true;
                }
            }

            result = -1;

            return false;
        }

        public bool TryGetIndexByType<T>(int startingIndex, out int result) where T : NodeBase
        {
            for (int i = startingIndex; i < nodes.Count; i++)
            {
                if (nodes[i].GetType() == typeof(T))
                {
                    result = i;
                    return true;
                }
            }

            result = -1;

            return false;
        }

        public bool TryGetIndexByType<T>(int startingIndex, int indentLevel, out int result) where T : NodeBase
        {
            for (int i = startingIndex; i < nodes.Count; i++)
            {
                if (nodes[i].IndentLevel == indentLevel && nodes[i].GetType() == typeof(T))
                {
                    result = i;
                    return true;
                }
            }

            result = -1;

            return false;
        }

        #region IList

        public int Count
        {
            get { return nodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
        
        public NodeBase this[int index]
        {
            get { return nodes[index]; }
            set { nodes[index] = value; }
        }

        public void Add(NodeBase item)
        {
            nodes.Add(item);
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public bool Contains(NodeBase item)
        {
            return nodes.Contains(item);
        }

        public void CopyTo(NodeBase[] array, int arrayIndex)
        {
            nodes.CopyTo(array, arrayIndex);
        }

        public bool Remove(NodeBase item)
        {
            return nodes.Remove(item);
        }

        public int IndexOf(NodeBase item)
        {
            return nodes.IndexOf(item);
        }

        public void Insert(int index, NodeBase item)
        {
            nodes.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            nodes.RemoveAt(index);
        }

        public IEnumerator<NodeBase> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        #endregion
    }
}