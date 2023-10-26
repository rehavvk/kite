namespace Rehawk.Kite.Dialogue
{
    public class ActorPositionAttribute : ShowIfAttribute
    {
        public ActorPositionAttribute() : base("", null) {}
        public ActorPositionAttribute(string propertyName, object comparedValue) : base(propertyName, comparedValue) {}
    }
}