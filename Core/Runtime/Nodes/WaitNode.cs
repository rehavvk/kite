using System.Collections;
using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Flow")]
    [Name("Wait For")]
    [Icon("WaitFor")]
    public class WaitNode : NodeBase
    {
        [SerializeField] private float seconds;

        public override string Title
        {
            get { return "WAIT FOR"; }
        }

        public override string Summary
        {
            get { return $"{seconds} s"; }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            flow.StartCoroutine(this, "wait_routine", ContinueDelayed(seconds, flow));
        }

        protected override void OnInterrupt(Flow flow)
        {
            base.OnInterrupt(flow);

            flow.StopCoroutine(this, "wait_routine");
        }

        private IEnumerator ContinueDelayed(float delay, Flow flow)
        {
            yield return new WaitForSeconds(delay);
            Continue(flow);
        }
    }
}