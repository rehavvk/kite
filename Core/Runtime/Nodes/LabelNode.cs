using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Flow")]
    public class LabelNode : NodeBase
    {
        [SerializeField] private string labelID;

        public override string Summary
        {
            get { return string.IsNullOrEmpty(labelID) ? "-" : $"'{labelID}'"; }
        }

        public string LabelID
        {
            get { return labelID; }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            Continue(flow);
        }
    }
}