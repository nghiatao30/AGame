using System.Collections;
using System.Collections.Generic;
using LatteGames.Monetization;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAdButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            AdsManager.Instance.RemoveAds();
        });
    }
}
