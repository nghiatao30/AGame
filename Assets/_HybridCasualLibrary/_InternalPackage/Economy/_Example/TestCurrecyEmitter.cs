using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestCurrecyEmitter : MonoBehaviour
{
    [SerializeField] CurrencyIconsEmitter currencyIconsEmitter;
    [SerializeField] RectTransform source;
    [SerializeField] RectTransform destination;
    [Button("Play")]
    void Play()
    {
        var emission = currencyIconsEmitter.CreateEmission(CurrencyType.Premium, source.position, destination.position, 50, 1, 0.5f, 200);
        emission.RadiateEmit();
    }
}
