﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace LatteGames.Utils{
public static class ListExt
{
    private static Random rnd = new Random();  
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rnd.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
}