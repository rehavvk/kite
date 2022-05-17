using UnityEngine;

namespace Rehawk.Kite.Dialogue
{
    public class KiteDialogueSettings : ScriptableObject
    {
        private const string ASSET_PATH = "Assets/Resources/KiteDialogueSettings.asset";
        
        [SerializeField] private string[] positions =
        {
            "Position 1",
            "Position 2",
            "Position 3",
            "Position 4"
        };
        
        [SerializeField] private string[] emotions =
        {
            "Default",
            "Joyful",
            "Angry",
            "Surprised",
            "Contemptuous",
            "Sad"
        };

        private static KiteDialogueSettings instance;
        
        public static KiteDialogueSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<KiteDialogueSettings>("KiteDialogueSettings");

#if UNITY_EDITOR
                    if (instance == null)
                    {
                        instance = CreateInstance<KiteDialogueSettings>();
                
                        UnityEditor.AssetDatabase.CreateAsset(instance, ASSET_PATH);
                        UnityEditor.AssetDatabase.SaveAssets();
                    }
#endif
                }

                return instance;
            }
        }
        
        public static string[] Positions
        {
            get { return Instance.positions; }
        }
        
        public static string[] Emotions
        {
            get { return Instance.emotions; }
        }

        public static string GetPositionName(int index)
        {
            if (index >= 0 && index < Instance.positions.Length)
            {
                return Instance.positions[index];
            }

            return string.Empty;
        }
        
        public static string GetEmotionName(int index)
        {
            if (index >= 0 && index < Instance.emotions.Length)
            {
                return Instance.emotions[index];
            }

            return string.Empty;
        }
    }
}