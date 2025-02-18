using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Sirenix.OdinInspector;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] protected Image darkness;
    [SerializeField] protected RectTransform board;
    [SerializeField] protected float transitionDuration = .5f;
    [SerializeField] protected float boardHiddingOffset = 20;
    [SerializeField] protected AnimationCurve easeIn;
    [SerializeField] protected Ease easeOut;
    [SerializeField] protected LeaderboardManagerSO leaderboardManagerSO;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected Leaderboard leaderboard;

    protected float darknessAlpha;
    protected float boardStartY;

    protected virtual void Awake()
    {
        darknessAlpha = darkness.color.a;
        boardStartY = board.anchoredPosition.y;
        canvasGroup.alpha = 0f;
    }

    protected virtual IEnumerator Start()
    {
        yield return new WaitForSeconds(0.01f);
        if (canvasGroup)
        {
            Show();
        }
    }

    [Button, ButtonGroup]
    public virtual void Show()
    {
        darkness.gameObject.SetActive(true);
        darkness.DOFade(darknessAlpha, transitionDuration);
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.DOFade(1f, transitionDuration);
        board.DOAnchorPosY(boardStartY, transitionDuration).SetEase(easeIn);

        leaderboardManagerSO.TriggerSimulation();
        leaderboard.Refresh();
    }

    [Button, ButtonGroup]
    public virtual void Hide(float duration = -1)
    {
        if (duration == -1)
            duration = transitionDuration;
        darkness.DOFade(0, duration).OnComplete(() => darkness.gameObject.SetActive(false));
        canvasGroup.DOFade(0f, duration).OnComplete(() => canvasGroup.gameObject.SetActive(false));
        board.DOAnchorPosY(-(board.rect.height + boardHiddingOffset), duration).SetEase(easeOut);
    }
}
