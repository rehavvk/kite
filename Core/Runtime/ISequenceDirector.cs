using System;
using System.Collections;
using UnityEngine;

namespace Rehawk.Kite
{
    public interface ISequenceDirector
    {
        event EventHandler<SequenceDirectorEventArgs> Started;
        event EventHandler<SequenceDirectorEventArgs> Stopped;
        event EventHandler<SequenceDirectorEventArgs> Cancelled;
        event EventHandler<SequenceDirectorEventArgs> Completed;
        
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