using System;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    public enum Textures
    {
        TitleBackground,
        ContainerBackground,
    }

    public static class TextureResources
    {
        static TextureResources()
        {
            GenerateSpecialTextures();
            LoadResourceAssets();
        }

        #region Texture Resources

        private static string[] s_skin =
        {
            "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAYAAABGM/VAAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAACZJREFUeNpi/P//vxQDGmABEffv3/8ME1BUVORlYsACGLFpBwgwABaWCjfQEetnAAAAAElFTkSuQmCC",
            "iVBORw0KGgoAAAANSUhEUgAAAAkAAAAFCAYAAACXU8ZrAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAACRJREFUeNpizM3N/c9AADAqKysTVMTi5eXFSFAREFPHOoAAAwBCfwcAO8g48QAAAABJRU5ErkJggg==",
        };

        public static Texture2D GetTexture(Textures name)
        {
            return s_Cached[(int)name];
        }

        #endregion

        #region Generated Resources

        public static Texture2D WhiteBackground { get; private set; }

        public static Texture2D NodeBackgroundTexture { get; private set; }

        public static Texture2D InspectorTitleBackgroundTexture { get; private set; }

        public static Texture2D InspectorUidBackgroundTexture { get; private set; }

        private static void GenerateSpecialTextures()
        {
            WhiteBackground = CreatePixelTexture("(Generated) White Background", Colors.WhiteBackground);
            NodeBackgroundTexture = CreatePixelTexture("(Generated) Node Background", Colors.NodeBackground);
            InspectorTitleBackgroundTexture = CreatePixelTexture("(Generated) Inspector Title Background", Colors.InspectorTitleBackground);
            InspectorUidBackgroundTexture = CreatePixelTexture("(Generated) Inspector Uid Background", Colors.InspectorSubtitleBackground);
        }

        public static Texture2D CreatePixelTexture(string name, Color color)
        {
            var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
            tex.name = name;
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.filterMode = FilterMode.Point;
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        #endregion

        #region Load PNG from Base-64 Encoded String

        private static Texture2D[] s_Cached;

        private static void LoadResourceAssets()
        {
            s_Cached = new Texture2D[s_skin.Length];

            for (int i = 0; i < s_Cached.Length; ++i)
            {
                // Get image data (PNG) from base64 encoded strings.
                byte[] imageData = Convert.FromBase64String(s_skin[i]);

                // Gather image size from image data.
                int texWidth, texHeight;
                GetImageSize(imageData, out texWidth, out texHeight);

                // Generate texture asset.
                var tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false, true);
                tex.hideFlags = HideFlags.HideAndDontSave;
                tex.name = "(Generated) KiteTexture:" + i;
                tex.filterMode = FilterMode.Point;
#if UNITY_2017_1_OR_NEWER
                ImageConversion.LoadImage(tex, imageData, markNonReadable: true);
#else
                tex.LoadImage(imageData);
#endif

                s_Cached[i] = tex;
            }

            s_skin = null;
        }

        /// <summary>
        /// Read width and height if PNG file in pixels.
        /// </summary>
        /// <param name="imageData">PNG image data.</param>
        /// <param name="width">Width of image in pixels.</param>
        /// <param name="height">Height of image in pixels.</param>
        private static void GetImageSize(byte[] imageData, out int width, out int height)
        {
            width = ReadInt(imageData, 3 + 15);
            height = ReadInt(imageData, 3 + 15 + 2 + 2);
        }

        private static int ReadInt(byte[] imageData, int offset)
        {
            return (imageData[offset] << 8) | imageData[offset + 1];
        }

        #endregion
    }
}