namespace Rehawk.Kite.Signals
{
    public static class ISequenceDirectorExtensions
    {
        public static void InvokeSignal(this ISequenceDirector director, Signal signal)
        {
            signal.Invoke(director);
        }
    }
}