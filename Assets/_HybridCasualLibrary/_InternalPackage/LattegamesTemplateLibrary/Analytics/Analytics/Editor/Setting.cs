using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Analytics
{ 
    [CreateAssetMenu(fileName = "Setting", menuName = "LatteGames/ScriptableObject/Analytics/Setting", order = 0)]
    public class Setting : ScriptableObject {
        public PresetSetting preset;
    }
}