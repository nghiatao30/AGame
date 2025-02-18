using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Utils{
public class FPSRequest : MonoBehaviour
{
    [SerializeField] private int targetFPS = 60;
    private void Awake() {
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = 0;
    }
}
}