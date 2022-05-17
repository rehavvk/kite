namespace Rehawk.Kite
{
    public class ElseNodeOperationProcessor : NodeOperationProcessor
    {
        public override void DoMove(Sequence sequence, NodeBase node, int sourceIndex, int destinationIndex, bool withoutGroup)
        {
            if (withoutGroup)
            {
                base.DoMove(sequence, node, sourceIndex, destinationIndex, withoutGroup);
            }
            else
            {
                if (!sequence.TryGetIndexByType<EndNode>(sourceIndex + 1, node.IndentLevel, out int endIndex))
                {
                    endIndex = sequence.Count - 1;
                }

                MoveRange(sequence, sourceIndex, endIndex - sourceIndex, destinationIndex);
            }
        }
    }
}