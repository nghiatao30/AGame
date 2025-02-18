using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeAsyncTask : IAsyncTask
{
    public CompositeAsyncTask(IAsyncTask[] asyncTasks)
    {
        this.asyncTasks = asyncTasks;
    }
    public CompositeAsyncTask(List<IAsyncTask> asyncTasks)
    {
        this.asyncTasks = asyncTasks.ToArray();
    }

    private event Action _onCompleted;
    public event Action onCompleted
    {
        add
        {
            if (isCompleted)
                value?.Invoke();
            _onCompleted += value;
        }
        remove
        {
            _onCompleted -= value;
        }
    }

    public bool isCompleted => asyncTasks.All(task => task.isCompleted);
    public float percentageComplete => asyncTasks.Average(task => task.percentageComplete);
    public IAsyncTask[] asyncTasks { get; protected set; }
}