using System;

namespace Rehawk.Kite.Dialogue
{
    public interface IDialogueHandler
    {
        void DoTextLine(DialogueDirector director, TextLineArgs args);
        void DoActorAction(DialogueDirector director, ActorArgs args);
        void DoCleanup(DialogueDirector director);
    }

    public class TextLineArgs
    {
        public string Id { get; set; }
        public Actor Speaker { get; set; }
        public string Text { get; set; }
        public string Meta { get; set; }
        
        public Action ContinueCallback { get; set; }
    }
    
    public class ActorArgs
    {
        public string Id { get; set; }
        public int Position { get; set; }
        public int Emotion { get; set; }
        public Actor Actor { get; set; }
        public string Meta { get; set; }
        
        public Action ContinueCallback { get; set; }
    }
}