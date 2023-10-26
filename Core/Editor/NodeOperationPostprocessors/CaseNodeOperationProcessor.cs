using System;
using System.Collections.Generic;

namespace Rehawk.Kite
{
    public class CaseNodeOperationProcessor : NodeOperationProcessor
    {
        public override int DoAdd(Sequence sequence, NodeBase node)
        {
            int index = base.DoAdd(sequence, node);
            
            sequence.Insert(index + 1, Activator.CreateInstance<BreakNode>());

            return index;
        }

        public override void DoInsert(Sequence sequence, NodeBase node, int index)
        {
            base.DoInsert(sequence, node, index);
            
            sequence.Insert(index + 1, Activator.CreateInstance<BreakNode>());
        }

        public override void DoInsertDuplicate(Sequence sequence, NodeBase node, int index, bool withoutGroup)
        {
            if (!withoutGroup && sequence.TryGetIndexByType<BreakNode>(index, node.IndentLevel, out int endIndex))
            {
                base.DoInsert(sequence, node, endIndex + 1);
                DuplicateRange(sequence, index, endIndex, endIndex + 2);
            }
            else
            {
                base.DoInsertDuplicate(sequence, node, index, withoutGroup);
            }
        }

        public override void DoMove(Sequence sequence, NodeBase node, int sourceIndex, int destinationIndex, bool withoutGroup)
        {
            if (withoutGroup)
            {
                base.DoMove(sequence, node, sourceIndex, destinationIndex, true);
            }
            else
            {
                if (!sequence.TryGetIndexByType<BreakNode>(sourceIndex + 1, node.IndentLevel, out int endIndex))
                {
                    endIndex = sequence.Count - 1;
                }

                MoveRange(sequence, sourceIndex, endIndex - sourceIndex, destinationIndex);
            }
        }
    }
}