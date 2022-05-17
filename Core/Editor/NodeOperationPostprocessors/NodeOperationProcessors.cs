using System;
using System.Collections.Generic;

namespace Rehawk.Kite
{
    public static class NodeOperationProcessors
    {
        private static readonly Dictionary<Type, NodeOperationProcessor> operationProcessors = new Dictionary<Type, NodeOperationProcessor>();
        
        private static readonly Dictionary<Type, NodeOperationProcessor> explicitOperationProcessorsCache = new Dictionary<Type, NodeOperationProcessor>();

        static NodeOperationProcessors()
        {
            Add(typeof(NodeBase), new NodeOperationProcessor());
            Add(typeof(ConditionNodeBase), new ConditionNodeOperationProcessor());
            Add(typeof(RandomNode), new RandomNodeOperationProcessor());
            Add(typeof(CaseNode), new CaseNodeOperationProcessor());
            Add(typeof(ElseNode), new ElseNodeOperationProcessor());
        }

        public static void Add(Type type, NodeOperationProcessor processor)
        {
            operationProcessors.Add(type, processor);    
        }
        
        public static bool TryGet(Type type, out NodeOperationProcessor processor)
        {
            if (explicitOperationProcessorsCache.TryGetValue(type, out processor))
            {
                return true;
            }
            
            // Create cache.
            
            NodeOperationProcessor bestMatchingProcessor = null;
            int bestMatchingProcessorLevelOfInheritance = int.MaxValue;

            foreach (KeyValuePair<Type, NodeOperationProcessor> entry in operationProcessors)
            {
                Type processorType = entry.Key;
                
                if (processorType == type)
                {
                    // Direct match... Cache it!
                    bestMatchingProcessor = entry.Value;

                    break;
                }
                else
                {
                    if (processorType.IsAssignableFrom(type))
                    {
                        int levelOfInheritance = 0;

                        Type typeToCheck = processorType;
                        while (typeToCheck != null && (typeToCheck.BaseType != typeof(object) || bestMatchingProcessor == null))
                        {
                            typeToCheck = typeToCheck.BaseType;
                            levelOfInheritance++;
                        }

                        if (bestMatchingProcessorLevelOfInheritance > levelOfInheritance)
                        {
                            bestMatchingProcessor = entry.Value;
                            bestMatchingProcessorLevelOfInheritance = levelOfInheritance;
                        }
                    }
                    else
                    {
                        // No match... Continue with next.
                    }
                }
            }

            if (bestMatchingProcessor != null)
            {
                explicitOperationProcessorsCache[type] = bestMatchingProcessor;

                processor = bestMatchingProcessor;
                
                return true;
            }
            
            return false;
        }
    }
}