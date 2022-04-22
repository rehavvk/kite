using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite.NodeList
{
    public class NodeListControl
    {
        #region Custom Styles

        /// <summary>
        /// Style for right-aligned label for element number prefix.
        /// </summary>
        private static GUIStyle s_RightAlignedLabelStyle;

        #endregion

        #region Utility

        private static readonly int s_ReorderableListControlHint = "_ReorderableListControl_".GetHashCode();

        private static int GetReorderableListControlID()
        {
            return GUIUtility.GetControlID(s_ReorderableListControlHint, FocusType.Passive);
        }

        #endregion

        private static float s_AnchorMouseOffset;

        private static int s_AnchorIndex = -1;

        private static int s_TargetIndex = -1;

        private static int s_AutoFocusControlID = 0;

        private static int s_AutoFocusIndex = -1;

        #region Events

        public event ItemClickedEventHandler ItemClicked;

        private void OnItemClicked(ItemClickedEventArgs args)
        {
            if (ItemClicked != null)
                ItemClicked(this, args);
        }

        #endregion

        #region Control State

        private int _controlID;

        private Rect _visibleRect;

        private float _indexLabelWidth;

        private bool _tracking;

        private int _newSizeInput;

        private void PrepareState(int controlID)
        {
            _controlID = controlID;
            _visibleRect = GUIHelper.VisibleRect();

            _tracking = IsTrackingControl(controlID);
        }

        private static int CountDigits(int number)
        {
            return Mathf.Max(2, Mathf.CeilToInt(Mathf.Log10((float)number)));
        }

        #endregion

        #region Event Handling

        private static int s_SimulateMouseDragControlID;

        private static bool s_TrackingCancelBlockContext;

        private static void BeginTrackingReorderDrag(int controlID, int itemIndex)
        {
            GUIUtility.hotControl = controlID;
            GUIUtility.keyboardControl = 0;
            s_AnchorIndex = itemIndex;
            s_TargetIndex = itemIndex;
            s_TrackingCancelBlockContext = false;
        }

        private static void StopTrackingReorderDrag()
        {
            GUIUtility.hotControl = 0;
            s_AnchorIndex = -1;
            s_TargetIndex = -1;
        }

        private static bool IsTrackingControl(int controlID)
        {
            return !s_TrackingCancelBlockContext && GUIUtility.hotControl == controlID;
        }

        private void AcceptReorderDrag(NodeListAdaptor adaptor)
        {
            try
            {
                s_TargetIndex = Mathf.Clamp(s_TargetIndex, 0, adaptor.Count + 1);
                if (s_TargetIndex != s_AnchorIndex && s_TargetIndex != s_AnchorIndex + 1)
                    MoveItem(adaptor, s_AnchorIndex, s_TargetIndex);
            }
            finally
            {
                StopTrackingReorderDrag();
            }
        }

        private static Rect s_DragItemPosition;

        private static Rect s_RemoveButtonPosition;

        private void DrawListItem(Rect position, NodeListAdaptor adaptor, int itemIndex)
        {
            Rect boxPosition = position;

            int indent = adaptor.GetItemIndent(itemIndex);
            int leftMargin = adaptor.GetItemLeftMargin(itemIndex);

            if (indent == 0 && leftMargin < 0)
            {
                leftMargin = 0;
            }
            
            boxPosition.x += (40 * indent) + leftMargin;
            boxPosition.width -= (40 * indent) + leftMargin;

            if (_indexLabelWidth != 0)
            {
                boxPosition.x += _indexLabelWidth;
                boxPosition.width -= _indexLabelWidth;
            }

            bool isRepainting = Event.current.type == EventType.Repaint;
            bool isVisible = (position.y < _visibleRect.yMax && position.yMax > _visibleRect.y);

            Rect indexPosition = position;
            Rect itemContentPosition = boxPosition;

            EditorGUI.BeginChangeCheck();

            if (isRepainting && isVisible)
            {
                s_RightAlignedLabelStyle.Draw(new Rect(indexPosition.x, indexPosition.y, _indexLabelWidth, indexPosition.height), itemIndex.ToString(), false, false, false, false);
                Styles.NodeBackground.Draw(boxPosition, GUIContent.none, false, false, false, false);
            }

            if (s_AutoFocusIndex == itemIndex)
                GUI.SetNextControlName("AutoFocus_" + _controlID + "_" + itemIndex);

            adaptor.DrawItem(itemContentPosition, itemIndex);

            if (EditorGUI.EndChangeCheck())
                NodeListGUI.IndexOfChangedItem = itemIndex;

            if (Event.current.GetTypeForControl(_controlID) == EventType.ContextClick && boxPosition.Contains(Event.current.mousePosition))
            {
                ShowContextMenu(itemIndex, adaptor);
                Event.current.Use();
            }
        }

        private void DrawFloatingListItem(NodeListAdaptor adaptor, float targetSlotPosition)
        {
            DrawListItem(s_DragItemPosition, adaptor, s_AnchorIndex);
        }

        private void DrawListContainerAndItems(Rect position, NodeListAdaptor adaptor)
        {
            EventType eventType = Event.current.GetTypeForControl(_controlID);
            Vector2 mousePosition = Event.current.mousePosition;

            int newTargetIndex = s_TargetIndex;

            float firstItemY = position.y + Styles.Container.padding.top;
            float dragItemMaxY = (position.yMax - Styles.Container.padding.bottom) - s_DragItemPosition.height + 1;

            bool isMouseDragEvent = eventType == EventType.MouseDrag;
            if (s_SimulateMouseDragControlID == _controlID && eventType == EventType.Repaint)
            {
                s_SimulateMouseDragControlID = 0;
                isMouseDragEvent = true;
            }

            if (isMouseDragEvent && _tracking)
            {
                if (mousePosition.y < firstItemY)
                    newTargetIndex = 0;
                else if (mousePosition.y >= position.yMax)
                    newTargetIndex = adaptor.Count;

                s_DragItemPosition.y = Mathf.Clamp(mousePosition.y + s_AnchorMouseOffset, firstItemY, dragItemMaxY);
            }

            switch (eventType)
            {
                case EventType.MouseDown:
                    
                    if (_tracking)
                    {
                        s_TrackingCancelBlockContext = true;
                        Event.current.Use();
                    }

                    break;

                case EventType.MouseUp:
                    if (_controlID == GUIUtility.hotControl)
                    {
                        if (!s_TrackingCancelBlockContext)
                            AcceptReorderDrag(adaptor);
                        else
                            StopTrackingReorderDrag();
                        Event.current.Use();
                    }

                    break;

                case EventType.KeyDown:
                    if (_tracking && Event.current.keyCode == KeyCode.Escape)
                    {
                        StopTrackingReorderDrag();
                        Event.current.Use();
                    }

                    break;

                case EventType.ExecuteCommand:
                    if (s_ContextControlID == _controlID)
                    {
                        int itemIndex = s_ContextItemIndex;
                        try
                        {
                            DoCommand(s_ContextCommandName, itemIndex, adaptor);
                            Event.current.Use();
                        }
                        finally
                        {
                            s_ContextControlID = 0;
                            s_ContextItemIndex = 0;
                        }
                    }

                    break;
            }

            NodeListGUI.IndexOfChangedItem = -1;

            Rect itemPosition = new Rect(position.x + Styles.Container.padding.left, firstItemY, position.width - Styles.Container.padding.horizontal, 0);
            float targetSlotPosition = dragItemMaxY;

            float lastMidPoint = 0f;
            float lastHeight = 0f;

            int count = adaptor.Count;
            for (int i = 0; i < count; ++i)
            {
                itemPosition.y = itemPosition.yMax;
                itemPosition.height = 0;

                if (i != 0)
                {
                    itemPosition.y += 4;
                }

                lastMidPoint = itemPosition.y - lastHeight / 2f;

                if (_tracking)
                {
                    if (i == s_TargetIndex)
                    {
                        targetSlotPosition = itemPosition.y;
                        itemPosition.y += s_DragItemPosition.height;
                    }

                    if (i == s_AnchorIndex)
                        continue;

                    itemPosition.height = adaptor.GetItemHeight(i) + 4;
                    lastHeight = itemPosition.height;
                }
                else
                {
                    itemPosition.height = adaptor.GetItemHeight(i) + 4;
                    lastHeight = itemPosition.height;
                }

                if (_tracking && isMouseDragEvent)
                {
                    float midpoint = itemPosition.y + itemPosition.height / 2f;

                    if (s_TargetIndex < i)
                    {
                        if (s_DragItemPosition.yMax > lastMidPoint && s_DragItemPosition.yMax < midpoint)
                            newTargetIndex = i;
                    }
                    else if (s_TargetIndex > i)
                    {
                        if (s_DragItemPosition.y > lastMidPoint && s_DragItemPosition.y < midpoint)
                            newTargetIndex = i;
                    }
                }

                DrawListItem(itemPosition, adaptor, i);

                if (adaptor.Count < count)
                {
                    count = adaptor.Count;
                    --i;
                    continue;
                }

                if (Event.current.type != EventType.Used)
                {
                    switch (eventType)
                    {
                        case EventType.MouseDown:
                            if (GUI.enabled && itemPosition.Contains(mousePosition))
                            {
                                // Remove input focus from control before attempting a context click or drag.
                                GUIUtility.keyboardControl = 0;

                                if (Event.current.button == 0)
                                {
                                    var args = new ItemClickedEventArgs(adaptor, i);
                                    OnItemClicked(args);

                                    if (adaptor.CanDrag(i))
                                    {
                                        s_DragItemPosition = itemPosition;

                                        BeginTrackingReorderDrag(_controlID, i);
                                        s_AnchorMouseOffset = itemPosition.y - mousePosition.y;
                                        s_TargetIndex = i;
                                    }

                                    Event.current.Use();
                                }
                            }

                            break;
                    }
                }
            }

            lastMidPoint = position.yMax - lastHeight / 2f;

            if (IsTrackingControl(_controlID))
            {
                if (isMouseDragEvent)
                {
                    if (s_DragItemPosition.yMax >= lastMidPoint)
                        newTargetIndex = count;

                    s_TargetIndex = newTargetIndex;

                    if (eventType == EventType.MouseDrag)
                        Event.current.Use();
                }

                DrawFloatingListItem(adaptor, targetSlotPosition);
            }

            GUIUtility.GetControlID(FocusType.Keyboard);

            if (isMouseDragEvent && IsTrackingControl(_controlID))
                AutoScrollTowardsMouse();
        }

        private static bool ContainsRect(Rect a, Rect b)
        {
            return a.Contains(new Vector2(b.xMin, b.yMin)) && a.Contains(new Vector2(b.xMax, b.yMax));
        }

        private void AutoScrollTowardsMouse()
        {
            const float triggerPaddingInPixels = 8f;
            const float maximumRangeInPixels = 4f;

            Rect visiblePosition = GUIHelper.VisibleRect();
            Vector2 mousePosition = Event.current.mousePosition;
            Rect mouseRect = new Rect(mousePosition.x - triggerPaddingInPixels, mousePosition.y - triggerPaddingInPixels, triggerPaddingInPixels * 2, triggerPaddingInPixels * 2);

            if (!ContainsRect(visiblePosition, mouseRect))
            {
                if (mousePosition.y < visiblePosition.center.y)
                    mousePosition = new Vector2(mouseRect.xMin, mouseRect.yMin);
                else
                    mousePosition = new Vector2(mouseRect.xMax, mouseRect.yMax);

                mousePosition.x = Mathf.Max(mousePosition.x - maximumRangeInPixels, mouseRect.xMax);
                mousePosition.y = Mathf.Min(mousePosition.y + maximumRangeInPixels, mouseRect.yMax);
                GUI.ScrollTo(new Rect(mousePosition.x, mousePosition.y, 1, 1));

                s_SimulateMouseDragControlID = _controlID;

                var focusedWindow = EditorWindow.focusedWindow;
                if (focusedWindow != null)
                    focusedWindow.Repaint();
            }
        }

        private void CheckForAutoFocusControl()
        {
            if (Event.current.type == EventType.Used)
                return;

            if (s_AutoFocusControlID == _controlID)
            {
                s_AutoFocusControlID = 0;
                GUIHelper.FocusTextInControl("AutoFocus_" + _controlID + "_" + s_AutoFocusIndex);
                s_AutoFocusIndex = -1;
            }
        }

        private static readonly Dictionary<int, float> s_ContainerHeightCache = new Dictionary<int, float>();
        
        private Rect GetListRectWithAutoLayout(NodeListAdaptor adaptor)
        {
            float totalHeight;

            // Calculate position of list field using layout engine.
            if (Event.current.type == EventType.Layout)
            {
                totalHeight = CalculateListHeight(adaptor);
                s_ContainerHeightCache[_controlID] = totalHeight;
            }
            else
            {
                totalHeight = s_ContainerHeightCache.ContainsKey(_controlID)
                    ? s_ContainerHeightCache[_controlID]
                    : 0;
            }

            return GUILayoutUtility.GetRect(GUIContent.none, Styles.Container, GUILayout.Height(totalHeight));
        }

        private void DrawLayoutListField(NodeListAdaptor adaptor)
        {
            Rect position = GetListRectWithAutoLayout(adaptor);

            // Make room for vertical spacing below footer buttons.
            position.height -= 10f;

            DrawListContainerAndItems(position, adaptor);

            CheckForAutoFocusControl();
        }
        
        private void FixStyles()
        {
            if (s_RightAlignedLabelStyle == null)
            {
                s_RightAlignedLabelStyle = new GUIStyle(GUI.skin.label);
                s_RightAlignedLabelStyle.alignment = TextAnchor.MiddleRight;
                s_RightAlignedLabelStyle.padding.right = 10;
                s_RightAlignedLabelStyle.fontSize = 10;
            }
        }

        private void Draw(int controlID, NodeListAdaptor adaptor)
        {
            FixStyles();
            PrepareState(controlID);

            _indexLabelWidth = CountDigits(adaptor.Count) * 8 + 8;

            if (Event.current.type == EventType.MouseDown)
            {
                LastMouseDownPosition = Event.current.mousePosition;
            }

            if (adaptor.Count > 0)
            {
                DrawLayoutListField(adaptor);
            }
            else
            {
                EditorGUILayout.LabelField("Sequence is empty...");

                if (!EditorApplication.isPlaying)
                {
                    EditorGUILayout.Space();
            
                    if (GUILayout.Button("Add Node"))
                    {
                        AddItem(adaptor);
                    }
                }
            }
        }

        public void Draw(NodeListAdaptor adaptor)
        {
            int controlID = GetReorderableListControlID();
            Draw(controlID, adaptor);
        }

        #endregion

        #region Context Menu

        private static readonly GUIContent CommandMoveToTop = new GUIContent("Move to Top");
        private static readonly GUIContent CommandMoveToBottom = new GUIContent("Move to Bottom");
        private static readonly GUIContent CommandInsert = new GUIContent("Insert");
        private static readonly GUIContent CommandInsertAbove = new GUIContent("Insert Above");
        private static readonly GUIContent CommandInsertBelow = new GUIContent("Insert Below");
        private static readonly GUIContent CommandDuplicate = new GUIContent("Duplicate");
        private static readonly GUIContent CommandRemove = new GUIContent("Remove");
        private static readonly GUIContent CommandClearAll = new GUIContent("Clear All");

        private static int s_ContextControlID;
        private static int s_ContextItemIndex;

        private static string s_ContextCommandName;

        public static Vector2 LastMouseDownPosition;

        private void ShowContextMenu(int itemIndex, NodeListAdaptor adaptor)
        {
            GenericMenu menu = new GenericMenu();

            s_ContextControlID = _controlID;
            s_ContextItemIndex = itemIndex;

            AddItemsToMenu(menu, itemIndex, adaptor);

            if (menu.GetItemCount() > 0)
                menu.ShowAsContext();
        }

        private static readonly GenericMenu.MenuFunction2 DefaultContextHandler = DefaultContextMenuHandler;

        private static void DefaultContextMenuHandler(object userData)
        {
            var commandContent = userData as GUIContent;
            if (commandContent == null || string.IsNullOrEmpty(commandContent.text))
                return;

            s_ContextCommandName = commandContent.text;

            var e = EditorGUIUtility.CommandEvent("ReorderableListContextCommand");
            EditorWindow.focusedWindow.SendEvent(e);
        }

        private void AddItemsToMenu(GenericMenu menu, int itemIndex, NodeListAdaptor adaptor)
        {
            if (!EditorApplication.isPlaying)
            {
                menu.AddItem(CommandInsert, false, DefaultContextHandler, CommandInsert);
                menu.AddItem(CommandDuplicate, false, DefaultContextHandler, CommandDuplicate);

                menu.AddSeparator("");

                menu.AddItem(CommandRemove, false, DefaultContextHandler, CommandRemove);

                menu.AddSeparator("");

                if (itemIndex > 0)
                {
                    menu.AddItem(CommandMoveToTop, false, DefaultContextHandler, CommandMoveToTop);
                }
                else
                {
                    menu.AddDisabledItem(CommandMoveToTop);
                }

                if (itemIndex + 1 >= adaptor.Count)
                {
                    menu.AddDisabledItem(CommandMoveToBottom);
                }
                else
                {
                    menu.AddItem(CommandMoveToBottom, false, DefaultContextHandler, CommandMoveToBottom);
                }

                menu.AddSeparator("");

                menu.AddItem(CommandClearAll, false, DefaultContextHandler, CommandClearAll);
            }
        }

        #endregion

        #region Command Handling

        private bool HandleCommand(string commandName, int itemIndex, NodeListAdaptor adaptor)
        {
            switch (commandName)
            {
                case "Move to Top":
                    MoveItem(adaptor, itemIndex, 0);
                    return true;
                case "Move to Bottom":
                    MoveItem(adaptor, itemIndex, adaptor.Count);
                    return true;

                case "Insert":
                    InsertItem(adaptor, itemIndex + 1);
                    return true;
                
                case "Duplicate":
                    DuplicateItem(adaptor, itemIndex);
                    return true;

                case "Remove":
                    RemoveItem(adaptor, itemIndex);
                    return true;
                case "Clear All":
                    ClearAll(adaptor);
                    return true;

                default:
                    return false;
            }
        }

        private bool DoCommand(string commandName, int itemIndex, NodeListAdaptor adaptor)
        {
            if (!HandleCommand(commandName, itemIndex, adaptor))
            {
                Debug.LogWarning("Unknown context command.");
                return false;
            }

            return true;
        }

        #endregion

        #region Methods

        private float CalculateListHeight(NodeListAdaptor adaptor)
        {
            FixStyles();

            float totalHeight = Styles.Container.padding.vertical - 1 + 10;

            int count = adaptor.Count;
            for (int i = 0; i < count; ++i)
            {
                totalHeight += adaptor.GetItemHeight(i);
            }

            totalHeight += 8 * count;

            return totalHeight;
        }

        private void MoveItem(NodeListAdaptor adaptor, int sourceIndex, int destIndex)
        {
            adaptor.Move(sourceIndex, destIndex);

            int newIndex = destIndex;
            if (newIndex > sourceIndex)
                --newIndex;

            GUI.changed = true;
            NodeListGUI.IndexOfChangedItem = -1;
        }

        private void AddItem(NodeListAdaptor adaptor)
        {
            adaptor.Add();
            
            GUI.changed = true;
            NodeListGUI.IndexOfChangedItem = -1;
        }

        private void InsertItem(NodeListAdaptor adaptor, int itemIndex)
        {
            adaptor.Insert(itemIndex);

            GUI.changed = true;
            NodeListGUI.IndexOfChangedItem = -1;
        }

        private void DuplicateItem(NodeListAdaptor adaptor, int itemIndex)
        {
            adaptor.Duplicate(itemIndex);

            GUI.changed = true;
            NodeListGUI.IndexOfChangedItem = -1;
        }

        private void RemoveItem(NodeListAdaptor adaptor, int itemIndex)
        {
            if (adaptor.CanRemove(itemIndex))
            {
                adaptor.Remove(itemIndex);

                GUI.changed = true;
                NodeListGUI.IndexOfChangedItem = -1;
            }
        }

        private void ClearAll(NodeListAdaptor adaptor)
        {
            if (adaptor.Count == 0)
                return;

            adaptor.Clear();

            GUI.changed = true;
            NodeListGUI.IndexOfChangedItem = -1;
        }

        #endregion
    }
}