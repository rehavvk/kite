namespace Rehawk.Kite
{
    public abstract class ConditionNodeBase : NodeBase
    {
        public sealed override IncreaseIndentLevelMode IncreasesIndentLevel
        {
            get { return IncreaseIndentLevelMode.Next; }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            if (EvaluateCondition(flow))
            {
                // Continue on next level
                ContinueOnIndentLevel(flow, IndentLevel + 1);
            }
            else
            {
                // Continue on same level (else or end)
                ContinueOnIndentLevel(flow, IndentLevel);
            }
        }

        protected abstract bool EvaluateCondition(Flow flow);
    }
}