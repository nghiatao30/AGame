using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HyrphusQ.Events;

public class PlayerAvatarUI : MonoBehaviour
{
    [SerializeField]
    private Image m_AvatarImage;
    [SerializeField]
    private Image m_FrameImage;
    [SerializeField]
    private ItemManagerSO m_AvatarManagerSO;
    [SerializeField]
    private AvatarManagerSO m_FrameManagerSO;

    private void Start()
    {
        GameEventHandler.AddActionEvent(m_AvatarManagerSO.itemUsedEventCode, OnAvatarInUseChanged);
        GameEventHandler.AddActionEvent(m_FrameManagerSO.itemUsedEventCode, OnFrameInUseChanged);

        OnAvatarInUseChanged(null, m_AvatarManagerSO.currentItemInUse);
        OnFrameInUseChanged(null, m_FrameManagerSO.currentItemInUse);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(m_AvatarManagerSO.itemUsedEventCode, OnAvatarInUseChanged);
        GameEventHandler.RemoveActionEvent(m_FrameManagerSO.itemUsedEventCode, OnFrameInUseChanged);
    }

    private void OnAvatarInUseChanged(params object[] eventData)
    {
        if (eventData == null || eventData.Length <= 0)
            return;
        var avatarItem = eventData[1] as ItemSO;
        if (avatarItem == null)
            return;
        m_AvatarImage.sprite = avatarItem.GetThumbnailImage();
    }

    private void OnFrameInUseChanged(params object[] eventData)
    {
        if (eventData == null || eventData.Length <= 0)
            return;
        var frameItem = eventData[1] as ItemSO;
        if (frameItem == null)
            return;
        m_FrameImage.sprite = frameItem.GetThumbnailImage();
    }
}