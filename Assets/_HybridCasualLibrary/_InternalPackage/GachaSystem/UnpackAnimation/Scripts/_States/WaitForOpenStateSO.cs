using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LatteGames.EditableStateMachine;
using TMPro;
using I2.Loc;

namespace LatteGames.UnpackAnimation
{
    [CreateAssetMenu(fileName = "WaitForOpenStateSO", menuName = "LatteGames/ScriptableObject/GachaSystem/StateSO/WaitForOpenStateSO")]
    public class WaitForOpenStateSO : StateSO
    {
        [SerializeField] protected bool isAbleToSkip = true;
        [SerializeField] protected float delayToShowTapCTA;
        [SerializeField] protected float camRotateToEndRotDuration;

        protected float elapsedTime = 0;
        protected bool isCTAShowed = false;
        protected CanvasGroup unpackCanvasGroup;
        protected OpenPackAnimationSM controller;

        protected Bag bagInstance;
        protected Camera camera;
        protected TMP_Text tapToOpenCTAText;

        protected virtual bool HasSetup
        {
            get
            {
                if (tapToOpenCTAText == null) return false;
                if (bagInstance == null) return false;
                if (camera == null) return false;
                if (unpackCanvasGroup == null) return false;
                return true;
            }
        }

        protected virtual bool IsAbleToSkip
        {
            get => isAbleToSkip;
        }

        public override void SetupState(object[] parameters = null)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            controller = (OpenPackAnimationSM)parameters[0];
            tapToOpenCTAText = controller.TapToOpenCTA;
            camera = controller.Camera;
            unpackCanvasGroup = controller.UnpackUICanvasGroup;
            foreach (var transition in transitions)
            {
                transition.SetupTransition(new object[] { controller });
            }
        }

        protected override void StateEnable()
        {
            bagInstance = (Bag)controller.PackInstance;
            if (HasSetup == false) return;
            bagInstance.packGameObject.SetActive(true);
            bagInstance.packTransform.position = bagInstance.endDropPoint.transform.position;
            camera.transform.rotation = bagInstance.packCenterCamRot.transform.rotation;
            elapsedTime = 0;
            isCTAShowed = false;
            unpackCanvasGroup.alpha = 1;
            var displayNameSplit = controller.CurrentGachaPack.GetDisplayName().Split(" - ");
            controller.PackNameTxt.text = displayNameSplit.Length <= 0 ? "Pack Name" : displayNameSplit[0];
            controller.PackNameTxt.gameObject.SetActive(displayNameSplit.Length >= 1);
            controller.FreeTxt.text = displayNameSplit.Length <= 1 ? "Free" : displayNameSplit[1];
            controller.FreeTxt.gameObject.SetActive(displayNameSplit.Length >= 2);
            controller.SkipBtn.gameObject.SetActive(IsAbleToSkip);
            tapToOpenCTAText.gameObject.SetActive(true);
            tapToOpenCTAText.DOKill();
            tapToOpenCTAText.color = Color.white.ToTransparent();
        }

        protected override void StateDisable()
        {
            if (HasSetup == false) return;
            camera.DOKill();
            camera.transform.DORotateQuaternion(bagInstance.endCamDropRot.transform.rotation, camRotateToEndRotDuration);
            bagInstance.packTransform.position = bagInstance.endDropPoint.transform.position;
            controller.PackNameTxt.gameObject.SetActive(false);
            controller.FreeTxt.gameObject.SetActive(false);
            controller.SkipBtn.gameObject.SetActive(false);
            tapToOpenCTAText.gameObject.SetActive(false);
            tapToOpenCTAText.DOKill();
            tapToOpenCTAText.color = Color.white.ToTransparent();
            isCTAShowed = true;
        }

        protected override void StateUpdate()
        {
            if (isCTAShowed == true) return;
            if (elapsedTime >= delayToShowTapCTA)
            {
                isCTAShowed = true;
                tapToOpenCTAText.gameObject.SetActive(true);
                tapToOpenCTAText.DOFade(1, 0.3f);
            }
            elapsedTime += Time.deltaTime;
        }
    }
}