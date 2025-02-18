using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using HyrphusQ.SerializedDataStructure;

[CreateAssetMenu(fileName = "CurrencyEmitterConfigSO", menuName = "LatteGames/Economy/CurrencyEmitterConfigSO")]
public class CurrencyEmitterConfigSO : ScriptableObject
{
    [Title("Default Config", titleAlignment: TitleAlignments.Centered)]
    public float emitDuration = 1;
    public float moveDuration = 0.5f;
    public float startRadius = 100f;
    public float endRadius = 0f;
    public SerializedDictionary<CurrencyType, AnimationCurve> currencyIconAmountCurve = new SerializedDictionary<CurrencyType, AnimationCurve>();

    [Title("Radiate Emit", titleAlignment: TitleAlignments.Centered)]
    public float radiateDuration = 0.2f;
    public float delayToMove = 0.3f;
    public float initScale = 1.3f;
    public Vector2 radiateDelayRange = new Vector2(0, 0.3f);
    public Vector2 moveDelayRange = new Vector2(0, 0.3f);
}