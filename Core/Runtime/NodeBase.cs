using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("uid")]
        [SerializeField] private string guid;

        public string Guid
        {
            get { return guid; }
            set { guid = value; }
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

        public virtual Texture2D Icon
        {
            get { return null; }
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        #region Execution

        public void Initialized()
        {
            OnInitialized();
        }
        
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

        protected void Complete(Flow flow)
        {
            flow.Complete();
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
        
        protected void Log(Flow flow, LogLevel level, string message, NodeBase node)
        {
            flow.Log(level, message, node);
        }
        
        protected void SetValue<T>(Flow flow, string key, T value, bool persistant = false)
        {
            flow.SetValue(key, value, persistant);
        }

        protected T GetValue<T>(Flow flow, string key, T fallback = default)
        {
            return flow.GetValue(key, fallback);
        }

        protected bool TryGetValue<T>(Flow flow, string key, out T result)
        {
            return flow.TryGetValue(key, out result);
        }

        protected void SetNodeValue<T>(Flow flow, string key, T value, bool persistant = false)
        {
            flow.SetValueInternal(this, key, value, persistant);
        }

        protected T GetNodeValue<T>(Flow flow, string key, T fallback = default)
        {
            return flow.GetValueInternal(this, key, fallback);
        }

        protected bool TryGetNodeValue<T>(Flow flow, string key, out T result)
        {
            return flow.TryGetValueInternal(this, key, out result);
        }

        protected Coroutine StartCoroutine(Flow flow, NodeBase ownerNode, IEnumerator routine)
        {
            return flow.StartCoroutine(ownerNode, routine);
        }

        protected void StartCoroutine(Flow flow, NodeBase ownerNode, string key, IEnumerator routine)
        {
            flow.StartCoroutine(ownerNode, key, routine);
        }

        protected void StopCoroutine(Flow flow, NodeBase ownerNode, Coroutine routine)
        {
            flow.StopCoroutine(ownerNode, routine);
        }

        protected void StopCoroutine(Flow flow, NodeBase ownerNode, string key)
        {
            flow.StopCoroutine(ownerNode, key);
        }

        protected void StopAllCoroutines(Flow flow, NodeBase ownerNode)
        {
            flow.StopAllCoroutines(ownerNode);
        }
        
        /// <summary>
        ///     Is called when the sequence is used the first time.
        /// </summary>
        protected virtual void OnInitialized()
        {
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