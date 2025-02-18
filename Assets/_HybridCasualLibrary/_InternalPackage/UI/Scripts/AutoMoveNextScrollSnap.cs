using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
namespace PFI.IAP
{
    public class AutoMoveNextScrollSnap : MonoBehaviour
    {
        [SerializeField] float timeToMoveNext = 5;
        [SerializeField] HorizontalScrollSnap horizontalScrollSnap;
        Coroutine autoMoveNextCoroutine;
        private void OnEnable()
        {
            if (autoMoveNextCoroutine != null)
            {
                StopCoroutine(autoMoveNextCoroutine);
            }
            StartCoroutine(CR_AutoMoveNext());
        }
        private void OnDisable()
        {
            if (autoMoveNextCoroutine != null)
            {
                StopCoroutine(autoMoveNextCoroutine);
            }
        }
        IEnumerator CR_AutoMoveNext()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeToMoveNext);
                if (horizontalScrollSnap.StartDrag)
                {
                    horizontalScrollSnap.NextScreen();
                }
            }
        }
    }
}


