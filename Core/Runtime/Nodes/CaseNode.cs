namespace Rehawk.Kite
{
    [Category("Flow")]
    public class CaseNode : NodeBase
    {
        public sealed override IncreaseIndentLevelMode IncreasesIndentLevel
        {
            get { return IncreaseIndentLevelMode.Next; }
        }

        public override string Title
        {
            get { return "CASE"; }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            // Continue on next level.
            ContinueOnIndentLevel(flow, IndentLevel + 1);
        }
    }
}