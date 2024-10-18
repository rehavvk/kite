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

        private InternalOptionArgs[] activeOptions;
        
        private ISequenceDirector sequenceDirector;
        
        public event Action<DialogueDirector> Started;
        public event Action<DialogueDirector> Stopped;
        public event Action<DialogueDirector> Cancelled;
        public event Action<DialogueDirector> Completed;

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
                        Started?.Invoke(this);
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
                activeOptions = null;
                activeContinueCallback.Invoke();
            }
        }

        public void Choose(int optionIndex)
        {
            if (!IsRunning)
                return;

            if (activeOptions != null && !hasContinued && 
                optionIndex >= 0 && optionIndex < activeOptions.Length)
            {
                InternalOptionArgs option = activeOptions[optionIndex];
                activeOptions = null;
                
                hasContinued = true;
                option.ContinueCallback.Invoke();
            }
        }

        private void Cancel()
        {
            // TODO: What should happen?
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
                ContinueModes = args.ContinueModes,
                Meta = args.Meta,
            };
            
            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoTextLine(this, publicArgs);
            }
        }

        internal void DoChoice(InternalChoiceArgs args)
        {
            activeContinueCallback = args.ContinueCallback;
            activeOptions = args.Options;
            
            IsRunning = true;
            hasContinued = false;

            if (handlers.Count <= 0 || args.Options.Length <= 0)
            {
                Continue();
                return;    
            }
            
            var optionArgs = new OptionArgs[args.Options.Length];
            for (int i = 0; i < args.Options.Length; i++)
            {
                InternalOptionArgs internalArgs = args.Options[i];

                optionArgs[i] = new OptionArgs
                {
                    Uid = internalArgs.Uid,
                    Speaker = internalArgs.Speaker,
                    Text = internalArgs.Text,
                    Meta = internalArgs.Meta,
                };
            }
            
            var publicArgs = new ChoiceArgs
            {
                Uid = args.Uid,
                Options = optionArgs,
                AutoChoose = args.AutoChoose,
                AutoChooseOptionIndex = args.AutoChooseOptionIndex,
                Meta = args.Meta,
            };
            
            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoChoice(this, publicArgs);
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
        
        private void OnSequenceStopped(ISequenceDirector director, Flow flow, Sequence sequence)
        {
            if (IsRunning)
                return;

            Stopped?.Invoke(this);
        }

        private void OnSequenceCancelled(ISequenceDirector director, Flow flow, Sequence sequence)
        {
            if (!IsRunning)
                return;
        
            IsRunning = false;
            activeOptions = null;
            
            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoCleanup(this);
            }
        
            Cancelled?.Invoke(this);
        }

        private void OnSequenceCompleted(ISequenceDirector director, Flow flow, Sequence sequence)
        {
            if (!IsRunning)
                return;
            
            IsRunning = false;
            activeOptions = null;

            foreach (IDialogueHandler handler in handlers)
            {
                handler.DoCleanup(this);
            }
            
            Completed?.Invoke(this);
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