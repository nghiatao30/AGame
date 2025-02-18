using System.Collections;
using System.Collections.Generic;
using PFI.IAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeBasedTextUpdater : MonoBehaviour
{
    [SerializeField] string preContent;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TimeBasedRewardSO timeBasedRewardSO;
    [SerializeField] bool getTimeInMinute;
    Coroutine updateCoroutine;
    private void OnEnable()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
        updateCoroutine = StartCoroutine(CR_Update());
    }
    IEnumerator CR_Update()
    {
        while (true)
        {
            timerText.text = $"{preContent}{(!getTimeInMinute ? timeBasedRewardSO.remainingTime : timeBasedRewardSO.remainingTimeInMinute)}";
            yield return Yielders.Get(1);
        }
    }
}