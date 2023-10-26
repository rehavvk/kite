using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    public abstract class SayNodePreviewTargetBase : MonoBehaviour
    {
        public abstract void DoTextLine(string text, ActorBase speaker);
    }
}