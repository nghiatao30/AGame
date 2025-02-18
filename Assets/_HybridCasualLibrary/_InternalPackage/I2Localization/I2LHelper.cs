using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;

public static class I2LHelper
{
    public static string TranslateTerm(I2LTerm term)
    {
        return TranslateTerm(term.ToString());
    }

    public static string TranslateTerm(string term)
    {
        return LocalizationManager.GetTranslation(term.Replace("_", "-"));
    }
}