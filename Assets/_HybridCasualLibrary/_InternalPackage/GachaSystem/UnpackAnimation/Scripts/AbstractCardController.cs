using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.SerializedDataStructure;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LatteGames;
using GachaSystem.Core;

namespace LatteGames.UnpackAnimation
{
    public abstract class AbstractCardController : MonoBehaviour
    {
        [HideInInspector] public bool IsAnimationEnded = false;

        public abstract void Setup(DuplicateGachaCardsGroup cardInfo, bool isInSummary = false);
        public abstract void ToggleTitleVisibility(bool isVisible);
    }
}