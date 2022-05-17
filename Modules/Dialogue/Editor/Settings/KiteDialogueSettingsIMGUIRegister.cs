﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    internal static class KiteDialogueSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateKiteDialogueSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Kite/Dialogue", SettingsScope.Project)
            {
                label = "Dialogue",
                
                guiHandler = searchContext =>
                {
                    SerializedObject settings = KiteDialogueSettingsReceiver.GetSerializedSettings();
                    
                    EditorGUILayout.PropertyField(settings.FindProperty("positions"), new GUIContent("Positions"));
                    EditorGUILayout.PropertyField(settings.FindProperty("emotions"), new GUIContent("Emotions"));
                },

                keywords = new HashSet<string>(new[] { "Positions", "Emotions" })
            };

            return provider;
        }
    }
}