namespace Rehawk.Kite.Dialogue
{
    public class ActorEmotionAttribute : ShowIfAttribute
    {
        public ActorEmotionAttribute() : base("", null) {}
        public ActorEmotionAttribute(string memberName, object comparedValue) : base(memberName, comparedValue) {}
    }
}