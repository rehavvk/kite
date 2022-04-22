using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rehawk.Kite
{
    public static class SequenceValidator
    {
        public static void Validate(Sequence sequence)
        {
            // Remove null nodes.

            for (int i = sequence.Count - 1; i >= 0; i--)
            {
                if (sequence[i] == null)
                {
                    sequence.RemoveAt(i);
                }
            }

            // Iterate over nodes to set persistant indices for new nodes.
            
            int nextPersistantIndex = sequence.Count > 0 ? sequence.Max(n => n.PersistantIndex) + 1 : 0;

            for (int i = 0; i < sequence.Count; i++)
            {
                NodeBase node = sequence[i];

                if (node.PersistantIndex < 0)
                {
                    node.PersistantIndex = nextPersistantIndex;
                    nextPersistantIndex += 1;
                }
            }
            
            // Iterate over nodes sorted by persistant indices to assign new guid if needed.

            var uids = new HashSet<string>();

            foreach (NodeBase node in sequence.OrderBy(n => n.PersistantIndex))
            {
                if (string.IsNullOrEmpty(node.Uid) || uids.Contains(node.Uid))
                {
                    node.Uid = Guid.NewGuid().ToString();
                }

                uids.Add(node.Uid);
            }

            // Setup index and indent level.

            int indentLevel = 0;

            for (int i = 0; i < sequence.Count; i++)
            {
                NodeBase node = sequence[i];

                if (node.IncreasesIndentLevel == NodeBase.IncreaseIndentLevelMode.Self)
                {
                    indentLevel = Mathf.Max(indentLevel + 1, 0);
                }

                if (node.DecreasesIndentLevel == NodeBase.DecreaseIndentLevelMode.Self)
                {
                    indentLevel = Mathf.Max(indentLevel - 1, 0);
                }

                node.Index = i;
                node.IndentLevel = indentLevel;

                if (node.IncreasesIndentLevel == NodeBase.IncreaseIndentLevelMode.Next)
                {
                    indentLevel = Mathf.Max(indentLevel + 1, 0);
                }

                if (node.DecreasesIndentLevel == NodeBase.DecreaseIndentLevelMode.Next)
                {
                    indentLevel = Mathf.Max(indentLevel - 1, 0);
                }
            }

            // Do node by node validation.

            for (int i = 0; i < sequence.Count; i++)
            {
                sequence[i].Validate(sequence);
            }
        }

    }
}