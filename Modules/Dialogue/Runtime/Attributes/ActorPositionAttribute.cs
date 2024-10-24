namespace Rehawk.Kite.Dialogue
{
    public class ActorPositionAttribute : ShowIfAttribute
    {
        public ActorPositionAttribute() : base("", null) {}
        public ActorPositionAttribute(string memberName, object comparedValue) : base(memberName, comparedValue) {}
    }
}