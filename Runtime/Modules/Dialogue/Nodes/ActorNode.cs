﻿using System;
using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    [Category("Dialogue")]
    [Icon("Actor")]
    public class ActorNode : NodeBase
    {
        [SerializeField] private ActorAction action;
        
        [Min(0)]
        [SerializeField] private int position;
        [SerializeField] private Actor actor;
        [SerializeField] private Emotion emotion;

        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                string actorName = actor ? actor.name : string.Empty;
                string emotionName = emotion.ToString().ToLower();
                
                switch (action)
                {
                    case ActorAction.Join:
                        summary += $"<b>{actorName}</b> joins";

                        if (emotion != Emotion.Default)
                        {
                            summary += $" <b>{emotionName}</b>";
                        }
                        
                        summary += $" at position <b>{position}</b>";

                        break;
                    case ActorAction.Update:
                        summary += $"<b>{actorName}</b> changes";

                        if (emotion != Emotion.Default)
                        {
                            summary += $" to <b>{emotionName}</b>";
                        }
                        
                        summary += $" at position <b>{position}</b>";

                        break;
                    case ActorAction.Leave:
                        summary += $"<b>{actorName}</b> leaves";

                        if (emotion != Emotion.Default)
                        {
                            summary += $" <b>{emotionName}</b>";
                        }
                        
                        summary += $" position <b>{position}</b>";
                        
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return summary;
            }
        }

        public override Color Color
        {
            get { return new Color32(255, 128, 0, 255); }
        }

        protected override void OnEnter(Flow flow)
        {
            if (flow.Director.TryGetHostObject(out GameObject obj) && obj.TryGetComponent(out DialogueDirector dialogueDirector))
            {
                dialogueDirector.DoActorAction(new ActorArgs
                {
                    Id = Uid,
                    Action = action,
                    Actor = actor,
                    Emotion = emotion,
                    Position = position,
                    ContinueCallback = () =>
                    {
                        Continue(flow);
                    }
                });
            }
            else
            {
                flow.Log(LogLevel.Error, "DialogueDirector couldn't be found. Node was skipped.", this);
                Continue(flow);
            }
        }
    }
}