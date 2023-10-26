using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    [RequireComponent(typeof(ISequenceDirector))]
    public class DialogueDirector : MonoBehaviour
    {
        [SerializeField] private bool isGlobal;
        
        private readonly List<IDialogueHandler> handlers = new List<IDialogueHandler>();

        private bool isRunning;
        private bool hasContinued;
        private Action activeContinueCallback;

        private ISequenceDirector sequenceDirector;
        
        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler Cancelled;
        public event EventHandler Completed;

        public bool IsRunning
        {
            get { return isRunning; }
            private set
            {
                if (isRunning != value)
                {
                    isRunning = value;

                    if (isRunning)
                    {
                        Started?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        private void Awake()
        {
            sequenceDirector = GetComponent<ISequenceDirector>();
            sequenceDirector.Stopped += OnSequenceStopped;
            sequenceDirector.Cancelled += OnSequenceCancelled;
            sequenceDirector.Completed += OnSequenceCompleted;
            
            if (isGlobal)
            {
                Global = this;
            }
        }

        private void OnDestroy()
        { 
            sequenceDirector.Stopped -= OnSequenceStopped;
            sequenceDirector.Cancelled -= OnSequenceCancelled;
            sequenceDirector.Completed -= OnSequenceCompleted;
        }

        public void Continue()
        {
            if (!IsRunning)
                return;

            if (activeContinueCallback != null && !hasContinued)
            {
                hasContinued = true;
                activeContinueCallback.Invoke();
            }
        }

        private void Cancel()
        {
            
        }
        
        internal void DoTextLine(InternalTextLineArgs args)
        {
            activeContinueCallback = args.ContinueCallback;
            
            IsRunning = true;
            hasContinued = false;

            if (handlers.Count <= 0)
            {
                Continue();
                return;
            }

            var publicArgs = new TextLineArgs
            {
                Uid = args.Uid,
                Speaker = args.Speaker,
                Text = args.Text,
                Meta = args.Meta,
            };
            
            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoTextLine(this, publicArgs);
            }
        }

        internal void DoActorAction(InternalActorArgs args)
        {
            activeContinueCallback = args.ContinueCallback;

            IsRunning = true;
            hasContinued = false;

            if (handlers.Count <= 0)
            {
                Continue();
                return;   
            }
            
            var publicArgs = new ActorArgs
            {
                Uid = args.Uid,
                Action = args.Action,
                Position = args.Position,
                Emotion = args.Emotion,
                Actor = args.Actor,
                Meta = args.Meta,
            };

            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoActorAction(this, publicArgs);
            }
        }

        public void AddHandler(IDialogueHandler handler)
        {
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }
        
        public void RemoveHandler(IDialogueHandler handler)
        {
            if (handlers.Contains(handler))
            {
                handlers.Remove(handler);
            }
        }
        
        private void OnSequenceStopped(object sender, SequenceDirectorEventArgs e)
        {
            if (IsRunning)
                return;

            Stopped?.Invoke(this, EventArgs.Empty);
        }

        private void OnSequenceCancelled(object sender, SequenceDirectorEventArgs e)
        {
            if (!IsRunning)
                return;
        
            IsRunning = false;
            
            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoCleanup(this);
            }
        
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        private void OnSequenceCompleted(object sender, SequenceDirectorEventArgs e)
        {
            if (!IsRunning)
                return;
            
            IsRunning = false;

            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoCleanup(this);
            }
            
            Completed?.Invoke(this, EventArgs.Empty);
        }

        private static DialogueDirector global;
        
        public static event Action GlobalChanged;
        
        public static DialogueDirector Global
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
    }
}