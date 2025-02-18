using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "NumberFormatConfig", menuName = "HyrphusQ/Misc/CultureTextFormatConfig/NumberFormatConfig")]
public class NumberFormatConfig : CultureTextFormatConfig
{
    [SerializeField]
    protected int m_NumberDecimalDigits = 0;
    [SerializeField]
    protected string m_NumberDecimalSeparator = ",";
    [SerializeField]
    protected string m_NumberGroupSeparator = ".";

    public override IFormatProvider formatProvider
    {
        get
        {
            var cultureInfo = base.formatProvider as CultureInfo;
            var numberFormat = cultureInfo.NumberFormat;
            numberFormat.NumberDecimalDigits = m_NumberDecimalDigits;
            numberFormat.NumberDecimalSeparator = m_NumberDecimalSeparator;
            numberFormat.NumberGroupSeparator = m_NumberGroupSeparator;
            return cultureInfo;
        }
    }
}