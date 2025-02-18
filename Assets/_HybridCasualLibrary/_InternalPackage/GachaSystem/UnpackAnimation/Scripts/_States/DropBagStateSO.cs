using UnityEngine;
using DG.Tweening;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    [CreateAssetMenu(fileName = "DropBagStateSO", menuName = "LatteGames/ScriptableObject/GachaSystem/StateSO/DropBagStateSO")]
    public class DropBagStateSO : StateSO
    {
        [SerializeField] protected float dropDuration;
        [SerializeField] protected AnimationCurve bagDroppingCurve, camFollowingCurve;

        protected OpenPackAnimationSM controller;
        protected Bag bagInstance;
        protected Camera camera;
        protected CanvasGroup unpackCanvasGroup;

        protected Sequence sequence;

        protected virtual bool HasSetup
        {
            get
            {
                if (controller == null) return false;
                if (bagInstance == null) return false;
                if (camera == null) return false;
                return true;
            }
        }

        public override void SetupState(object[] parameters)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            controller = (OpenPackAnimationSM)parameters[0];
            camera = controller.Camera;
            unpackCanvasGroup = controller.UnpackUICanvasGroup;
            foreach (var transition in transitions)
            {
                transition.SetupTransition(new object[] { controller, dropDuration });
            }
        }

        protected override void StateEnable()
        {
            bagInstance = (Bag)controller.PackInstance;
            if (HasSetup == false) return;
            unpackCanvasGroup.alpha = 0;
            bagInstance.packGameObject.SetActive(true);
            bagInstance.packTransform.position = bagInstance.startDropPoint.transform.position;
            camera.transform.rotation = bagInstance.startCamDropRot.transform.rotation;
            sequence.Kill();
            sequence = DOTween.Sequence();
            sequence
                .Join(camera.transform.DORotateQuaternion(bagInstance.packCenterCamRot.transform.rotation, dropDuration).SetEase(camFollowingCurve))
                .Join(bagInstance.packTransform.DOMove(bagInstance.endDropPoint.transform.position, dropDuration, false).SetEase(bagDroppingCurve))
                .Play().OnComplete(() =>
                {
                    bagInstance.packDropOnGroundFX.PlayAnim();
                });
            bagInstance.packAnimator.SetTrigger("DropBox");
        }

        protected override void StateDisable()
        {
            if (HasSetup == false) return;
            //sequence.Kill();
            camera.transform.rotation = bagInstance.packCenterCamRot.transform.rotation;
            bagInstance.packTransform.position = bagInstance.endDropPoint.transform.position;
        }

        protected override void StateUpdate()
        {
            //Do nothing
        }
    }
}