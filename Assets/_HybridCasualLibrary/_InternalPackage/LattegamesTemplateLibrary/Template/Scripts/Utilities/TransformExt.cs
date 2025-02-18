using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace LatteGames.Utils{
public static class TransformExt
{
    public static void ResetTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void CopyTransfrom(this Transform transform, Transform other)
    {
        transform.position = other.position;
        transform.rotation = other.rotation;
        Vector3 localscale = other.lossyScale;
        if(transform.parent != null)
            localscale = transform.parent.InverseTransformVector(localscale);
        transform.localScale = localscale;
    }

    public static void CopyTransfromRecursive(this Transform transform, Transform other)
    {
        for (int i = 0; i < Math.Min(transform.childCount, other.childCount); i++)
            CopyTransfromRecursive(transform.GetChild(i), other.GetChild(i));
        CopyTransfrom(transform, other);
    }

    public static void Flattern(this Transform transform, bool addParentConstraint, Transform container = null)
    {
        var parent = transform.parent;
        transform.parent = container;
        if(addParentConstraint && parent != null)
            transform.gameObject.AddComponent<ParentConstraint>().AddSource(new ConstraintSource(){
                sourceTransform = parent,
                weight = 1
            });
        for (int i = 0; i < transform.childCount; i++)
            Flattern(transform.GetChild(i), addParentConstraint, container);   
    }

    public static void DestroyChildren(this Transform transform)
    {
        var childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if(Application.isPlaying)
                GameObject.Destroy(transform.GetChild(childCount - i - 1).gameObject);
            else
                GameObject.DestroyImmediate(transform.GetChild(childCount - i - 1).gameObject);    
        }
    }

    public static List<Transform> GetChildren(this Transform transform)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }
        return children;
    }
}
}