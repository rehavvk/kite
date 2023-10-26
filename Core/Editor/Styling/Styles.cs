using UnityEngine;

namespace Rehawk.Kite
{
    public static class Styles
    {
        private static GUIStyle topCenterLabel;
        private static GUIStyle leftLabel;
        private static GUIStyle rightLabel;
        private static GUIStyle toolbarSearchTextField;
        private static GUIStyle toolbarSearchCancelButton;
        private static GUIStyle shadowedBackground;

        static Styles()
        {
            Title = new GUIStyle();
            Title.border = new RectOffset(2, 2, 2, 1);
            Title.margin = new RectOffset(5, 5, 5, 0);
            Title.padding = new RectOffset(5, 5, 3, 3);
            Title.alignment = TextAnchor.MiddleLeft;
            Title.normal.background = TextureResources.TitleBackground;
            Title.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

            Container = new GUIStyle();
            Container.border = new RectOffset(2, 2, 2, 2);
            Container.margin = new RectOffset(5, 5, 5, 5);
            Container.normal.background = TextureResources.ContainerBackground;

            NodeBackground = new GUIStyle();
            NodeBackground.normal.background = TextureResources.NodeBackgroundTexture;
            NodeBackground.normal.textColor = Color.white;
            NodeBackground.fontSize = 12;
            NodeBackground.wordWrap = false;

            NodeTitleBackground = new GUIStyle();
            NodeTitleBackground.normal.background = TextureResources.WhiteBackground;

            NodeTitle = new GUIStyle();
            NodeTitle.normal.textColor = Color.white;
            NodeTitle.padding = new RectOffset(10, 10, 4, 4);
            NodeTitle.fontStyle = FontStyle.Bold;
            NodeTitle.alignment = TextAnchor.MiddleLeft;
            NodeTitle.fontSize = 12;

            NodeSummary = new GUIStyle();
            NodeSummary.normal.textColor = Color.white;
            NodeSummary.padding = new RectOffset(10, 10, 4, 4);
            NodeSummary.alignment = TextAnchor.MiddleLeft;
            NodeSummary.fontSize = 12;
            NodeSummary.clipping = TextClipping.Clip;

            InspectorTitle = new GUIStyle();
            InspectorTitle.normal.background = TextureResources.InspectorTitleBackgroundTexture;
            InspectorTitle.normal.textColor = Color.white;
            InspectorTitle.padding = new RectOffset(10, 10, 4, 4);
            InspectorTitle.fontSize = 14;
            InspectorTitle.fontStyle = FontStyle.Bold;
            InspectorTitle.alignment = TextAnchor.MiddleCenter;

            NodeInspectorUid = new GUIStyle();
            NodeInspectorUid.normal.background = TextureResources.InspectorUidBackgroundTexture;
            NodeInspectorUid.normal.textColor = Color.grey;
            NodeInspectorUid.padding = new RectOffset(10, 10, 2, 2);
            NodeInspectorUid.fontSize = 10;
            NodeInspectorUid.alignment = TextAnchor.MiddleCenter;
        }
        
        public static GUIStyle Title { get; }

        public static GUIStyle Container { get; }

        public static GUIStyle NodeBackground { get; }

        public static GUIStyle NodeTitleBackground { get; }

        public static GUIStyle NodeTitle { get; }

        public static GUIStyle NodeSummary { get; }

        public static GUIStyle InspectorTitle { get; }

        public static GUIStyle NodeInspectorUid { get; }

        public static GUIStyle TopCenterLabel
        {
            get
            {
                if (topCenterLabel == null)
                {
                    topCenterLabel = new GUIStyle(GUI.skin.label);
                    topCenterLabel.richText = true;
                    topCenterLabel.fontSize = 11;
                    topCenterLabel.alignment = TextAnchor.UpperCenter;
                }

                return topCenterLabel;
            }
        }

        public static GUIStyle LeftLabel
        {
            get
            {
                if (leftLabel == null)
                {
                    leftLabel = new GUIStyle(GUI.skin.label);
                    leftLabel.richText = true;
                    leftLabel.fontSize = 10;
                    leftLabel.alignment = TextAnchor.MiddleLeft;
                    leftLabel.padding.left = 10;
                }

                return leftLabel;
            }
        }

        public static GUIStyle RightLabel
        {
            get
            {
                if (rightLabel == null)
                {
                    rightLabel = new GUIStyle(GUI.skin.label);
                    rightLabel.richText = true;
                    rightLabel.fontSize = 10;
                    rightLabel.alignment = TextAnchor.MiddleRight;
                    rightLabel.padding.right = 10;
                }

                return rightLabel;
            }
        }

        public static GUIStyle ToolbarSearchTextField
        {
            get
            {
                if (toolbarSearchTextField == null)
                {
                    toolbarSearchTextField = new GUIStyle("ToolbarSeachTextField");
                }

                return toolbarSearchTextField;
            }
        }

        public static GUIStyle ToolbarSearchCancelButton
        {
            get
            {
                if (toolbarSearchCancelButton == null)
                {
                    toolbarSearchCancelButton = new GUIStyle("ToolbarSeachCancelButton");
                }

                return toolbarSearchCancelButton;
            }
        }

        public static GUIStyle ShadowedBackground
        {
            get
            {
                if (shadowedBackground == null)
                {
                    shadowedBackground = new GUIStyle("CurveEditorBackground");
                }

                return shadowedBackground;
            }
        }
    }
}