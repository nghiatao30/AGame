using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HyrphusQ.Helpers;
using HyrphusQ.SerializedDataStructure;
using System.Reflection;
using System.Linq;

[CustomPropertyDrawer(typeof(SerializedDictionary<,>), true)]
public class SerializedDictionaryDrawer : PropertyDrawer
{
    private static readonly float k_DictionarySizeFieldWidth = 50f;
    private static readonly float k_RemoveButtonWidth = 35f;
    private static readonly float k_KeyValueOffset = EditorGUIUtility.singleLineHeight * 0.25f;
    private static readonly Vector2 k_KeyValueLabelOffset = new Vector2(5f, EditorGUIUtility.singleLineHeight * 0.5f);
    private static readonly BindingFlags k_BindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    static class Styles
    {
        public static GUIContent s_AddItemGUIContent = EditorGUIUtility.TrTextContentWithIcon("Add", "CreateAddNew");
        public static GUIContent s_KeyLabelGUIContent = EditorGUIUtility.TrTextContent("Key");
        public static GUIContent s_ValueLabelGUIContent = EditorGUIUtility.TrTextContent("Value");
        public static GUIContent s_RemoveItemGUIContent = EditorGUIUtility.TrTextContentWithIcon(string.Empty, "TreeEditor.Trash");
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty keysSerializedProp = property.FindPropertyRelative("m_Keys");
        SerializedProperty valuesSerializedProp = property.FindPropertyRelative("m_Values");
        SerializedProperty keyTempSerializedProp = property.FindPropertyRelative("m_KeyTemp");
        SerializedProperty valueTempSerializedProp = property.FindPropertyRelative("m_ValueTemp");

        Rect originalPosition = new Rect(position);
        Rect headerRect = new Rect(position);
        Rect contentRect = new Rect(position);
        Rect footerRect = new Rect(position);

        Rect propertyLabelRect = new Rect(
            contentRect.position, 
            new Vector2(contentRect.width - k_DictionarySizeFieldWidth, EditorGUIUtility.singleLineHeight));

        Rect dictionarySizeRect = new Rect(
            contentRect.position + Vector2.right * propertyLabelRect.width,
            new Vector2(k_DictionarySizeFieldWidth, EditorGUIUtility.singleLineHeight));

        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(propertyLabelRect, property.isExpanded, label);

        GUI.enabled = false;
        EditorGUI.IntField(dictionarySizeRect, GUIContent.none, keysSerializedProp.arraySize);
        GUI.enabled = true;

        if (property.isExpanded)
        {
            var keyType = GetKeyType(property);
            var valueType = GetValueType(property);
            if (!CheckSupportKeyType(keyType))
            {
                contentRect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                contentRect.height = EditorGUIUtility.singleLineHeight * 2f;
                EditorGUI.HelpBox(contentRect, "Key type is not support", MessageType.Error);
                EditorGUI.EndFoldoutHeaderGroup();
                return;
            }

            // Create header
            headerRect = new Rect(originalPosition.position + new Vector2(0f, EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
                new Vector2(originalPosition.width, EditorGUIUtility.singleLineHeight));

            if (keyTempSerializedProp.propertyType == SerializedPropertyType.ObjectReference && keyTempSerializedProp.objectReferenceValue == null)
            {
                EditorGUI.HelpBox(headerRect, "Value can not be null", MessageType.Error);
                headerRect.position += Vector2.up * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }
            else
            {
                // Draw add btn
                Rect addButtonRect = new Rect(headerRect.position + new Vector2(originalPosition.width * (1f - 0.25f), GetHeightOfProperty(keyTempSerializedProp, valueTempSerializedProp) + EditorGUIUtility.standardVerticalSpacing),
                    new Vector2(headerRect.width * 0.25f, EditorGUIUtility.singleLineHeight));

                if (GUI.Button(addButtonRect, Styles.s_AddItemGUIContent))
                    OnClickAddItem(property, keysSerializedProp, valuesSerializedProp);
            }

            Rect keyTempRect = new Rect(
                headerRect.position,
                new Vector2(headerRect.width * 0.5f - k_KeyValueOffset * 5f, EditorGUIUtility.singleLineHeight));

            Rect valueTempRect = new Rect(
                keyTempRect.position + Vector2.right * (keyTempRect.width + k_KeyValueOffset),
                keyTempRect.size);
#if UNITY_ADDRESSABLE_ASSETS
            // Dirty fix for AssetReference drawer broken property field "None LABEL"
            if (GetValueType(property).IsAssignableFrom(typeof(UnityEngine.AddressableAssets.AssetReference)))
                DrawKeyValuePropertyAssetReference(keyTempSerializedProp, valueTempSerializedProp, keyTempRect, valueTempRect);
            else
                DrawKeyValueProperty(keyTempSerializedProp, valueTempSerializedProp, keyTempRect, valueTempRect);
#else
            DrawKeyValueProperty(keyTempSerializedProp, valueTempSerializedProp, keyTempRect, valueTempRect);
#endif
            contentRect.position += Vector2.up * (GetHeightOfProperty(keyTempSerializedProp, valueTempSerializedProp) + EditorGUIUtility.singleLineHeight);

            Rect keyLabelRect = new Rect(
                contentRect.position + Vector2.up * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 3f),
                new Vector2(contentRect.width * 0.5f, EditorGUIUtility.singleLineHeight));

            Rect valueLabelRect = new Rect(
                keyLabelRect.position + Vector2.right * (contentRect.width * 0.5f),
                keyLabelRect.size);

            // Draw toolbar label
            GUIStyle toolbarStyle = new GUIStyle(EditorStyles.toolbar);
            toolbarStyle.fontSize = 12;
            toolbarStyle.fontStyle = FontStyle.Bold;
            toolbarStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUI.LabelField(keyLabelRect, Styles.s_KeyLabelGUIContent, toolbarStyle);
            EditorGUI.LabelField(valueLabelRect, Styles.s_ValueLabelGUIContent, toolbarStyle);
            Rect keyRect = new Rect(keyLabelRect.position, new Vector2((contentRect.width - k_RemoveButtonWidth - 2f * k_KeyValueLabelOffset.x) * 0.5f, 0f));
            Rect valueRect = new Rect(keyRect.position + Vector2.right * (keyRect.width + k_KeyValueLabelOffset.x), keyRect.size);
            Rect removeBtnRect = new Rect(valueRect.position + Vector2.right * (valueRect.width + k_KeyValueLabelOffset.x), new Vector2(k_RemoveButtonWidth, 0f));
            if(keysSerializedProp.isArray && keysSerializedProp.arraySize > 0)
            {
                for (int i = 0; i < keysSerializedProp.arraySize; i++)
                {
                    var keyProperty = keysSerializedProp.GetArrayElementAtIndex(i);
                    var valueProperty = valuesSerializedProp.GetArrayElementAtIndex(i);
                    var height = GetHeightOfProperty(keyProperty, valueProperty) + k_KeyValueOffset;
                    keyRect.position += Vector2.up * height;
                    keyRect.height = height;
                    valueRect.position += Vector2.up * height;
                    valueRect.height = height;
                    removeBtnRect.position += Vector2.up * height;
                    removeBtnRect.height = EditorGUIUtility.singleLineHeight;
#if UNITY_ADDRESSABLE_ASSETS
                    // Dirty fix for AssetReference drawer broken property field "None LABEL"
                    if (GetValueType(property).IsAssignableFrom(typeof(UnityEngine.AddressableAssets.AssetReference)))
                        DrawKeyValuePropertyAssetReference(keyProperty, valueProperty, keyRect, valueRect);
                    else
                        DrawKeyValueProperty(keyProperty, valueProperty, keyRect, valueRect);
#else
                    DrawKeyValueProperty(keyProperty, valueProperty, keyRect, valueRect);
#endif
                    if (GUI.Button(removeBtnRect, Styles.s_RemoveItemGUIContent))
                    {
                        if(keysSerializedProp.propertyType == SerializedPropertyType.ObjectReference && keysSerializedProp.objectReferenceValue != null)
                            keysSerializedProp.DeleteArrayElementAtIndex(i);
                        keysSerializedProp.DeleteArrayElementAtIndex(i);
                        if (valuesSerializedProp.propertyType == SerializedPropertyType.ObjectReference && valuesSerializedProp.objectReferenceValue != null)
                            valuesSerializedProp.DeleteArrayElementAtIndex(i);
                        valuesSerializedProp.DeleteArrayElementAtIndex(i);
                        property.serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    }
                }
            }
        }
        EditorGUI.EndFoldoutHeaderGroup();
    }
    private void DrawKeyValuePropertyAssetReference(SerializedProperty key, SerializedProperty value, Rect keyRect, Rect valueRect)
    {
        EditorGUI.PropertyField(keyRect, key, GUIContent.none, true);
        // Dirty fix for asset reference broken property field "LABEL.None"
        EditorGUIUtility.labelWidth = 0.1f;
        EditorGUI.PropertyField(valueRect, value, GUIContent.none, true);
        EditorGUIUtility.labelWidth = 0f;
    }
    private void DrawKeyValueProperty(SerializedProperty key, SerializedProperty value, Rect keyRect, Rect valueRect)
    {
        EditorGUI.PropertyField(keyRect, key, GUIContent.none, true);
        EditorGUI.PropertyField(valueRect, value, GUIContent.none, true);
    }
    private float GetHeightOfProperty(SerializedProperty key, SerializedProperty value)
    {
        var keyPropertyHeight = EditorGUI.GetPropertyHeight(key);
        var valuePropertyHeight = EditorGUI.GetPropertyHeight(value);
        return Mathf.Max(keyPropertyHeight, valuePropertyHeight);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            var keyType = GetKeyType(property);
            if (!CheckSupportKeyType(keyType))
                return EditorGUIUtility.singleLineHeight * 3.5f;
            var totalHeight = EditorGUIUtility.singleLineHeight * 4f + GetHeightOfProperty(property.FindPropertyRelative("m_KeyTemp"), property.FindPropertyRelative("m_ValueTemp"));
            var keysSerializedProp = property.FindPropertyRelative("m_Keys");
            var valuesSerializedProp = property.FindPropertyRelative("m_Values");
            for (int i = 0; i < valuesSerializedProp.arraySize; i++)
            {
                var keyProperty = keysSerializedProp.GetArrayElementAtIndex(i);
                var valueProperty = valuesSerializedProp.GetArrayElementAtIndex(i);
                totalHeight += GetHeightOfProperty(keyProperty, valueProperty) + k_KeyValueOffset;
            }
            return totalHeight;
        }
        return EditorGUIUtility.singleLineHeight;
    }
    private void OnClickAddItem(SerializedProperty property, SerializedProperty keysSerializedProp, SerializedProperty valuesSerializedProp)
    {
        AddNewItem(property);
        property.serializedObject.UpdateIfRequiredOrScript();
        property.serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }
    private bool TryGenerateKey(SerializedProperty property, SerializedProperty keysSerializedProp, out object generateKey)
    {
        generateKey = null;
        var firstElementSerializedProp = keysSerializedProp.GetArrayElementAtIndex(0);
        switch (firstElementSerializedProp.propertyType)
        {
            case SerializedPropertyType.ObjectReference:
                break;
            case SerializedPropertyType.Generic:
                return false;
            case SerializedPropertyType.Integer:
                generateKey = default(int);
                break;
            case SerializedPropertyType.Boolean:
                generateKey = default(bool);
                break;
            case SerializedPropertyType.Float:
                generateKey = default(float);
                break;
            case SerializedPropertyType.String:
                generateKey = $"Key_{keysSerializedProp.arraySize}";
                break;
            case SerializedPropertyType.Color:
                generateKey = UnityEngine.Random.ColorHSV();
                break;
            case SerializedPropertyType.LayerMask:
                generateKey = default(LayerMask);
                break;
            case SerializedPropertyType.Enum:
                if (keysSerializedProp.arraySize >= firstElementSerializedProp.enumDisplayNames.Length)
                    return false;
                var listEnumKeys = new List<string>();
                var enumType = GetKeyType(property);
                for (int i = 0; i < firstElementSerializedProp.enumNames.Length; i++)
                {
                    int j;
                    for (j = 0; j < keysSerializedProp.arraySize; j++)
                    {
                        var elementSerializedProp = keysSerializedProp.GetArrayElementAtIndex(j);
                        if (elementSerializedProp.enumValueIndex == i)
                            break;
                    }
                    if (j == keysSerializedProp.arraySize)
                        listEnumKeys.Add(firstElementSerializedProp.enumNames[i]);
                }
                generateKey = Enum.Parse(enumType, listEnumKeys[0]);
                break;
            case SerializedPropertyType.Vector2:
                generateKey = default(Vector2);
                break;
            case SerializedPropertyType.Vector3:
                generateKey = default(Vector3);
                break;
            case SerializedPropertyType.Vector4:
                generateKey = default(Vector4);
                break;
            case SerializedPropertyType.Rect:
                generateKey = default(Rect);
                break;
            case SerializedPropertyType.ArraySize:
                generateKey = default(int);
                break;
            case SerializedPropertyType.Character:
                generateKey = default(char);
                break;
            case SerializedPropertyType.AnimationCurve:
                generateKey = default(AnimationCurve);
                break;
            case SerializedPropertyType.Bounds:
                generateKey = default(Bounds);
                break;
            case SerializedPropertyType.Gradient:
                generateKey = default(Gradient);
                break;
            case SerializedPropertyType.Quaternion:
                generateKey = default(Quaternion);
                break;
            case SerializedPropertyType.ExposedReference:
                return false;
            case SerializedPropertyType.FixedBufferSize:
                return false;
            case SerializedPropertyType.Vector2Int:
                generateKey = default(Vector2Int);
                break;
            case SerializedPropertyType.Vector3Int:
                generateKey = default(Vector3Int);
                break;
            case SerializedPropertyType.RectInt:
                generateKey = default(RectInt);
                break;
            case SerializedPropertyType.BoundsInt:
                generateKey = default(BoundsInt);
                break;
            case SerializedPropertyType.ManagedReference:
                return false;
            default:
                return false;
        }
        return true;
    }
    private object GenerateValue(SerializedProperty property)
    {
        var valueType = GetValueType(property);
        if (valueType.IsValueType)
            return Activator.CreateInstance(valueType);
        return null;
    }
    private bool CheckSupportKeyType(Type type)
    {
        // Check property type is primitive
        if (type.IsPrimitive || type.IsEnum || type == typeof(decimal) || type == typeof(string))
            return true;
        // Check property type is UnityObject
        if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            return true;
        // Check property type is Vector
        if (type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4))
            return true;
        // Check property type is VectorInt
        if (type == typeof(Vector2Int) || type == typeof(Vector3Int))
            return true;
        // Check property type is AnimationCurve or LayerMask
        if (type == typeof(AnimationCurve) || type == typeof(LayerMask))
            return true;
        if (type == typeof(Color) || type == typeof(Color32))
            return true;
        return false;
    }
    private void AddNewItem(SerializedProperty property)
    {
        var genericDictionaryProp = property.serializedObject.targetObject.GetFieldValue<object>(property.propertyPath);
        var method = genericDictionaryProp.GetType().GetMethods(k_BindingFlags).Where(methodInfo => methodInfo.Name == "Add").FirstOrDefault(methodInfo => methodInfo.GetParameters().Length == 2);
        if (method == null)
            return;
#if UNITY_ADDRESSABLE_ASSETS
        if (GetValueType(property).IsAssignableFrom(typeof(UnityEngine.AddressableAssets.AssetReference)))
        {
            var valueTemp = genericDictionaryProp.GetFieldValue<object>("m_ValueTemp") as UnityEngine.AddressableAssets.AssetReference;
            var shallowCopyObject = new UnityEngine.AddressableAssets.AssetReference(valueTemp.AssetGUID);
            shallowCopyObject.SubObjectName = valueTemp.SubObjectName;
            method.Invoke(genericDictionaryProp, new object[] { genericDictionaryProp.GetFieldValue<object>("m_KeyTemp"), shallowCopyObject });
        }
        else if (!GetValueType(property).IsValueType && !GetValueType(property).IsSubclassOf(typeof(UnityEngine.Object)))
        {
            var valueType = GetValueType(property);
            var shallowCopyObject = Activator.CreateInstance(valueType);
            var valueTemp = genericDictionaryProp.GetFieldValue<object>("m_ValueTemp");

            foreach (var originalProp in valueType.GetProperties())
            {
                originalProp.SetValue(shallowCopyObject, originalProp.GetValue(valueTemp));
            }
            foreach (var originalField in valueType.GetFields())
            {
                originalField.SetValue(shallowCopyObject, originalField.GetValue(valueTemp));
            }
            method.Invoke(genericDictionaryProp, new object[] { genericDictionaryProp.GetFieldValue<object>("m_KeyTemp"), shallowCopyObject });
        }
        else
        {
            method.Invoke(genericDictionaryProp, new object[] { genericDictionaryProp.GetFieldValue<object>("m_KeyTemp"), genericDictionaryProp.GetFieldValue<object>("m_ValueTemp") });
        }
#else
        var key = CreateInstance(GetKeyType(property), genericDictionaryProp.GetFieldValue<object>("m_KeyTemp"));
        var value = CreateInstance(GetValueType(property), genericDictionaryProp.GetFieldValue<object>("m_ValueTemp"));
        method.Invoke(genericDictionaryProp, new object[] { key, value });

        object CreateInstance(Type type, object cloneObject)
        {
            if (!type.IsValueType && !type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                var shallowCopyObj = Activator.CreateInstance(type);
                foreach (var originalProp in type.GetProperties())
                {
                    originalProp.SetValue(shallowCopyObj, originalProp.GetValue(cloneObject));
                }
                foreach (var originalField in type.GetFields())
                {
                    originalField.SetValue(shallowCopyObj, originalField.GetValue(cloneObject));
                }
                return shallowCopyObj;
            }
            else
                return cloneObject;
        }
#endif

    }
    private Type GetKeyType(SerializedProperty property) => GetType(property, "GetKeyType");
    private Type GetValueType(SerializedProperty property) => GetType(property, "GetValueType");
    private Type GetType(SerializedProperty property, string method)
    {
        var genericDictionary = property.serializedObject.targetObject.GetFieldValue<object>(property.propertyPath);
        var keyType = genericDictionary.InvokeMethod<Type>(method);
        return keyType;
    }
}
