using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    public static class KiteDialogueSettingsReceiver
    {
        private static KiteDialogueSettings GetOrCreateSettings()
        {
            return KiteDialogueSettings.Instance;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}