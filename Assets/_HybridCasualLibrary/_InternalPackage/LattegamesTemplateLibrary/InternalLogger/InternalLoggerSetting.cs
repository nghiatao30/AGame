using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InternalLoggerSetting", menuName = "LatteGames/ScriptableObject/InternalLoggerSetting", order = 0)]
public class InternalLoggerSetting : ScriptableObject {
    public List<string> EnabledTags = new List<string>(){"default"};
    private static InternalLoggerSetting instance;
    public static InternalLoggerSetting Instance
    {
        get{
            if(instance == null)
                instance = Resources.Load<InternalLoggerSetting>("LatteGames/InternalLoggerSetting");
            return instance;
        }
    }
}