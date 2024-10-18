using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Kite
{
    public class Flow
    {
        private readonly Dictionary<NodeBase, List<Coroutine>> routinesByOwner = new Dictionary<NodeBase, List<Coroutine>>();
        private readonly VariableContainer variables;
        
        private NodeBase activeNode;

        private Coroutine loopRoutine;
        private int nextNodeIndex;

        public event Action<Flow> Cancelled;
        public event Action<Flow> Stopped;
        public event Action<Flow> Completed;

        private Flow(ISequenceDirector director, Sequence sequence)
        {
            Director = director;
            Sequence = sequence;
            variables = new VariableContainer(Director.Variables);
        }

        private Flow(Flow parentFlow, Sequence sequence)
        {
            ParentFlow = parentFlow;
            Director = parentFlow.Director;
            Sequence = sequence;
            variables = new VariableContainer(Director.Variables);
        }

        public ISequenceDirector Director { get; }
        public Sequence Sequence { get; }
        public Flow ParentFlow { get; }

        public NodeBase PreviousNode { get; private set; }

        public bool IsRunning { get; private set; }
        
        private void Run()
        {
            if (!initializedSequences.Contains(Sequence))
            {
                for (int i = 0; i < Sequence.Count; i++)
                {
                    Sequence[i].Initialized();
                }
                
                initializedSequences.Add(Sequence);
            }
            
            for (int i = 0; i < Sequence.Count; i++)
            {
                Sequence[i].FlowStarted(this);
            }

            IsRunning = true;
            
            loopRoutine = Director.RunCoroutine(Loop(0));
        }

        public void Cancel()
        {
            Stop();

            Cancelled?.Invoke(this);
        }

        private void Stop()
        {
            if (activeNode != null)
            {
                activeNode.Interrupt(this);
            }

            for (int i = 0; i < Sequence.Count; i++)
            {
                Sequence[i].FlowStopped(this);
            }

            Director.CancelCoroutine(loopRoutine);
            loopRoutine = null;
            
            IsRunning = false;

            Stopped?.Invoke(this);
        }

        public void Continue(int nodeIndex)
        {
            nextNodeIndex = nodeIndex;
        }

        private void Complete()
        {
            Stop();

            Completed?.Invoke(this);
        }

        public void SetValue<T>(string key, T value, bool persistant = false)
        {
            SetValueInternal<T>(this, key, value, persistant);
        }

        public T GetValue<T>(string key, T fallback = default)
        {
            return GetValueInternal<T>(this, key, fallback);
        }

        public bool TryGetValue<T>(string key, out T result)
        {
            return TryGetValueInternal<T>(this, key, out result);
        }

        private IEnumerator Loop(int nodeIndex)
        {
            nextNodeIndex = nodeIndex;

            int i = 0;
            while (true)
            {
                if (nextNodeIndex > -1)
                {
                    i = nextNodeIndex;
                    nextNodeIndex = -1;
                }

                try
                {
                    // Stop execution if out of nodes.
                    if (i >= Sequence.Count)
                    {
                        if (PreviousNode != null)
                        {
                            PreviousNode.Exit(this);
                        }

                        break;
                    }

                    activeNode = Sequence[i];

                    if (PreviousNode != activeNode)
                    {
                        PreviousNode?.Exit(this);
                        activeNode?.Enter(this);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                yield return null;

                PreviousNode = activeNode;
            }

            yield return null;

            Complete();
        }

        internal void Log(LogLevel level, string message, NodeBase node)
        {
            int originIndex = Sequence.IndexOf(node);

            Director.Log(level, message, originIndex, node);
        }

        internal void SetValueInternal<T>(object owner, string key, T value, bool persistant = false)
        {
            variables.SetValue<T>(owner, key, value);

            if (persistant)
            {
                Director.SetValue(owner, key, value);
            }
        }

        internal T GetValueInternal<T>(object owner, string key, T fallback = default)
        {
            return variables.GetValue<T>(owner, key, fallback);
        }

        internal bool TryGetValueInternal<T>(object owner, string key, out T result)
        {
            return variables.TryGetValue<T>(owner, key, out result);
        }

        internal Coroutine StartCoroutine(NodeBase ownerNode, IEnumerator routine)
        {
            Coroutine coroutine = Director.RunCoroutine(routine);

            if (!routinesByOwner.ContainsKey(ownerNode))
            {
                routinesByOwner.Add(ownerNode, new List<Coroutine>());
            }

            routinesByOwner[ownerNode].Add(coroutine);

            return coroutine;
        }

        internal Coroutine StartCoroutine(NodeBase ownerNode, string key, IEnumerator routine)
        {
            Coroutine coroutine = StartCoroutine(ownerNode, routine);
            SetValueInternal(ownerNode, key, routine);

            return coroutine;
        }

        internal void StopCoroutine(NodeBase ownerNode, Coroutine routine)
        {
            if (routine != null)
            {
                Director.CancelCoroutine(routine);

                if (routinesByOwner.TryGetValue(ownerNode, out List<Coroutine> routines))
                {
                    routines.Remove(routine);
                }
            }
        }

        internal void StopCoroutine(NodeBase ownerNode, string key)
        {
            Coroutine routine = GetValueInternal<Coroutine>(ownerNode, key);
            StopCoroutine(ownerNode, routine);
        }

        internal void StopAllCoroutines(NodeBase ownerNode)
        {
            if (routinesByOwner.ContainsKey(ownerNode))
            {
                foreach (Coroutine coroutine in routinesByOwner[ownerNode])
                {
                    Director.CancelCoroutine(coroutine);
                }

                routinesByOwner.Remove(ownerNode);
            }
        }

        private static readonly List<Sequence> initializedSequences = new List<Sequence>();
        
        public static Flow Run(ISequenceDirector director, Sequence sequence)
        {
            var flow = new Flow(director, sequence);

            flow.Run();

            return flow;
        }

        public static Flow Run(Flow parentFlow, Sequence sequence)
        {
            var flow = new Flow(parentFlow, sequence);

            flow.Run();

            return flow;
        }
    }
}