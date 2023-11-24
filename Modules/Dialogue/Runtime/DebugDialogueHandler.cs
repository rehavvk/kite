using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    public class DebugDialogueHandler : MonoBehaviour, IDialogueHandler
    {
        [SerializeField] private DialogueDirector director;

        private bool inChoice;
        private OptionArgs[] options;
        
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
            if (director.IsRunning)
            {
                if (GUI.Button(new Rect(10, 10, 200, 18), "Continue"))
                {
                    director.Continue();
                }

                if (inChoice)
                {
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (GUI.Button(new Rect(10, 40 + (20 * i), 200, 18), $"Option {i}"))
                        {
                            options = null;
                            inChoice = false;
                            
                            director.Choose(i);
                            
                            break;
                        }
                    }
                }
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

        void IDialogueHandler.DoChoice(DialogueDirector director, ChoiceArgs args)
        {
            inChoice = true;
            options = args.Options;
            
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("CHOICE:");

            for (int i = 0; i < options.Length; i++)
            {
                OptionArgs option = options[i];
                
                string beautifiedText = option.Text;
            
                if (!string.IsNullOrEmpty(beautifiedText))
                {
                    // Removed tags.
                    beautifiedText = Regex.Replace(beautifiedText, "<[^>]*>", string.Empty);
                    beautifiedText = Regex.Replace(beautifiedText, "{[^}]*}", string.Empty);
                }

                stringBuilder.AppendLine($"{i} => {option.Speaker.name}:\n\n{beautifiedText}");
                stringBuilder.AppendLine();
            }
            
            Debug.Log(stringBuilder.ToString());
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