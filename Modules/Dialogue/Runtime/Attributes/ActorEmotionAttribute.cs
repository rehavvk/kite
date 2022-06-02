namespace Rehawk.Kite.Dialogue
{
    public class ActorEmotionAttribute : ShowIfAttribute
    {
        public ActorEmotionAttribute() : base("", null) {}
        public ActorEmotionAttribute(string propertyName, object comparedValue) : base(propertyName, comparedValue) {}
    }
}