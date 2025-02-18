using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Utils{
public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
}