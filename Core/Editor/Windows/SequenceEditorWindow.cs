using System;
using Rehawk.Kite.NodeList;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Kite
{
    public class SequenceEditorWindow : EditorWindow, IHasCustomMenu
    {
        private Sequence sequence;

        private Vector2 scrollPosition;
        
        private NodeListControl listControl;
        private NodeListAdaptor listAdaptor;

        private NodeWrapper nodeWrapper;
        
        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            InspectedNodeEvents.NodeGotDirty += OnInspectedNodeGotDirty;
            
            listControl = new NodeListControl();
            listControl.ItemClicked += OnNodeClicked;
            
            RefreshList();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            InspectedNodeEvents.NodeGotDirty -= OnInspectedNodeGotDirty;
            
            if (nodeWrapper != null)
            {
                DestroyImmediate(nodeWrapper);
            }
        }

        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            InspectedNodeEvents.NodeGotDirty -= OnInspectedNodeGotDirty;
            
            if (nodeWrapper != null)
            {
                DestroyImmediate(nodeWrapper);
            }
        }

        private void OpenSequence(Sequence sequence)
        {
            this.sequence = sequence;
            OnSequenceChanged();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (sequence)
            {
                EditorGUILayout.LabelField(sequence.name, Styles.InspectorTitle);

                EditorGUILayout.Space();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                {
                    listControl.Draw(listAdaptor);
                    
                    GUILayout.Space(100);
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void RefreshList()
        {
            if (sequence)
            {
                listAdaptor = new NodeListAdaptor(sequence);
                listAdaptor.Changed += OnNodesChanged;
            }
            else
            {
                listAdaptor = null;
            }
            
            Repaint();
        }

        private void RefreshInspector(bool forceOpen)
        {
            if (sequence.TryGetNodeByGuid(InspectedNodeUid, out NodeBase node))
            {
                if (nodeWrapper && forceOpen)
                {
                    // Otherwise the property drawer of the node is not reset and some property drawers will not work.
                    DestroyImmediate(nodeWrapper);
                }
                
                if (nodeWrapper == null)
                {
                    nodeWrapper = CreateInstance<NodeWrapper>();
                }

                nodeWrapper.node = (NodeBase) node.Clone();
                InspectedNodeEvents.InvokeNodeChanged(this, node);
                
                if (forceOpen)
                {
                    Selection.activeObject = nodeWrapper;
                }
            }
        }

        private void OnSequenceChanged()
        {
            if (sequence)
            {
                SequenceValidator.Validate(sequence);
            }
            
            RefreshList();
        }

        private void OnUndoRedoPerformed()
        {
            RefreshList();
            RefreshInspector(false);
        }

        private void OnNodeClicked(object sender, ItemClickedEventArgs args)
        {
            InspectedNodeUid = sequence[args.ItemIndex].Guid;

            RefreshInspector(true);
        }

        private void OnNodesChanged(object sender, EventArgs args)
        {
            RefreshInspector(false);
        }

        private void OnInspectedNodeGotDirty(object sender, NodeBase node)
        {
            if (sequence.TryGetIndexByGuid(node.Guid, out int index))
            {
                Undo.RegisterCompleteObjectUndo(sequence, "Changed Node Properties");

                sequence[index] = node;
                SequenceValidator.Validate(sequence);
                
                EditorUtility.SetDirty(sequence);

                RefreshInspector(false);
            }
            
            Repaint();
        }
        
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Settings"), false, () =>
            {
                SettingsService.OpenProjectSettings("Project/Kite");
            });
            
            menu.AddItem(new GUIContent("Clear Sequence"), false, () =>
            {
                listAdaptor.Clear();
            });
        }
        
        private static string InspectedNodeUid
        {
            get { return EditorPrefs.GetString("INSPECTED_NODE_UID"); }
            set { EditorPrefs.SetString("INSPECTED_NODE_UID", value); }
        }

        [MenuItem("Tools/Kite/Sequence Editor")]
        public static SequenceEditorWindow Open()
        {
            bool isNewWindow = !HasOpenInstances<SequenceEditorWindow>();
            
            var window = GetWindow<SequenceEditorWindow>();

            if (isNewWindow)
            {
                window.titleContent = new GUIContent("Sequence Editor");
                window.position = new Rect(window.position.x, window.position.y, 1270, 720);
            }

            window.Show();

            return window;
        }

        public static void Open(Sequence sequence)
        {
            SequenceEditorWindow window = Open();
            window.OpenSequence(sequence);

            if (Selection.activeObject is NodeWrapper)
            {
                Selection.activeObject = null;
            }
        }
    }
}