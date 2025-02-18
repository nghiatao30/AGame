using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CopyPasteTransformShortcut
{
    struct CacheTransform
    {
        public int siblingIndex;
        public TransformData transformData;

        public static CacheTransform Create(int siblingIndex, TransformData transformData)
        {
            return new CacheTransform()
            {
                siblingIndex = siblingIndex,
                transformData = transformData
            };
        }
    }

    private static List<CacheTransform> cacheTransforms;

    [MenuItem("Edit/Copy Transform Value", false)]
    public static void CopyTransformValue()
    {
        if (Selection.gameObjects.Length == 0)
            return;
        cacheTransforms = new List<CacheTransform>();
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            var selectionTransform = Selection.gameObjects[i].transform;
            var transformData = new TransformData(selectionTransform.position, selectionTransform.rotation, selectionTransform.localScale);
            cacheTransforms.Add(CacheTransform.Create(selectionTransform.GetSiblingIndex(), transformData));
        }
    }

    [MenuItem("Edit/Paste Transform Value", false)]
    public static void PasteTransformValue()
    {
        if (cacheTransforms == null)
            return;
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            Transform selectionTransform = Selection.gameObjects[i].transform;
            Undo.RecordObject(selectionTransform, "Paste Transform Value");
            var transformData = GetTransformDataOfIndex(selectionTransform.GetSiblingIndex());
            selectionTransform.transform.position = transformData.position;
            selectionTransform.transform.rotation = transformData.rotation;
            selectionTransform.transform.localScale = transformData.scale;
        }

        TransformData GetTransformDataOfIndex(int index)
        {
            var foundIndex = cacheTransforms.FindIndex(cacheTransform => cacheTransform.siblingIndex == index);
            if (foundIndex == -1)
            {
                return cacheTransforms.OrderByDescending(cacheTransform => cacheTransform.siblingIndex).First().transformData;
            }
            return cacheTransforms[foundIndex].transformData;
        }
    }
}