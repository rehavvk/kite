namespace Rehawk.Kite
{
    [Category("Flow")]
    public class EndNode : NodeBase
    {
        public override string Title
        {
            get { return "END"; }
        }

        public override DecreaseIndentLevelMode DecreasesIndentLevel
        {
            get { return DecreaseIndentLevelMode.Self; }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            // Continue on same level.
            ContinueOnIndentLevel(flow, IndentLevel);
        }
    }
}