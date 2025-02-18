using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseForm
{

}
[CustomInspectorName("FormModule")]
public abstract class FormItemModule : ItemModule
{
    public abstract bool hasNextForm { get; }
    public abstract int formCount { get; }
    public abstract int currentFormIndex { get; }
}
public abstract class FormItemModule<T> : FormItemModule where T : BaseForm
{
    [SerializeField]
    protected List<T> m_Forms;

    public override bool hasNextForm => GetNextForm() != null;
    public override int formCount => m_Forms.Count;
    public override int currentFormIndex => 0;

    public virtual T GetCurrentForm() => GetForm(currentFormIndex);
    public virtual T GetNextForm() => GetForm(currentFormIndex + 1);
    public virtual T GetForm (int formIndex)
    {
        if (formIndex < 0 || formIndex >= formCount)
            return null;
        return m_Forms[formIndex];
    }
}