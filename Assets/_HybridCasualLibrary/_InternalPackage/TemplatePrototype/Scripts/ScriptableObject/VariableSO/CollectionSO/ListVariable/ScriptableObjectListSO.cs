using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "HyrphusQ/CollectionSO/List/ScriptableObject")]
public class ScriptableObjectListSO : ListVariable<ScriptableObject>
{
    public List<T> GetGenericList<T>() where T : ScriptableObject
    {
        return value.Select(item => item as T).ToList();
    }
}