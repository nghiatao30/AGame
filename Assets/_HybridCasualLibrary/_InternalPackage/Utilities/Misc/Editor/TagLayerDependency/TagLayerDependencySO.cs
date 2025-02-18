using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class TagLayerDependencySO : UnityEditor.SingletonSO<TagLayerDependencySO>
{
    [Serializable]
    public class UnityTag
    {
        [ShowInInspector, PropertyOrder(-1), CustomValueDrawer("CustomLabelDrawer")]
        private string m_Label;
        [SerializeField]
        private string m_Tag;

        public string tag => m_Tag;

        private void CustomLabelDrawer(string labelText, GUIContent label)
        {
            EditorGUILayout.LabelField($"Tag {IndexOf(this)}");
        }
    }

    [Serializable]
    public class UnityLayer
    {
        [ShowInInspector, PropertyOrder(-1), CustomValueDrawer("CustomLabelDrawer")]
        private string m_Label;
        [SerializeField, TableColumnWidth(1, Resizable = true)]
        private bool m_IsBuiltIn;
        [SerializeField, CustomValueDrawer("CustomLayerNameDrawer")]
        private string m_LayerName;

        public bool isBuiltIn => m_IsBuiltIn;
        public string layerName => m_LayerName;

        private void CustomLabelDrawer(string labelText, GUIContent label)
        {
            if (m_IsBuiltIn)
            {
                GUI.enabled = false;
                EditorGUILayout.LabelField($"BuiltIn Layer {IndexOf(this)}");
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.LabelField($"User Layer {IndexOf(this)}");
            }
        }

        private string CustomLayerNameDrawer(string layerName, GUIContent label)
        {
            if (isBuiltIn)
                GUI.enabled = false;
            var result = EditorGUILayout.TextField(layerName);
            GUI.enabled = true;
            return result;
        }
    }

    [SerializeField, TableList, ListDrawerSettings(ShowPaging = false, ShowIndexLabels = true)]
    private List<UnityTag> m_Tags = new List<UnityTag>();
    [SerializeField, TableList, ListDrawerSettings(ShowPaging = false, HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ShowIndexLabels = true)]
    private List<UnityLayer> m_Layers = new List<UnityLayer>(32);

    public List<UnityTag> tags => m_Tags;
    public List<UnityLayer> layers => m_Layers;

    private static int IndexOf(UnityTag tag)
    {
        return Instance.m_Tags.IndexOf(tag);
    }

    private static int IndexOf(UnityLayer layer)
    {
        return Instance.m_Layers.IndexOf(layer);
    }
}