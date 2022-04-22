using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Utils")]
    [Icon("Comment")]
    public class CommentNode : NodeBase
    {
        [TextArea]
        [SerializeField] private string text = "My Comment";

        public override string Summary
        {
            get { return text; }
        }
        
        public override Color Color
        {
            get { return new Color32(207, 173, 64, 255); }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            Continue(flow);
        }
    }
}