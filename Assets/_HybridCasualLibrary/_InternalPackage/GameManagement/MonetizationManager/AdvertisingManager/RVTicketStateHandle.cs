using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using LatteGames.Monetization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RVTicketStateHandle : MonoBehaviour
{
    [SerializeField] Vector2 rvTicketIconImgOffset;
    [SerializeField] Image iconImg;
    [SerializeField] Sprite adsSprite, ticketSprite;
    [SerializeField] RVReadyStateHandle m_RVReadyStateHandle;

    Vector2 originalAnchoredPos;
    CurrencySO rvTicketCurrencySO;

    private void Awake()
    {
        rvTicketCurrencySO = CurrencyManager.Instance.GetCurrencySO(CurrencyType.RVTicket);
        rvTicketCurrencySO.onValueChanged += OnRVTicketValueChanged;
    }

    private IEnumerator Start()
    {
        // Delay 1 frame after layout group calculation
        yield return null;
        originalAnchoredPos = iconImg.rectTransform.anchoredPosition;
        UpdateView();
    }

    private void OnDestroy()
    {
        rvTicketCurrencySO.onValueChanged -= OnRVTicketValueChanged;
    }

    void OnRVTicketValueChanged(ValueDataChanged<float> data)
    {
        UpdateView();
    }

    void UpdateView()
    {
        if (CurrencyManager.Instance.IsAffordable(CurrencyType.RVTicket, RVButtonBehaviorConfigs.RV_TICKET_CONVERSION_RATE))
        {
            iconImg.rectTransform.anchoredPosition = originalAnchoredPos + rvTicketIconImgOffset;
            iconImg.sprite = ticketSprite;
            if (m_RVReadyStateHandle != null)
            {
                m_RVReadyStateHandle.enabled = false;
                m_RVReadyStateHandle.OnRVReady.Invoke();
            }
        }
        else
        {
            iconImg.rectTransform.anchoredPosition = originalAnchoredPos;
            iconImg.sprite = adsSprite;
            if (m_RVReadyStateHandle != null)
            {
                m_RVReadyStateHandle.enabled = true;
            }
        }
    }
}