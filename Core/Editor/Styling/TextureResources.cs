using UnityEngine;

namespace Rehawk.Kite
{
    public static class TextureResources
    {
        static TextureResources()
        {
            GenerateSpecialTextures();
        }

        public static Texture2D WhiteBackground { get; private set; }

        public static Texture2D NodeBackgroundTexture { get; private set; }
        
        public static Texture2D TitleBackground { get; private set; }
        
        public static Texture2D ContainerBackground { get; private set; }

        public static Texture2D InspectorTitleBackgroundTexture { get; private set; }

        public static Texture2D InspectorUidBackgroundTexture { get; private set; }

        private static void GenerateSpecialTextures()
        {
            WhiteBackground = CreatePixelTexture("(Generated) White Background", Colors.WhiteBackground);
            NodeBackgroundTexture = CreatePixelTexture("(Generated) Node Background", Colors.NodeBackground);
            TitleBackground = CreatePixelTexture("(Generated) Title Background", Colors.TitleBackground);
            ContainerBackground = CreatePixelTexture("(Generated) Container Background", Colors.ContainerBackground);
            InspectorTitleBackgroundTexture = CreatePixelTexture("(Generated) Inspector Title Background", Colors.InspectorTitleBackground);
            InspectorUidBackgroundTexture = CreatePixelTexture("(Generated) Inspector Uid Background", Colors.InspectorSubtitleBackground);
        }

        public static Texture2D CreatePixelTexture(string name, Color color)
        {
            var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false, QualitySettings.activeColorSpace != ColorSpace.Linear);
            tex.name = name;
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.filterMode = FilterMode.Point;
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }
    }
}