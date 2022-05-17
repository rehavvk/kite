using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite.NodeList
{
    public class NodeListControl
    {
        #region Utility

        private static readonly int reorderableListControlHint = "_ReorderableListControl_".GetHashCode();

        private static int GetReorderableListControlID()
        {
            return GUIUtility.GetControlID(reorderableListControlHint, FocusType.Passive);
        }

        #endregion

        private static float anchorMouseOffset;

        private static int anchorIndex = -1;

        private static int targetIndex = -1;

        private static int autoFocusControlID = 0;

        private static int autoFocusIndex = -1;

        #region Events

        public event ItemClickedEventHandler ItemClicked;

        private void OnItemClicked(ItemClickedEventArgs args)
        {
            if (ItemClicked != null)
                ItemClicked(this, args);
        }

        #endregion

        #region Control State

        private int controlID;

        private Rect visibleRect;

        private float indexLabelWidth;

        private bool tracking;

        private int newSizeInput;

        private void PrepareState(int controlID)
        {
            this.controlID = controlID;
            visibleRect = GUIHelper.VisibleRect();

            tracking = IsTrackingControl(controlID);
        }

        private static int CountDigits(int number)
        {
            return Mathf.Max(2, Mathf.CeilToInt(Mathf.Log10((float)number)));
        }

        #endregion

        #region Event Handling

        private static int simulateMouseDragControlID;

        private static bool trackingCancelBlockContext;

        private static void BeginTrackingReorderDrag(int controlID, int itemIndex)
        {
            GUIUtility.hotControl = controlID;
            GUIUtility.keyboardControl = 0;
            anchorIndex = itemIndex;
            targetIndex = itemIndex;
            trackingCancelBlockContext = false;
        }

        private static void StopTrackingReorderDrag()
        {
            GUIUtility.hotControl = 0;
            anchorIndex = -1;
            targetIndex = -1;
        }

        private static bool IsTrackingControl(int controlID)
        {
            return !trackingCancelBlockContext && GUIUtility.hotControl == controlID;
        }

        private void AcceptReorderDrag(NodeListAdaptor adaptor)
        {
            try
            {
                targetIndex = Mathf.Clamp(targetIndex, 0, adaptor.Count + 1);
                if (targetIndex != anchorIndex && targetIndex != anchorIndex + 1)
                {
                    MoveItem(adaptor, anchorIndex, targetIndex);
                }
            }
            finally
            {
                StopTrackingReorderDrag();
            }
        }

        private static Rect dragItemPosition;

        private static Rect removeButtonPosition;

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

            if (indexLabelWidth != 0)
            {
                boxPosition.x += indexLabelWidth;
                boxPosition.width -= indexLabelWidth;
            }

            bool isRepainting = Event.current.type == EventType.Repaint;
            bool isVisible = (position.y < visibleRect.yMax && position.yMax > visibleRect.y);

            Rect indexPosition = position;
            Rect itemContentPosition = boxPosition;

            EditorGUI.BeginChangeCheck();

            if (isRepainting && isVisible)
            {
                Styles.RightLabel.Draw(new Rect(indexPosition.x, indexPosition.y, indexLabelWidth, indexPosition.height), itemIndex.ToString(), false, false, false, false);
                Styles.NodeBackground.Draw(boxPosition, GUIContent.none, false, false, false, false);
            }

            if (autoFocusIndex == itemIndex)
                GUI.SetNextControlName("AutoFocus_" + controlID + "_" + itemIndex);

            adaptor.DrawItem(itemContentPosition, itemIndex);

            if (EditorGUI.EndChangeCheck())
                NodeListGUI.IndexOfChangedItem = itemIndex;

            if (Event.current.GetTypeForControl(controlID) == EventType.ContextClick && boxPosition.Contains(Event.current.mousePosition))
            {
                ShowContextMenu(itemIndex, adaptor);
                Event.current.Use();
            }
        }

        private void DrawFloatingListItem(NodeListAdaptor adaptor, float targetSlotPosition)
        {
            DrawListItem(dragItemPosition, adaptor, anchorIndex);
        }

        private void DrawListContainerAndItems(Rect position, NodeListAdaptor adaptor)
        {
            EventType eventType = Event.current.GetTypeForControl(controlID);
            Vector2 mousePosition = Event.current.mousePosition;

            int newTargetIndex = targetIndex;

            float firstItemY = position.y + Styles.Container.padding.top;
            float dragItemMaxY = (position.yMax - Styles.Container.padding.bottom) - dragItemPosition.height + 1;

            bool isMouseDragEvent = eventType == EventType.MouseDrag;
            if (simulateMouseDragControlID == controlID && eventType == EventType.Repaint)
            {
                simulateMouseDragControlID = 0;
                isMouseDragEvent = true;
            }

            if (isMouseDragEvent && tracking)
            {
                if (mousePosition.y < firstItemY)
                    newTargetIndex = 0;
                else if (mousePosition.y >= position.yMax)
                    newTargetIndex = adaptor.Count;

                dragItemPosition.y = Mathf.Clamp(mousePosition.y + anchorMouseOffset, firstItemY, dragItemMaxY);
            }

            switch (eventType)
            {
                case EventType.MouseDown:
                    
                    if (tracking)
                    {
                        trackingCancelBlockContext = true;
                        Event.current.Use();
                    }

                    break;

                case EventType.MouseUp:
                    if (controlID == GUIUtility.hotControl)
                    {
                        if (!trackingCancelBlockContext)
                            AcceptReorderDrag(adaptor);
                        else
                            StopTrackingReorderDrag();
                        Event.current.Use();
                    }

                    break;

                case EventType.KeyDown:
                    if (tracking && Event.current.keyCode == KeyCode.Escape)
                    {
                        StopTrackingReorderDrag();
                        Event.current.Use();
                    }

                    break;

                case EventType.ExecuteCommand:
                    if (contextControlID == controlID)
                    {
                        int itemIndex = contextItemIndex;
                        try
                        {
                            DoCommand(contextCommandName, itemIndex, adaptor);
                            Event.current.Use();
                        }
                        finally
                        {
                            contextControlID = 0;
                            contextItemIndex = 0;
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

                if (tracking)
                {
                    if (i == targetIndex)
                    {
                        targetSlotPosition = itemPosition.y;
                        itemPosition.y += dragItemPosition.height;
                    }

                    if (i == anchorIndex)
                        continue;

                    itemPosition.height = adaptor.GetItemHeight(i) + 4;
                    lastHeight = itemPosition.height;
                }
                else
                {
                    itemPosition.height = adaptor.GetItemHeight(i) + 4;
                    lastHeight = itemPosition.height;
                }

                if (tracking && isMouseDragEvent)
                {
                    float midpoint = itemPosition.y + itemPosition.height / 2f;

                    if (targetIndex < i)
                    {
                        if (dragItemPosition.yMax > lastMidPoint && dragItemPosition.yMax < midpoint)
                            newTargetIndex = i;
                    }
                    else if (targetIndex > i)
                    {
                        if (dragItemPosition.y > lastMidPoint && dragItemPosition.y < midpoint)
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
                                        dragItemPosition = itemPosition;

                                        BeginTrackingReorderDrag(controlID, i);
                                        anchorMouseOffset = itemPosition.y - mousePosition.y;
                                        targetIndex = i;
                                    }

                                    Event.current.Use();
                                }
                            }

                            break;
                    }
                }
            }

            lastMidPoint = position.yMax - lastHeight / 2f;

            if (IsTrackingControl(controlID))
            {
                if (isMouseDragEvent)
                {
                    if (dragItemPosition.yMax >= lastMidPoint)
                        newTargetIndex = count;

                    targetIndex = newTargetIndex;

                    if (eventType == EventType.MouseDrag)
                        Event.current.Use();
                }

                DrawFloatingListItem(adaptor, targetSlotPosition);
            }

            GUIUtility.GetControlID(FocusType.Keyboard);

            if (isMouseDragEvent && IsTrackingControl(controlID))
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

                simulateMouseDragControlID = controlID;

                var focusedWindow = EditorWindow.focusedWindow;
                if (focusedWindow != null)
                    focusedWindow.Repaint();
            }
        }

        private void CheckForAutoFocusControl()
        {
            if (Event.current.type == EventType.Used)
                return;

            if (autoFocusControlID == controlID)
            {
                autoFocusControlID = 0;
                GUIHelper.FocusTextInControl("AutoFocus_" + controlID + "_" + autoFocusIndex);
                autoFocusIndex = -1;
            }
        }

        private static readonly Dictionary<int, float> containerHeightCache = new Dictionary<int, float>();
        
        private Rect GetListRectWithAutoLayout(NodeListAdaptor adaptor)
        {
            float totalHeight;

            // Calculate position of list field using layout engine.
            if (Event.current.type == EventType.Layout)
            {
                totalHeight = CalculateListHeight(adaptor);
                containerHeightCache[controlID] = totalHeight;
            }
            else
            {
                totalHeight = containerHeightCache.ContainsKey(controlID) ? containerHeightCache[controlID] : 0;
            }

            return GUILayoutUtility.GetRect(GUIContent.none, Styles.Container, GUILayout.Height(totalHeight));
        }

        private void DrawLayoutListField(NodeListAdaptor adaptor)
        {
            Rect position = GetListRectWithAutoLayout(adaptor);

            DrawListContainerAndItems(position, adaptor);

            CheckForAutoFocusControl();
        }
        
        private void Draw(int controlID, NodeListAdaptor adaptor)
        {
            PrepareState(controlID);

            indexLabelWidth = CountDigits(adaptor.Count) * 8 + 8;

            if (Event.current.type == EventType.MouseDown)
            {
                lastMouseDownPosition = Event.current.mousePosition;
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

        private static int contextControlID;
        private static int contextItemIndex;

        private static string contextCommandName;

        public static Vector2 lastMouseDownPosition;

        private void ShowContextMenu(int itemIndex, NodeListAdaptor adaptor)
        {
            GenericMenu menu = new GenericMenu();

            contextControlID = controlID;
            contextItemIndex = itemIndex;

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

            contextCommandName = commandContent.text;

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

                // menu.AddSeparator("");
                //
                // menu.AddItem(CommandClearAll, false, DefaultContextHandler, CommandClearAll);
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
            adaptor.Move(sourceIndex, destIndex, Event.current.modifiers == EventModifiers.Alt);

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