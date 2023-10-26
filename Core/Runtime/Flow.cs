using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Kite
{
    public class Flow
    {
        private readonly Dictionary<object, List<Coroutine>> routinesByOwner = new Dictionary<object, List<Coroutine>>();
        private readonly VariableContainer variables;
        
        private NodeBase activeNode;

        private Coroutine loopRoutine;
        private int nextNodeIndex;

        public event EventHandler Cancelled;
        public event EventHandler Stopped;
        public event EventHandler Completed;

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

            Cancelled?.Invoke(this, EventArgs.Empty);
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

            Stopped?.Invoke(this, EventArgs.Empty);
        }

        public void Continue(int nodeIndex)
        {
            nextNodeIndex = nodeIndex;
        }

        private void Complete()
        {
            Stop();

            Completed?.Invoke(this, EventArgs.Empty);
        }

        public Coroutine StartCoroutine(object owner, IEnumerator routine)
        {
            Coroutine coroutine = Director.RunCoroutine(routine);

            if (!routinesByOwner.ContainsKey(owner))
            {
                routinesByOwner.Add(owner, new List<Coroutine>());
            }

            routinesByOwner[owner].Add(coroutine);

            return coroutine;
        }

        public Coroutine StartCoroutine(object owner, string key, IEnumerator routine)
        {
            Coroutine coroutine = StartCoroutine(owner, routine);
            SetValue(owner, key, routine);

            return coroutine;
        }

        public void StopCoroutine(object owner, Coroutine routine)
        {
            if (routine != null)
            {
                Director.CancelCoroutine(routine);

                if (routinesByOwner.ContainsKey(owner))
                {
                    routinesByOwner[owner].Remove(routine);
                }
            }
        }

        public void StopCoroutine(object owner, string key)
        {
            Coroutine routine = GetValue<Coroutine>(owner, key);
            StopCoroutine(owner, routine);
        }

        public void StopAllCoroutines(object owner)
        {
            if (routinesByOwner.ContainsKey(owner))
            {
                foreach (Coroutine coroutine in routinesByOwner[owner])
                {
                    Director.CancelCoroutine(coroutine);
                }

                routinesByOwner.Remove(owner);
            }
        }

        public void SetValue<T>(object owner, string key, T value, bool persistant = false)
        {
            variables.SetValue<T>(owner, key, value);

            if (persistant)
            {
                Director.SetValue(owner, key, value);
            }
        }

        public T GetValue<T>(object owner, string key, T fallback = default)
        {
            return variables.GetValue<T>(owner, key, fallback);
        }

        public bool TryGetValue<T>(object owner, string key, out T result)
        {
            return variables.TryGetValue<T>(owner, key, out result);
        }

        public void Log(LogLevel level, string message, NodeBase node)
        {
            int originIndex = Sequence.IndexOf(node);

            Director.Log(level, message, originIndex, node);
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