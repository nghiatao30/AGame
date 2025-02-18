using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpdateTimeBasedRewardSO : MonoBehaviour
{
    public UnityEvent OnGetReward;
    [SerializeField] TimeBasedRewardSO timeBasedRewardSO;
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
            yield return new WaitUntil(() => timeBasedRewardSO.canGetReward);
            timeBasedRewardSO.GetReward();
            OnGetReward?.Invoke();
        }
    }
}