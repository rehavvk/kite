using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    [Category("Dialogue")]
    public class OptionNode : NodeBase
    {
        [TextArea(10, int.MaxValue)]
        [SerializeField] private string text;

        [TextArea(2, int.MaxValue)] 
        [SerializeField] private string meta;

        [HideInInspector]
        [SerializeField] private string beautifiedText;
        
        public sealed override IncreaseIndentLevelMode IncreasesIndentLevel
        {
            get { return IncreaseIndentLevelMode.Next; }
        }

        public override string Title
        {
            get { return "OPTION"; }
        }

        public override string Summary
        {
            get { return beautifiedText; }
        }

        public override Color Color
        {
            get { return new Color32(121, 173, 84, 255); }
        }

        public string Text
        {
            get { return text; }
        }
        
        public string Meta
        {
            get { return meta; }
        }
        
        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            // Continue on next level.
            ContinueOnIndentLevel(flow, IndentLevel + 1);
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