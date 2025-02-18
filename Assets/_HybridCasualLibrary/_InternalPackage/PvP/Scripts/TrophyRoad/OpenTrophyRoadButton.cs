using UnityEngine;
using HyrphusQ.GUI;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using HyrphusQ.Events;

namespace LatteGames.PvP.TrophyRoad
{
    public class OpenTrophyRoadButton : MonoBehaviour
    {
        [SerializeField] protected TextAdapter currentMedalsText;
        [SerializeField] protected Image currentProgressImage;
        [SerializeField] protected Image highestAchievedProgressImage;
        [SerializeField] protected TrophyRoadSO trophyRoadSO;
        [SerializeField] protected TextMeshProUGUI claimableCountText;

        protected virtual void Awake()
        {
            GameEventHandler.AddActionEvent(TrophyRoadEventCode.OnTrophyRoadClosed, HandleTrophyRoadClosed);
        }

        protected virtual void OnDestroy()
        {
            GameEventHandler.RemoveActionEvent(TrophyRoadEventCode.OnTrophyRoadClosed, HandleTrophyRoadClosed);
        }

        protected virtual IEnumerator Start()
        {
            yield return Yielders.Get(0.1f);
            yield return null;
            UpdateView();
        }

        private void HandleTrophyRoadClosed()
        {
            UpdateView();
        }

        protected virtual void UpdateView()
        {
            if (trophyRoadSO.TryGetHighestArenaProgressValues(out var currentValue, out var highestAchievedValue))
            {
                currentProgressImage.fillAmount = currentValue;
                highestAchievedProgressImage.fillAmount = highestAchievedValue;
            }
            else
            {
                currentProgressImage.fillAmount = highestAchievedProgressImage.fillAmount = 0f;
            }
            currentMedalsText.SetText(currentMedalsText.blueprintText.Replace(Const.StringValue.PlaceholderValue, trophyRoadSO.CurrentMedals.ToRoundedText()));

            var claimableCount = trophyRoadSO.GetClaimableCount();
            if (claimableCount > 0)
            {
                claimableCountText.text = claimableCount.ToString();
            }
            else
            {
                claimableCountText.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
