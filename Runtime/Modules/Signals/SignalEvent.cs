namespace Rehawk.Kite.Signals
{
    public struct SignalEventArgs
    {
        public Signal Signal { get; set; }
        public object Payload { get; set; }
    }
}