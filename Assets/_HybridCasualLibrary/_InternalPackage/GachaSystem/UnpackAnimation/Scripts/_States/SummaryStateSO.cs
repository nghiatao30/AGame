using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GachaSystem.Core;
using LatteGames.EditableStateMachine;
using LatteGames.Template;

namespace LatteGames.UnpackAnimation
{
    [CreateAssetMenu(fileName = "SummaryStateSO", menuName = "LatteGames/ScriptableObject/GachaSystem/StateSO/SummaryStateSO")]
    public class SummaryStateSO : StateSO
    {
        [SerializeField] protected float delayToShowSummary;
        [SerializeField] protected float fadeUnpackCanvasGroupDuration = 0.3f;
        [SerializeField] protected float yOffsetPerSummaryRow;
        [SerializeField] protected float minSummaryShowingTime = 1f;
        [SerializeField] protected SummaryUI summaryUIPrefab;
        [SerializeField] protected GameObject tapToContinueCTAPrefab;

        [HideInInspector] public bool hasShown = false;

        //------ Instance ---------
        protected SummaryUI summaryUIInstance;
        protected CanvasGroup unpackCanvasGroup;
        protected GameObject tapToContinueCTAInstance;
        protected OpenPackAnimationSM controller;

        //------ Values ---------
        protected Coroutine summaryStateCoroutine;
        protected Vector3 originalSummaryUIPos;

        protected virtual bool HasSetup
        {
            get
            {
                if (controller == null) return false;
                if (summaryUIInstance == null) return false;
                if (unpackCanvasGroup == null) return false;
                if (tapToContinueCTAInstance == null) return false;
                return true;
            }
        }

        public override void SetupState(object[] parameters = null)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            controller = (OpenPackAnimationSM)parameters[0];
            summaryUIInstance = Instantiate(summaryUIPrefab, controller.NonInteractiveCanvas);
            summaryUIInstance.HideImmediately();
            originalSummaryUIPos = summaryUIInstance.rect.anchoredPosition;
            unpackCanvasGroup = controller.UnpackUICanvasGroup;
            tapToContinueCTAInstance = Instantiate(tapToContinueCTAPrefab, controller.NonInteractiveCanvas);
            tapToContinueCTAInstance.SetActive(false);
            foreach (var transition in transitions)
            {
                transition.SetupTransition(new object[] { controller, summaryUIInstance, minSummaryShowingTime });
            }
        }

        protected override void StateEnable()
        {
            if (HasSetup == false) return;
            if (summaryStateCoroutine != null)
            {
                summaryUIInstance.StopCoroutine(summaryStateCoroutine);
                summaryStateCoroutine = null;
            }
            summaryStateCoroutine = controller.StartCoroutine(CR_ShowSummaryUI());
            var bagInstance = (Bag)controller.PackInstance;

            if (bagInstance != null)
            {
                if (bagInstance.packShadow != null)
                    bagInstance.packShadow.SetActive(true);

                if (bagInstance.packGameObject != null)
                {
                    SkinnedMeshRenderer boxSkinMesh = bagInstance.packGameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (boxSkinMesh != null)
                        boxSkinMesh.enabled = true;
                }
            }

            if (bagInstance != null && bagInstance.lightFX != null && !bagInstance.lightFX.isPlaying)
            {
                bagInstance.packAnimator.SetTrigger("FirstOpen");
                bagInstance.lightFX.Play();
                SoundManager.Instance.PlaySFX(GeneralSFX.UIBoxShaking);
            }
        }

        protected override void StateDisable()
        {
            if (HasSetup == false) return;
            unpackCanvasGroup.DOKill();
            unpackCanvasGroup.alpha = 0;
            summaryUIInstance.Hide();
            tapToContinueCTAInstance.SetActive(false);
        }

        protected override void StateUpdate()
        {
            //Do nothing
        }

        protected virtual int CalMaxCellAmountPerRow(int cardCount)
        {
            int maxCellAmountPerRow = 4;
            if (cardCount > 12)
            {
                maxCellAmountPerRow = 5;
            }
            else if (cardCount <= 3)
            {
                maxCellAmountPerRow = cardCount;
            }
            else if (cardCount <= 10)
            {
                maxCellAmountPerRow = Mathf.CeilToInt((float)cardCount / 2);
            }
            return maxCellAmountPerRow;
        }

        protected virtual IEnumerator CR_ShowSummaryUI()
        {
            yield return CommonCoroutine.GetWaitForSeconds(delayToShowSummary);
            unpackCanvasGroup.DOFade(0, fadeUnpackCanvasGroupDuration);
            yield return CommonCoroutine.GetWaitForSeconds(fadeUnpackCanvasGroupDuration);
            summaryUIInstance.Show();
            tapToContinueCTAInstance.SetActive(true);

            //Setup cards & Calculate summary y position
            var cards = controller.CurrentGroupedCards;
            int maxCellAmountPerRow = CalMaxCellAmountPerRow(cards.Count);
            int maxRowAmount = 3;
            int rowAmount = Mathf.CeilToInt((float)cards.Count / maxCellAmountPerRow);
            summaryUIInstance.rect.anchoredPosition = originalSummaryUIPos + yOffsetPerSummaryRow * (maxRowAmount - rowAmount) * Vector3.down;
            summaryUIInstance.SetupCards(cards, maxCellAmountPerRow);
        }
    }
}