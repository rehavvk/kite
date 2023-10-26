using System;
using System.Collections.Generic;

namespace Rehawk.Kite
{
    public class NodeOperationProcessor
    {
        public virtual int DoAdd(Sequence sequence, NodeBase node)
        {
            sequence.Add(node);
            return sequence.IndexOf(node);
        }
        
        public virtual void DoInsert(Sequence sequence, NodeBase node, int index)
        {
            sequence.Insert(index, node);
        }
        
        public virtual void DoInsertDuplicate(Sequence sequence, NodeBase node, int index, bool withoutGroup)
        {
            sequence.Insert(index, node);
        }
        
        public virtual void DoRemove(Sequence sequence, NodeBase node, int index)
        {
            sequence.RemoveAt(index);
        }
        
        public virtual void DoMove(Sequence sequence, NodeBase node, int sourceIndex, int destinationIndex, bool withoutGroup)
        {
            if (destinationIndex > sourceIndex)
            {
                --destinationIndex;
            }

            sequence.RemoveAt(sourceIndex);
            sequence.Insert(destinationIndex, node);
        }
        
        public static void DuplicateRange(Sequence sequence, int startIndex, int endIndex, int destinationIndex)
        {
            var nodesToClone = new List<NodeBase>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                nodesToClone.Add(sequence[i]);
            }
 
            for (int i = nodesToClone.Count - 1; i >= 0; i--)
            {
                sequence.Insert(destinationIndex, (NodeBase) nodesToClone[i].Clone());
            }
        }
        
        public static void MoveRange(Sequence sequence, int startIndex, int length, int destinationIndex)
        {
            int endIndex = startIndex + length;
            
            if (destinationIndex > startIndex)
            {
                destinationIndex -= 1;

                if (destinationIndex > endIndex)
                {
                    for (int i = 0; i <= length; i++)
                    {
                        NodeBase subNode = sequence[startIndex];
                    
                        sequence.RemoveAt(startIndex);
                        sequence.Insert(destinationIndex, subNode);
                    }
                }
            }
            else
            {
                for (int i = 0; i <= length; i++)
                {
                    int subSourceIndex = startIndex + i;
                    int subDestinationIndex = destinationIndex + i;
                        
                    NodeBase subNode = sequence[subSourceIndex];
                    
                    sequence.RemoveAt(subSourceIndex);
                    sequence.Insert(subDestinationIndex, subNode);
                }
            }
        }
    }
}