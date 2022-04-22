using System;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    [CustomEditor(typeof(NodeWrapper), true)]
    public class NodeWrapperEditor : Editor
    {
        private Vector2 scrollPosition;

        protected override void OnHeaderGUI()
        {
            var wrapper = (NodeWrapper)target;
            NodeBase node = wrapper.node;

            if (node != null)
            {
                GUILayout.Box(GetNodeTypeName(node), Styles.InspectorTitle);
                if (GUILayout.Button(node.Uid, Styles.NodeInspectorUid))
                {
                    GUIUtility.systemCopyBuffer = node.Uid;
                    Debug.Log($"Copied node uid '{node.Uid}' to clipboard.");
                }

                string description = GetNodeTypeDescription(node);
                if (!string.IsNullOrEmpty(description))
                {
                    EditorGUILayout.HelpBox(description, MessageType.Info, true);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var wrapper = (NodeWrapper)target;
            NodeBase node = wrapper.node;

            if (node != null)
            {
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                    {
                        SerializedProperty iterator = serializedObject.GetIterator();

                        while (iterator.NextVisible(true))
                        {
                            if (iterator.name == "node")
                            {
                                iterator.isExpanded = true;
                            
                                EditorGUILayout.PropertyField(iterator, GUIContent.none, true);

                                break;
                            }
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    NodeGotDirty?.Invoke(this, node);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public static event EventHandler<NodeBase> NodeGotDirty;

        private static string GetNodeTypeName(NodeBase node)
        {
            var nameAttribute = node.GetType().GetAttribute<NameAttribute>(true);
            if (nameAttribute != null)
            {
                return nameAttribute.name;
            }

            return node.GetType().Name.Replace("Node", string.Empty).SplitCamelCase();
        }

        private static string GetNodeTypeDescription(NodeBase node)
        {
            var descriptionAttribute = node.GetType().GetAttribute<DescriptionAttribute>(true);
            if (descriptionAttribute != null)
            {
                return descriptionAttribute.description;
            }

            return string.Empty;
        }
    }
}