using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAsyncTask
{
    event Action onCompleted;

    bool isCompleted { get; }
    float percentageComplete { get; }
}
public interface IAsyncTask<T> : IAsyncTask
{
    T result { get; }
}