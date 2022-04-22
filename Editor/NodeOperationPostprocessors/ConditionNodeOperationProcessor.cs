using System;

namespace Rehawk.Kite
{
    public class ConditionNodeOperationProcessor : NodeOperationProcessor
    {
        public override int DoAdd(Sequence sequence, NodeBase node)
        {
            int index = base.DoAdd(sequence, node);
            
            sequence.Insert(index + 1, Activator.CreateInstance<EndNode>());

            return index;
        }

        public override void DoInsert(Sequence sequence, NodeBase node, int index, bool isDuplicate)
        {
            base.DoInsert(sequence, node, index, isDuplicate);
            
            if (!isDuplicate)
            {
                sequence.Insert(index + 1, Activator.CreateInstance<EndNode>());
            }
        }

        public override void DoMove(Sequence sequence, NodeBase node, int sourceIndex, int destinationIndex)
        {
            if (!sequence.TryGetIndexByType<EndNode>(sourceIndex + 1, node.IndentLevel, out int endIndex))
            {
                endIndex = sequence.Count - 1;
            }

            MoveRange(sequence, sourceIndex, endIndex - sourceIndex, destinationIndex);
        }
    }
}