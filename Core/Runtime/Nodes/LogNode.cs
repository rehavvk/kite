using System;
using UnityEngine;

namespace Rehawk.Kite
{
    [Category("Debug")]
    [Icon("Info")]
    public class LogNode : NodeBase
    {
        [SerializeField] private LogMode mode;
        [SerializeField] private string text;

        public override string Title
        {
            get { return $"Log {mode}"; }
        }

        public override string Summary
        {
            get { return text; }
        }

        public override Color Color
        {
            get
            {
                switch (mode)
                {
                    case LogMode.Info:
                        return new Color32(184, 184, 184, 255);
                    case LogMode.Warning:
                        return new Color32(250, 193, 35, 255);
                    case LogMode.Error:
                        return new Color32(235, 51, 38, 255);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void OnEnter(Flow flow)
        {
            base.OnEnter(flow);

            switch (mode)
            {
                case LogMode.Info:
                    Debug.Log(text);
                    break;
                case LogMode.Warning:
                    Debug.LogWarning(text);
                    break;
                case LogMode.Error:
                    Debug.LogError(text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Continue(flow);
        }

        private enum LogMode
        {
            Info,
            Warning,
            Error
        }
    }
}