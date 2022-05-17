using System;
using System.Collections;
using UnityEngine;

namespace Rehawk.Kite
{
    public interface ISequenceDirector
    {
        event EventHandler<SequenceDirectorEventArgs> Started;
        event EventHandler<SequenceDirectorEventArgs> Completed;
        
        VariableContainer Variables { get; }
        
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);

        bool TryGetHostObject(out GameObject result);
        
        void SetValue<T>(object owner, string key, T value);
        T GetValue<T>(object owner, string key, T fallback = default);
        bool TryGetValue<T>(object owner, string key, out T result);

        void Log(LogLevel level, string message, int originIndex, NodeBase originNode);
    }
}