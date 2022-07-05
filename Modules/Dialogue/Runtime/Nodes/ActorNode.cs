﻿using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    [Category("Dialogue")]
    [Icon("Actor")]
    public class ActorNode : NodeBase
    {
        [ActorPosition]
        [SerializeField] private int position;
        [SerializeField] private ActorAction action;
        [ShowIf("action", ActorAction.Update)]
        [SerializeField] private Actor actor;
        [ActorEmotion("action", ActorAction.Update)]
        [SerializeField] private int emotion;

        [Space]
        
        [TextArea(2, int.MaxValue)] 
        [SerializeField] private string meta;

        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                string actorName = actor ? actor.name : string.Empty;
                string emotionName = KiteDialogueSettings.GetEmotionName(emotion);
                string positionName = KiteDialogueSettings.GetPositionName(position);

                summary += $"<b>{positionName}</b>";
                
                if (action == ActorAction.Clear)
                {
                    summary += $" is cleared";
                }
                else
                {
                    summary += $" is changed to";
                        
                    if (emotionName.ToLower() != "default")
                    {
                        summary += $" <b>{emotionName}</b>";
                    }
                        
                    summary += $" <b>{actorName}</b>";
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
                    Position = position,
                    Emotion = emotion,
                    Actor = actor,
                    Meta = meta,
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