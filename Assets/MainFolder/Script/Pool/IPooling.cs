using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooling<T> 
{   
    int count
    {
        get;
    }

    void Clear();
    T PullOut();
    void PullIn(T item);
}
