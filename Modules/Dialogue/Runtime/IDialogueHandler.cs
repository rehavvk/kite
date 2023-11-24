using System;

namespace Rehawk.Kite.Dialogue
{
    public interface IDialogueHandler
    {
        void DoTextLine(DialogueDirector director, TextLineArgs args);
        void DoChoice(DialogueDirector director, ChoiceArgs args);
        void DoActorAction(DialogueDirector director, ActorArgs args);
        void DoCleanup(DialogueDirector director);
    }
}