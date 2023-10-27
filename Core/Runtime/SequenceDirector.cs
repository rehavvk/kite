using System;
using System.Collections;
using UnityEngine;

namespace Rehawk.Kite
{
    public class SequenceDirector : MonoBehaviour, ISequenceDirector
    {
        [SerializeField] private bool isGlobal;
        [SerializeField] private Sequence sequence;
        [SerializeField] private InvokeMode invokeMode = InvokeMode.Start;

        public event EventHandler<SequenceDirectorEventArgs> Started;
        public event EventHandler<SequenceDirectorEventArgs> Stopped;
        public event EventHandler<SequenceDirectorEventArgs> Cancelled;
        public event EventHandler<SequenceDirectorEventArgs> Completed;
        
        public VariableContainer Variables { get; private set; }
        
        public Sequence Sequence
        {
            get { return sequence; }
        }

        private void Awake()
        {
            Variables = new VariableContainer();

            if (isGlobal)
            {
                Global = this;
            }
            
            if (invokeMode == InvokeMode.Awake)
            {
                RunSequence();
            }
        }

        private void Start()
        {
            if (invokeMode == InvokeMode.Start)
            {
                RunSequence();
            }
        }

        [ContextMenu("Run Sequence")]
        public void RunSequence()
        {
            RunSequence(sequence);
        }

        public Flow RunSequence(Sequence sequence)
        {
            if (sequence)
            {
                var flow = Flow.Run(this, sequence);
                flow.Stopped += OnFlowStopped;
                flow.Cancelled += OnFlowCancelled;
                flow.Completed += OnFlowCompleted;
            
                Started?.Invoke(this, new SequenceDirectorEventArgs
                {
                    Sequence = sequence,
                    Flow = flow
                });

                return flow;
            }
            else
            {
                Debug.LogError("Sequence couldn't be started because it was null.", this);
            }

            return null;
        }

        public Coroutine RunCoroutine(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public void CancelCoroutine(Coroutine routine)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }
        }

        public bool TryGetHostObject(out GameObject result)
        {
            result = gameObject;
            return result != null;
        }
        
        public void SetValue<T>(object owner, string key, T value)
        {
            Variables.SetValue<T>(owner, key, value);
        }
        
        public T GetValue<T>(object owner, string key, T fallback = default)
        {
            return Variables.GetValue<T>(owner, key, fallback);
        }
        
        public bool TryGetValue<T>(object owner, string key, out T result)
        {
            return Variables.TryGetValue<T>(owner, key, out result);
        }
        
        void ISequenceDirector.Log(LogLevel level, string message, int originIndex, NodeBase originNode)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Debug.Log(message, this);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message, this);
                    break;
                case LogLevel.Error:
                    Debug.LogError(message, this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
        
        private void OnFlowStopped(object sender, EventArgs e)
        {
            var flow = (Flow) sender;
            
            Stopped?.Invoke(this, new SequenceDirectorEventArgs
            {
                Sequence = flow.Sequence,
                Flow = flow
            });
        }

        private void OnFlowCancelled(object sender, EventArgs e)
        {
            var flow = (Flow) sender;
            
            Cancelled?.Invoke(this, new SequenceDirectorEventArgs
            {
                Sequence = flow.Sequence,
                Flow = flow
            });
        }

        private void OnFlowCompleted(object sender, EventArgs e)
        {
            var flow = (Flow) sender;
            
            Completed?.Invoke(this, new SequenceDirectorEventArgs
            {
                Sequence = flow.Sequence,
                Flow = flow
            });
        }

        private static SequenceDirector global;
        
        public static event Action GlobalChanged;
        
        public static SequenceDirector Global
        {
            get { return global; }
            private set
            {
                if (global != value)
                {
                    global = value;
                    GlobalChanged?.Invoke();
                }
            }
        }

        public enum InvokeMode
        {
            Awake,
            Start,
            Custom
        }
    }
}