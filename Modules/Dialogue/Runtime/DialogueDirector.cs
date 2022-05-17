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
            sequenceDirector.Completed += OnSequenceCompleted;
            
            if (isGlobal)
            {
                Global = this;
            }
        }

        private void OnDestroy()
        { 
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

        // TODO: reimplement
        private void Cancel()
        {
            if (!IsRunning)
                return;

            IsRunning = false;
            
            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoCleanup(this);
            }

            // Canceled?.Invoke(this, EventArgs.Empty);
        }
        
        private void Complete()
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
        
        internal void DoTextLine(TextLineArgs args)
        {
            activeContinueCallback = args.ContinueCallback;
            
            IsRunning = true;
            hasContinued = false;

            if (handlers.Count <= 0)
            {
                Continue();
                return;
            }

            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoTextLine(this, args);
            }
        }

        internal void DoActorAction(ActorArgs args)
        {
            activeContinueCallback = args.ContinueCallback;

            IsRunning = true;
            hasContinued = false;

            if (handlers.Count <= 0)
            {
                Continue();
                return;   
            }
            
            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoActorAction(this, args);
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
        
        private void OnSequenceCompleted(object sender, SequenceDirectorEventArgs e)
        {
            Complete();
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