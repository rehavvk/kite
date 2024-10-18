using System;
using System.Collections;
using UnityEngine;

namespace Rehawk.Kite
{
    public delegate void SequenceEventDelegate(ISequenceDirector director, Flow flow, Sequence sequence);

    public interface ISequenceDirector
    {
        event SequenceEventDelegate Started;
        event SequenceEventDelegate Stopped;
        event SequenceEventDelegate Cancelled;
        event SequenceEventDelegate Completed;
        
        VariableContainer Variables { get; }
        
        Coroutine RunCoroutine(IEnumerator routine);
        void CancelCoroutine(Coroutine routine);

        bool TryGetHostObject(out GameObject result);
        
        void SetValue<T>(object owner, string key, T value);
        T GetValue<T>(object owner, string key, T fallback = default);
        bool TryGetValue<T>(object owner, string key, out T result);

        void Log(LogLevel level, string message, int originIndex, NodeBase originNode);
    }
}