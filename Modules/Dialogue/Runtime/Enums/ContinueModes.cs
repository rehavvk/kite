using System;

namespace Rehawk.Kite.Dialogue
{
    [Flags]
    public enum ContinueModes
    {
        Manual = 1 << 0,
        Auto = 1 << 1,
    }
}