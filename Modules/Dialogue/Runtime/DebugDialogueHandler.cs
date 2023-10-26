using System.Text.RegularExpressions;
using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    public class DebugDialogueHandler : MonoBehaviour, IDialogueHandler
    {
        [SerializeField] private DialogueDirector director;

        private void Awake()
        {
            director.AddHandler(this);
        }

        private void OnDestroy()
        {
            director.RemoveHandler(this);
        }

        private void OnGUI()
        {
            if (director.IsRunning && GUI.Button(new Rect(10, 10, 200, 18), "Continue"))
            {
                director.Continue();
            }
        }

        void IDialogueHandler.DoTextLine(DialogueDirector director, TextLineArgs args)
        {
            string beautifiedText = args.Text;
            
            if (!string.IsNullOrEmpty(beautifiedText))
            {
                // Removed tags.
                beautifiedText = Regex.Replace(beautifiedText, "<[^>]*>", string.Empty);
                beautifiedText = Regex.Replace(beautifiedText, "{[^}]*}", string.Empty);
            }
            
            Debug.Log($"SAY\n\n{args.Speaker.name}:\n{beautifiedText}");
        }

        void IDialogueHandler.DoActorAction(DialogueDirector director, ActorArgs args)
        {
            Debug.Log($"ACTOR\n\n{args.Position} {args.Emotion} {args.Actor.name}");
            director.Continue();
        }

        void IDialogueHandler.DoCleanup(DialogueDirector director)
        {
            Debug.Log("CLEANUP");
        }
    }
}