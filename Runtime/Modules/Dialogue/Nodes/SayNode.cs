using System.Text.RegularExpressions;
using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    [Category("Dialogue")]
    [Icon("Say")]
    public class SayNode : NodeBase
    {
        [SerializeField] private Actor speaker;

        [TextArea(10, int.MaxValue)] 
        [SerializeField] private string text;

        [TextArea(2, int.MaxValue)] 
        [SerializeField] private string meta;

        [HideInInspector]
        [SerializeField] private string beautifiedText;
        
        public override string Summary
        {
            get
            {
                string summary = string.Empty;

                if (speaker)
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

        protected override void OnEnter(Flow flow)
        {
            if (flow.Director.TryGetHostObject(out GameObject obj) && obj.TryGetComponent(out DialogueDirector dialogueDirector))
            {
                dialogueDirector.DoTextLine(new TextLineArgs
                {
                    Id = Uid,
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
                // Removed tags.
                beautifiedText = Regex.Replace(beautifiedText, "<[^>]*>", string.Empty);
                beautifiedText = Regex.Replace(beautifiedText, "{[^}]*}", string.Empty);
            }
        }
    }
}