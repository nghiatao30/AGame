using System.Collections;
using System.Collections.Generic;
using LatteGames.Monetization;
using UnityEngine;
using UnityEngine.Events;

public class RVReadyStateHandle : MonoBehaviour
{
    public UnityEvent OnRVReady;
    public UnityEvent OnRVNotReady;

    Coroutine coroutine;

    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    private void OnEnable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(CR_OnEnable());
    }

    IEnumerator CR_OnEnable()
    {
        while (true)
        {
            if (!AdsManager.Instance.IsReadyRewarded)
            {
                OnRVNotReady?.Invoke();
                yield return new WaitUntil(() => AdsManager.Instance.IsReadyRewarded);
            }
            else
            {
                OnRVReady?.Invoke();
                yield return new WaitUntil(() => !AdsManager.Instance.IsReadyRewarded);
            }
        }
    }
}
