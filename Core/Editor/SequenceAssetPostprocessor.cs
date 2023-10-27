using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    public class SequenceAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);

            Sequence[] sequences = AssetHelper.FindAssetsOfType<Sequence>();

            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (typeof(Sequence).IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(importedAssets[i])))
                {
                    var importedSequence = AssetDatabase.LoadAssetAtPath<Sequence>(importedAssets[i]);

                    if (string.IsNullOrEmpty(importedSequence.Guid))
                    {
                        importedSequence.Guid = Guid.NewGuid().ToString();
                        EditorUtility.SetDirty(importedSequence);
                    }
                    else
                    {
                        Sequence sequenceWithSameGuid = sequences.FirstOrDefault(s => s != importedSequence && s.Guid == importedSequence.Guid);
                        if (sequenceWithSameGuid != null && sequenceWithSameGuid != importedSequence)
                        {
                            var importedCreationTime = File.GetCreationTimeUtc(importedAssets[i]);

                            string pathToLoadedSequence = Path.Combine(projectPath, AssetDatabase.GetAssetPath(sequenceWithSameGuid));
                            
                            var loadedCreationTime = File.GetCreationTimeUtc(pathToLoadedSequence);

                            if (importedCreationTime >= loadedCreationTime)
                            {
                                importedSequence.Guid = Guid.NewGuid().ToString();
                                EditorUtility.SetDirty(importedSequence);
                            }
                            else
                            {
                                sequenceWithSameGuid.Guid = Guid.NewGuid().ToString();
                                EditorUtility.SetDirty(sequenceWithSameGuid);
                            }
                        }
                    }
                }
            }
        }
    }
}