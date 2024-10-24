using System;
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
        
        [Space]
        
        [SerializeField] private bool once;
        [Indent(1), ShowIf(nameof(once), true), Tooltip("If TRUE also later flows will remember the state of the once node.")]
        [SerializeField] private bool persistant = true;
        
        [Space]

        [SerializeField] private ConditionEvaluationMode mode;
        [SerializeField] private bool invert;
        
        [SubclassSelector]
        [SerializeReference] private ConditionBase[] conditions = Array.Empty<ConditionBase>();

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

        public bool Evaluate(Flow flow)
        {
            if (!once || (!TryGetNodeValue(flow, "was_invoked_before", out bool wasInvokedBefore) || !wasInvokedBefore))
            {
                return ConditionUtility.Evaluate(flow, conditions, mode, invert);
            }

            return false;
        }
            
        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            // Continue on next level.
            ContinueOnIndentLevel(flow, IndentLevel + 1);
        }

        protected override void OnExit(Flow flow)
        {
            base.OnExit(flow);
            
            SetNodeValue(flow, "was_invoked_before", true, persistant);
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