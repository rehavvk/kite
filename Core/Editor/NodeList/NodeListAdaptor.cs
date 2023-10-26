using System;
using System.Linq;
using Rehawk.Kite.NodeList;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    public class NodeListAdaptor
    {
        private readonly Sequence sequence;

        private int selectedNodeIndex;

        public event EventHandler Changed;
        
        public NodeListAdaptor(Sequence sequence)
        {
            this.sequence = sequence;
        }

        public int Count
        {
            get { return sequence.Count; }
        }

        public bool CanDrag(int index)
        {
            return !EditorApplication.isPlaying;
        }

        public bool CanRemove(int index)
        {
            return !EditorApplication.isPlaying;
        }

        public void Add()
        {
            GenericMenuBrowser.ShowAsync(NodeListControl.lastMouseDownPosition, "Add Node", () =>
            {
                return GetTypeSelectionMenu(typeof(NodeBase), type =>
                {
                    if (NodeOperationProcessors.TryGet(type, out NodeOperationProcessor processor))
                    {
                        var nodeToAdd = (NodeBase) Activator.CreateInstance(type);
                        
                        Undo.RegisterCompleteObjectUndo(sequence, "Add Node");

                        processor.DoAdd(sequence, nodeToAdd);
                        
                        SequenceValidator.Validate(sequence);
            
                        EditorUtility.SetDirty(sequence);

                        Changed?.Invoke(this, EventArgs.Empty);
                    }
                });
            });
        }

        public void Insert(int index)
        {
            GenericMenuBrowser.ShowAsync(NodeListControl.lastMouseDownPosition, "Insert Node", () =>
            {
                return GetTypeSelectionMenu(typeof(NodeBase), type =>
                {
                    if (NodeOperationProcessors.TryGet(type, out NodeOperationProcessor processor))
                    {
                        var nodeToInsert = (NodeBase) Activator.CreateInstance(type);
                        
                        Undo.RegisterCompleteObjectUndo(sequence, "Insert Node");

                        processor.DoInsert(sequence, nodeToInsert, index);
                        
                        SequenceValidator.Validate(sequence);
            
                        EditorUtility.SetDirty(sequence);

                        Changed?.Invoke(this, EventArgs.Empty);
                    }
                });
            });
        }

        public void Duplicate(int index, bool withoutGroup)
        {
            NodeBase nodeToDuplicate = sequence[index];
            
            if (NodeOperationProcessors.TryGet(nodeToDuplicate.GetType(), out NodeOperationProcessor processor))
            {
                var duplicatedNode = (NodeBase) sequence[index].Clone();

                Undo.RegisterCompleteObjectUndo(sequence, "Duplicate Node");
                
                processor.DoInsertDuplicate(sequence, duplicatedNode, index + 1, withoutGroup);
                
                SequenceValidator.Validate(sequence);
            
                EditorUtility.SetDirty(sequence);

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Remove(int index)
        {
            NodeBase nodeToRemove = sequence[index];

            if (NodeOperationProcessors.TryGet(nodeToRemove.GetType(), out NodeOperationProcessor processor))
            {
                Undo.RegisterCompleteObjectUndo(sequence, "Remove Node");

                processor.DoRemove(sequence, nodeToRemove, index);
                
                SequenceValidator.Validate(sequence);
            
                EditorUtility.SetDirty(sequence);

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Move(int sourceIndex, int destinationIndex, bool withoutGroup)
        {
            NodeBase node = sequence[sourceIndex];

            if (NodeOperationProcessors.TryGet(node.GetType(), out NodeOperationProcessor processor))
            {
                Undo.RegisterCompleteObjectUndo(sequence, "Move Node");

                processor.DoMove(sequence, node, sourceIndex, destinationIndex, withoutGroup);
                
                SequenceValidator.Validate(sequence);
            
                EditorUtility.SetDirty(sequence);

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Clear()
        {
            Undo.RegisterCompleteObjectUndo(sequence, "Clear Nodes");

            sequence.Clear();
            
            SequenceValidator.Validate(sequence);
            
            EditorUtility.SetDirty(sequence);

            Changed?.Invoke(this, EventArgs.Empty);
        }

        #region Drawning
        
        public float GetItemHeight(int index)
        {
            return 34;
        }

        public int GetItemIndent(int index)
        {
            if (sequence[index] != null)
            {
                return sequence[index].IndentLevel;
            }

            return 0;
        }

        public int GetItemLeftMargin(int index)
        {
            if (sequence[index] != null)
            {
                return sequence[index].LeftMargin;
            }

            return 0;
        }

        public void DrawItem(Rect position, int index)
        {
            NodeBase node = sequence[index];

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.Repaint:

                    var titleContent = new GUIContent();

                    Texture2D icon = node.Icon;
                    if (icon == null)
                    {
                        icon = (Texture2D) IconUtils.GetTypeIcon(node.GetType(), false);
                    }
                    
                    if (icon != null)
                    {
                        titleContent.image = icon;
                    }
                    else
                    {
                        titleContent.text = node.Title;
                    }
                    
                    EditorGUIUtility.SetIconSize(new Vector2(20, 20));
                    Vector2 titleSize = Styles.NodeTitle.CalcSize(titleContent);

                    var titleRect = new Rect(position);
                    titleRect.width = titleSize.x;

                    var nodeTitleStyle = new GUIStyle(Styles.NodeTitle);

                    Color restoreColor = GUI.color;

                    if (node.Color.a > 0)
                    {
                        GUI.color = node.Color;

                        Color.RGBToHSV(node.Color, out float h, out float s, out float v);

                        if (v >= 0.5f)
                        {
                            nodeTitleStyle.normal.textColor = Color.black;
                        }
                        else
                        {
                            nodeTitleStyle.normal.textColor = Color.white;
                        }
                    }
                    else
                    {
                        GUI.color = Colors.NodeTitleFallbackBackground;
                    }

                    GUI.Box(titleRect, "", Styles.NodeTitleBackground);

                    GUI.color = restoreColor;

                    EditorGUI.LabelField(titleRect, titleContent, nodeTitleStyle);

                    string summary = sequence[index].Summary;
                    if (!string.IsNullOrEmpty(summary))
                    {
                        var summaryContent = new GUIContent(summary);

                        var summaryRect = new Rect(position);
                        summaryRect.x = titleRect.xMax;
                        summaryRect.width -= titleRect.width;

                        var summaryStyle = new GUIStyle(Styles.NodeSummary);
                        if (summary.Contains('\n') || summary.Contains('\t'))
                        {
                            summaryStyle.alignment = TextAnchor.UpperLeft;
                        }

                        EditorGUI.LabelField(summaryRect, summaryContent, summaryStyle);
                    }
                    
                    EditorGUIUtility.SetIconSize(Vector2.zero);

                    break;
            }
        }
        
        private static GenericMenu GetTypeSelectionMenu(Type baseType, Action<Type> callback, GenericMenu menu = null, string subCategory = null)
        {
            if (menu == null)
            {
                menu = new GenericMenu();
            }

            if (subCategory != null)
            {
                subCategory += "/";
            }

            void Selected(object selectedType)
            {
                callback((Type)selectedType);
            }

            var scriptInfos = ScriptInfos.GetScriptInfosOfType(baseType);

            foreach (var info in scriptInfos.Where(info => string.IsNullOrEmpty(info.category)))
            {
                menu.AddItem(new GUIContent(subCategory + info.name), false, info.type != null ? (GenericMenu.MenuFunction2)Selected : null, info.type);
            }

            foreach (var info in scriptInfos.Where(info => !string.IsNullOrEmpty(info.category)))
            {
                menu.AddItem(new GUIContent(subCategory + info.category + "/" + info.name), false, info.type != null ? (GenericMenu.MenuFunction2)Selected : null, info.type);
            }

            return menu;
        }

        #endregion
    }
}