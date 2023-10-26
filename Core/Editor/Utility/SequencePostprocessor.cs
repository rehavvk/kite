using System;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    public class SequencePostprocessor : AssetPostprocessor
    {
        private static bool hasCopiedSequence;

        static SequencePostprocessor()
        {
            hasCopiedSequence = false;

            EditorApplication.projectWindowItemOnGUI += (guid, rect) =>
            {
                if (Event.current.commandName == "Duplicate")
                {
                    Sequence sequence = AssetDatabase.LoadAssetAtPath<Sequence>(AssetDatabase.GUIDToAssetPath(guid));
                    if (sequence)
                    {
                        hasCopiedSequence = true;
                    }
                }
            };
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (hasCopiedSequence)
            {
                foreach (string path in importedAssets)
                {
                    Sequence sequence = AssetDatabase.LoadAssetAtPath<Sequence>(path);
                    if (sequence)
                    {
                        for (int i = 0; i < sequence.Count; i++)
                        {
                            sequence[i].Guid = Guid.NewGuid().ToString();
                        }
                    }
                }
            }

            hasCopiedSequence = false;
        }
    }
}