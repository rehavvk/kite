using System;

namespace Rehawk.Kite.Dialogue
{
    internal class InternalTextLineArgs
    {
        /// <summary>
        /// An unique identifier for the operation. Can be used for localization etc.
        /// </summary>
        public string Uid { get; set; }
        
        public ActorBase Speaker { get; set; }
        public string Text { get; set; }
        public string Meta { get; set; }
        
        public Action ContinueCallback { get; set; }
    }
    
    public class TextLineArgs
    {
        /// <summary>
        /// An unique identifier for the operation. Can be used for localization etc.
        /// </summary>
        public string Uid { get; set; }
        
        public IActor Speaker { get; set; }
        public string Text { get; set; }
        public string Meta { get; set; }
    }
    
    internal class InternalActorArgs
    {
        /// <summary>
        /// An unique identifier for the operation. Can be used for localization etc.
        /// </summary>
        public string Uid { get; set; }
        
        public int Position { get; set; }
        public ActorAction Action { get; set; }
        public int Emotion { get; set; }
        public IActor Actor { get; set; }
        public string Meta { get; set; }
        
        public Action ContinueCallback { get; set; }
    }
    
    public class ActorArgs
    {
        /// <summary>
        /// An unique identifier for the operation. Can be used for localization etc.
        /// </summary>
        public string Uid { get; set; }
        
        public int Position { get; set; }
        public ActorAction Action { get; set; }
        public int Emotion { get; set; }
        public IActor Actor { get; set; }
        public string Meta { get; set; }
    }
}