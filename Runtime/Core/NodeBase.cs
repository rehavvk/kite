using System;
using UnityEngine;

namespace Rehawk.Kite
{
    [Serializable]
    public abstract class NodeBase : ICloneable
    {
        public enum DecreaseIndentLevelMode
        {
            None,
            Self,
            Next
        }

        public enum IncreaseIndentLevelMode
        {
            None,
            Self,
            Next
        }

        [HideInInspector] 
        [SerializeField] private int indentLevel;

        [HideInInspector] 
        [SerializeField] private int index;

        [HideInInspector] 
        [SerializeField] private int persistantIndex = -1;

        [HideInInspector] 
        [SerializeField] private string uid;

        public string Uid
        {
            get { return uid; }
            set { uid = value; }
        }

        public int PersistantIndex
        {
            get { return persistantIndex; }
            set { persistantIndex = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public int IndentLevel
        {
            get { return indentLevel; }
            set { indentLevel = value; }
        }

        public virtual int LeftMargin
        {
            get { return 0; }
        }

        public virtual IncreaseIndentLevelMode IncreasesIndentLevel
        {
            get { return IncreaseIndentLevelMode.None; }
        }

        public virtual DecreaseIndentLevelMode DecreasesIndentLevel
        {
            get { return DecreaseIndentLevelMode.None; }
        }

        public virtual string Title
        {
            get { return GetType().Name.Replace("Node", string.Empty).SplitCamelCase(); }
        }

        public virtual string Summary
        {
            get { return string.Empty; }
        }

        public virtual Color Color
        {
            get { return Color.clear; }
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        #region Execution

        public void FlowStarted(Flow flow)
        {
            OnFlowStarted(flow);
        }

        public void FlowStopped(Flow flow)
        {
            OnFlowStopped(flow);
        }

        public void Enter(Flow flow)
        {
            OnEnter(flow);
        }

        public void Exit(Flow flow)
        {
            OnExit(flow);
        }

        public void Interrupt(Flow flow)
        {
            OnInterrupt(flow);
        }

        protected void Continue(Flow flow)
        {
            flow.Continue(Index + 1);
        }

        protected void ContinueWithIndex(Flow flow, int index)
        {
            flow.Continue(index);
        }

        protected void ContinueOnIndentLevel(Flow flow, int indentLevel)
        {
            if (flow.Sequence.TryGetIndexByIndentLevel(Index + 1, indentLevel, out int nextIndex))
            {
                flow.Continue(nextIndex);
            }
            else
            {
                // Dead end. Jump to the very end to stop the flow.
                flow.Continue(int.MaxValue);
            }
        }

        /// <summary>
        ///     Is called when the flow starts.
        /// </summary>
        protected virtual void OnFlowStarted(Flow flow)
        {
        }

        /// <summary>
        ///     Is called when the flow stops.
        /// </summary>
        protected virtual void OnFlowStopped(Flow flow)
        {
        }

        /// <summary>
        ///     Is called when the flow steps forward to this node.
        /// </summary>
        protected virtual void OnEnter(Flow flow)
        {
        }

        /// <summary>
        ///     Is called when the flow steps forward to the next node.
        /// </summary>
        protected virtual void OnExit(Flow flow)
        {
        }

        /// <summary>
        ///     Is called on the active node when the flow is stopped.
        /// </summary>
        protected virtual void OnInterrupt(Flow flow)
        {
        }

        #endregion

        #region Setup

        public void Validate(Sequence sequence)
        {
            OnValidate(sequence);
        }

        /// <summary>
        ///     Is called in the editor to validate the sequence and its nodes.
        /// </summary>
        protected virtual void OnValidate(Sequence sequence)
        {
        }

        #endregion
    }
}