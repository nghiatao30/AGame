using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public abstract class CultureTextFormatConfig : ScriptableObject
{
    [SerializeField]
    protected string m_Format;
    protected CultureInfo m_CultureInfo;

    public virtual string format => m_Format;
    public virtual IFormatProvider formatProvider
    {
        get
        {
            if (m_CultureInfo == null)
                m_CultureInfo = (CultureInfo) CultureInfo.CurrentCulture.Clone();
            return m_CultureInfo;
        }
    }

    public virtual string Format(int number)
    {
        return number.ToString(format, formatProvider);
    }
    public virtual string Format(float number)
    {
        return number.ToString(format, formatProvider);
    }
    public virtual string Format(double number)
    {
        return number.ToString(format, formatProvider);
    }
    public virtual string Format(decimal number)
    {
        return number.ToString(format, formatProvider);
    }
    public virtual string Format(DateTime dateTime)
    {
        return dateTime.ToString(format, formatProvider);
    }
}