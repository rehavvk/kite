﻿using System;

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

        public override void DoInsert(Sequence sequence, NodeBase node, int index, bool isDuplicate)
        {
            base.DoInsert(sequence, node, index, isDuplicate);
            
            if (!isDuplicate)
            {
                sequence.Insert(index + 1, Activator.CreateInstance<BreakNode>());
            }
        }

        public override void DoMove(Sequence sequence, NodeBase node, int sourceIndex, int destinationIndex)
        {
            if (!sequence.TryGetIndexByIndentLevel(sourceIndex + 1, node.IndentLevel, out int endIndex))
            {
                endIndex = sequence.Count - 1;
            }

            MoveRange(sequence, sourceIndex, endIndex - sourceIndex, destinationIndex);
        }
    }
}