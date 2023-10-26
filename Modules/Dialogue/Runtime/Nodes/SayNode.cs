using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    [Category("Dialogue")]
    [Icon("Say")]
    public class SayNode : NodeBase
    {
        [SerializeField] private ActorBase speaker;

        [TextArea(10, int.MaxValue)]
        [SerializeField] private string text;

        [TextArea(2, int.MaxValue)] 
        [SerializeField] private string meta;

        [HideInInspector]
        [SerializeField] private string beautifiedText;
        
        public override int LeftMargin
        {
            get { return 20; }
        }

        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                if (speaker != null)
                {
                    summary += $"<b>{speaker.name}</b>\n";
                }

                summary += beautifiedText;
                
                return summary;
            }
        }

        public override Color Color
        {
            get { return new Color32(121, 173, 84, 255); }
        }

        public ActorBase Speaker
        {
            get { return speaker; }
        }
        
        public string Text
        {
            get { return text; }
        }
        
        protected override void OnEnter(Flow flow)
        {
            if (flow.Director.TryGetHostObject(out GameObject obj) && obj.TryGetComponent(out DialogueDirector dialogueDirector))
            {
                dialogueDirector.DoTextLine(new InternalTextLineArgs
                {
                    Uid = flow.Sequence.Guid + "_" + Guid,
                    Speaker = speaker,
                    Text = text,
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

        protected override void OnValidate(Sequence sequence)
        {
            base.OnValidate(sequence);

            beautifiedText = text;
            
            if (!string.IsNullOrEmpty(beautifiedText))
            {
                beautifiedText = StringUtils.RemoveTags(beautifiedText);
            }
        }
    }
}