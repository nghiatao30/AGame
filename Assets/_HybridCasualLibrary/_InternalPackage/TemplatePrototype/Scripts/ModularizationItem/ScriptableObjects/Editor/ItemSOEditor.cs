using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using HyrphusQ.Helpers;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

[CustomEditor(typeof(ItemSO), true), CanEditMultipleObjects]
public class ItemSOEditor : OdinEditor
{
    private static class Styles
    {
        public static GUIStyle s_HeaderLabelStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };
        public static GUIStyle s_ItemModuleLabelStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel)
        {
            alignment = TextAnchor.MiddleCenter,
        };
    }
    private const float k_LabelValueRatio = 0.25f;

    private ItemSO itemSO;
    private Sprite itemThumbnail;
    private GenericMenu menu;
    private SerializedProperty listItemModulesSerializedProp;
    private ReorderableList reorderableListItemModules;
    private Type[] itemModuleTypes;
    private List<ItemModule> listItemModules;

    protected override void OnEnable()
    {
        base.OnEnable();
        itemSO = target as ItemSO;
        itemThumbnail = itemSO.TryGetModule(out ImageItemModule imageModule) ? imageModule.thumbnailImage : null;
        listItemModules = serializedObject.targetObject.GetFieldValue<List<ItemModule>>("m_ItemModules");
        listItemModulesSerializedProp = serializedObject.FindProperty("m_ItemModules");
        reorderableListItemModules = new ReorderableList(listItemModules, typeof(ItemModule), true, true, false, true);
        reorderableListItemModules.drawHeaderCallback += OnDrawHeader;
        reorderableListItemModules.drawElementCallback += OnDrawElement;
        reorderableListItemModules.onRemoveCallback += OnRemoveItem;
        reorderableListItemModules.elementHeightCallback += OnGetElementHeight;

        // Replace C# Reflection with TypeCache for Editor performance wise
        itemModuleTypes = TypeCache.GetTypesDerivedFrom<ItemModule>().Where(type => !type.IsAbstract).ToArray();
    }

    protected virtual void DrawFirstProperties()
    {
        if (itemThumbnail != null)
        {
            GUI.enabled = false;
            SirenixEditorFields.UnityPreviewObjectField(itemThumbnail, typeof(Sprite), false, 100f, ObjectFieldAlignment.Left);
            GUI.enabled = true;
        }
    }

    protected virtual void DrawLastProperties()
    {

    }

    private void OnDrawHeader(Rect rect)
    {
        Rect leftRect = new Rect(rect.position, Vector2.Scale(rect.size, new Vector2(k_LabelValueRatio, 1f)));
        Rect rightRect = new Rect(rect.position + leftRect.width * Vector2.right, Vector2.Scale(rect.size, new Vector2(1f - k_LabelValueRatio, 1f)));

        // Layout header label
        EditorGUI.LabelField(leftRect, "Module Name", Styles.s_HeaderLabelStyle);
        EditorGUI.LabelField(rightRect, "Module Value", Styles.s_HeaderLabelStyle);
    }

    private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index >= listItemModulesSerializedProp.arraySize)
            return;

        ItemModule itemModule = listItemModules[index];
        SerializedProperty elementSerializedProp = listItemModulesSerializedProp.GetArrayElementAtIndex(index).Copy();

        Rect labelRect = new Rect(rect.position, Vector2.Scale(rect.size, new Vector2(k_LabelValueRatio, 1f)));
        Rect valueRect = new Rect(rect.position + Vector2.right * (labelRect.width + 0.025f * rect.width), Vector2.Scale(rect.size, new Vector2(1f - (k_LabelValueRatio + 0.025f), 1f)));

        // Layout module label & value
        EditorGUI.LabelField(labelRect, GetItemModuleInspectorName(itemModule), Styles.s_ItemModuleLabelStyle);
        EditorGUI.PropertyField(valueRect, elementSerializedProp, elementSerializedProp.isExpanded);
    }

    private float OnGetElementHeight(int index)
    {
        if (index >= listItemModulesSerializedProp.arraySize)
            return 0f;

        SerializedProperty elementSerializedProp = listItemModulesSerializedProp.GetArrayElementAtIndex(index).Copy();
        return EditorGUI.GetPropertyHeight(elementSerializedProp, elementSerializedProp.isExpanded);
    }

    private void OnRemoveItem(ReorderableList list)
    {
        listItemModules.RemoveAt(list.index);

        // Set dirty
        EditorUtility.SetDirty(target);
        // Save data
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawProperty(ref SerializedProperty serializedProp, bool includeChildren = true, Predicate<SerializedProperty> predicate = null)
    {
        if (string.IsNullOrEmpty(serializedProp.propertyPath) || (!predicate?.Invoke(serializedProp) ?? false))
            return;
        if (serializedProp.propertyType == SerializedPropertyType.Generic)
        {
            // Draw array property
            EditorGUILayout.PropertyField(serializedProp, includeChildren);
            serializedProp = serializedProp.GetEndProperty(includeChildren);
            DrawProperty(ref serializedProp, includeChildren, predicate);
        }
        else
        {
            // Draw normal property
            EditorGUILayout.PropertyField(serializedProp, includeChildren);
        }
    }

    private string GetItemModuleInspectorName(ItemModule itemModule)
    {
        var customInspectorName = itemModule.GetType().GetCustomAttributes(typeof(CustomInspectorName), true);
        if (customInspectorName.Length <= 0)
            return itemModule.GetType().Name;
        return (customInspectorName[0] as CustomInspectorName).displayName;
    }

    private void DrawLayoutListModules()
    {
        reorderableListItemModules.DoLayoutList();
    }

    private string CreateMenuGUIContentOfType(Type itemModuleType)
    {
        Type baseModuleType = itemModuleType.BaseType;
        bool isBaseTypeOfAnyType = Array.Exists(itemModuleTypes, item => item.BaseType == itemModuleType);
        string menuGUIContent = isBaseTypeOfAnyType ? $"{itemModuleType.Name}/{itemModuleType.Name}" : itemModuleType.Name;
        while (baseModuleType != typeof(ItemModule))
        {
            menuGUIContent = $"{baseModuleType.Name}/" + menuGUIContent;
            baseModuleType = baseModuleType.BaseType;
        }
        return menuGUIContent;
    }

    private void CreateMenuEventCodeOptions(Action<object> onSelectCallback, bool menuForAddModule = true)
    {
        menu = new GenericMenu();
        foreach (var itemModuleType in itemModuleTypes)
        {
            if (menuForAddModule && listItemModules.Exists(item => item.GetType().Equals(itemModuleType)))
                continue;
            menu.AddItem(new GUIContent(CreateMenuGUIContentOfType(itemModuleType)), false, onSelectCallback.Invoke, itemModuleType);
        }
        menu.ShowAsContext();
    }

    private void OnAddEvent(object itemModuleType)
    {
        foreach (var obj in Selection.objects)
        {
            var itemSO = obj as ItemSO;
            var itemModuleInstance = Activator.CreateInstance(itemModuleType as Type);
            itemModuleInstance.SetFieldValue("m_ItemSO", itemSO);
            itemSO.itemModules.Add(itemModuleInstance as ItemModule);
            itemSO.InvokeMethod("OnValidate");
            // Set dirty
            EditorUtility.SetDirty(obj);
        }
    }

    private void OnRemoveEvent(object itemModuleType)
    {
        var moduleType = itemModuleType as Type;
        foreach (var obj in Selection.objects)
        {
            var itemSO = obj as ItemSO;
            var moduleInstance = itemSO.itemModules.FirstOrDefault(itemModule => itemModule.GetType().Equals(moduleType));
            if (moduleInstance == null)
                continue;
            itemSO.itemModules.Remove(moduleInstance);
            itemSO.InvokeMethod("OnValidate");
            // Set dirty
            EditorUtility.SetDirty(obj);
        }
    }

    private void DrawDuplicateModuleNotice()
    {
        var hashSet = new HashSet<Type>();
        foreach (var item in listItemModules)
        {
            var baseModuleType = GetBaseModuleType(item.GetType());
            var hasDuplicateModule = hashSet.Contains(baseModuleType);
            if (hasDuplicateModule)
            {
                EditorGUILayout.HelpBox($"A duplicate module of the same type '{baseModuleType}' has been detected. Exercise caution!", MessageType.Warning);
                break;
            }
            hashSet.Add(baseModuleType);
        }

        Type GetBaseModuleType(Type itemModuleType)
        {
            var previousType = itemModuleType;
            var baseType = itemModuleType.BaseType;
            do
            {
                if (baseType == typeof(ItemModule))
                {
                    return previousType;
                }
                previousType = baseType;
                baseType = baseType.BaseType;
            }
            while (true);
        }
    }

    public override void OnInspectorGUI()
    {
        // Update data
        serializedObject.UpdateIfRequiredOrScript();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        GUI.enabled = true;

        // Draw properties at first after m_Script
        DrawFirstProperties();

        // Draw default inspector with Odin
        DrawTree();

        // Draw default inspector
        //DrawPropertiesExcluding(serializedObject, "m_ItemModules", "m_Script");
        //var serializedProp = serializedObject.GetIterator().Copy();
        //if (serializedProp.NextVisible(true))
        //{
        //    while (serializedProp.NextVisible(true))
        //    {
        //        DrawProperty(ref serializedProp, predicate: item =>
        //        {
        //            if (serializedProp.propertyPath == listItemModulesSerializedProp.propertyPath)
        //            {
        //                serializedProp = listItemModulesSerializedProp.GetEndProperty(true);
        //                DrawProperty(ref serializedProp);
        //                return false;
        //            }
        //            return true;
        //        });
        //        if (string.IsNullOrEmpty(serializedProp.propertyPath))
        //            break;
        //    };
        //}

        EditorGUI.BeginChangeCheck();
        DrawLayoutListModules();
        if (EditorGUI.EndChangeCheck())
        {
            // Set dirty
            EditorUtility.SetDirty(target);
        }

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.TrTextContent("Add Module")))
                CreateMenuEventCodeOptions(OnAddEvent);
            if (GUILayout.Button(EditorGUIUtility.TrTextContent("Remove Module")))
                CreateMenuEventCodeOptions(OnRemoveEvent, false);
            GUILayout.Space(10f);
        }

        // Draw properties at last
        DrawLastProperties();

        // Draw duplicate module notice
        DrawDuplicateModuleNotice();

        // Save data
        serializedObject.ApplyModifiedProperties();
    }
}