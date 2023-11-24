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
        public ContinueModes ContinueModes { get; set; }
        public string Meta { get; set; }
        
        public Action ContinueCallback { get; set; }
    }
    
    public class TextLineArgs
    {
        /// <summary>
        /// An unique identifier for the operation. Can be used for localization etc.
        /// </summary>
        public string Uid { get; set; }
        
        public ActorBase Speaker { get; set; }
        public string Text { get; set; }
        public ContinueModes ContinueModes { get; set; }
        public string Meta { get; set; }
    }
    
    internal class InternalChoiceArgs
    {
        /// <summary>
        /// An unique identifier for the operation. Can be used for localization etc.
        /// </summary>
        public string Uid { get; set; }
        
        public InternalOptionArgs[] Options { get; set; }
        public bool AutoChoose { get; set; }
        public int AutoChooseOptionIndex { get; set; }
        public string Meta { get; set; }

        public Action ContinueCallback { get; set; }
    }
    
    public class ChoiceArgs
    {
        /// <summary>
        /// An unique identifier for the operation. Can be used for localization etc.
        /// </summary>
        public string Uid { get; set; }
        
        public OptionArgs[] Options { get; set; }
        public bool AutoChoose { get; set; }
        public int AutoChooseOptionIndex { get; set; }
        public string Meta { get; set; }
    }

    internal class InternalOptionArgs
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
    
    public class OptionArgs
    {
        /// <summary>
        /// An unique identifier for the operation. Can be used for localization etc.
        /// </summary>
        public string Uid { get; set; }
        
        public ActorBase Speaker { get; set; }
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
        public ActorBase Actor { get; set; }
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
        public ActorBase Actor { get; set; }
        public string Meta { get; set; }
    }
}