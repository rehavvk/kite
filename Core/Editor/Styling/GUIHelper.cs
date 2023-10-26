using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    public static class GUIHelper
    {
        static GUIHelper()
        {
            var tyGUIClip = Type.GetType("UnityEngine.GUIClip,UnityEngine");
            if (tyGUIClip != null)
            {
                var piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (piVisibleRect != null)
                {
                    var getMethod = piVisibleRect.GetGetMethod(true) ?? piVisibleRect.GetGetMethod(false);
                    VisibleRect = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), getMethod);
                }
            }

            var miFocusTextInControl = typeof(EditorGUI).GetMethod("FocusTextInControl", BindingFlags.Static | BindingFlags.Public);
            if (miFocusTextInControl == null)
                miFocusTextInControl = typeof(GUI).GetMethod("FocusControl", BindingFlags.Static | BindingFlags.Public);

            FocusTextInControl = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), miFocusTextInControl);
        }

        public static readonly Func<Rect> VisibleRect;

        public static readonly Action<string> FocusTextInControl;

        private static GUIStyle s_TempStyle = new GUIStyle();

        private static GUIContent s_TempIconContent = new GUIContent();
        private static readonly int s_IconButtonHint = "_ReorderableIconButton_".GetHashCode();

        public static void Draw(Rect position, GUIStyle style) 
        {
            GUI.Box(position, string.Empty, style);
        }
    }
}