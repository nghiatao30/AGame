using UnityEngine;
using DG.Tweening;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    [CreateAssetMenu(fileName = "OfferBagStateSO", menuName = "LatteGames/ScriptableObject/GachaSystem/StateSO/OfferBagStateSO")]
    public class OfferBagStateSO : StateSO
    {
        [SerializeField] protected OfferNewBagUI offerNewBagUIPrefab;
        [SerializeField] protected float camRotateToEndRotDuration;

        protected OfferNewBagUI offerNewBagUIInstance;
        protected OpenPackAnimationSM controller;
        protected Bag bagInstance;
        protected Camera camera;

        protected virtual bool HasSetup
        {
            get
            {
                if (bagInstance == null) return false;
                if (camera == null) return false;
                return true;
            }
        }

        public override void SetupState(object[] parameters = null)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            controller = (OpenPackAnimationSM)parameters[0];
            offerNewBagUIInstance = Instantiate(offerNewBagUIPrefab, controller.InteractiveCanvas);
            offerNewBagUIInstance.HideImmediately();
            camera = controller.Camera;
            foreach (var transition in transitions)
            {
                transition.SetupTransition(new object[] { offerNewBagUIInstance, controller });
            }
        }

        protected override void StateEnable()
        {
            bagInstance = (Bag)controller.PackInstance;
            if (HasSetup == false) return;
            offerNewBagUIInstance.gameObject.SetActive(true);
            offerNewBagUIInstance.Setup(controller.CurrentSubPackInfo.offerRVAdsLocation);
            offerNewBagUIInstance.OnRVWatched += OnRVWatched;
            offerNewBagUIInstance.Show();
            controller.PackNameTxt.gameObject.SetActive(true);
            controller.FreeTxt.gameObject.SetActive(true);
            var displayNameSplit = controller.CurrentGachaPack.GetDisplayName().Split(" - ");
            controller.PackNameTxt.text = displayNameSplit.Length <= 0 ? "Pack Name" : displayNameSplit[0];
            controller.PackNameTxt.gameObject.SetActive(displayNameSplit.Length >= 1);
            controller.FreeTxt.text = displayNameSplit.Length <= 1 ? "Free" : displayNameSplit[1];
            controller.FreeTxt.gameObject.SetActive(displayNameSplit.Length >= 2);
            bagInstance.packGameObject.SetActive(true);
            bagInstance.packTransform.position = bagInstance.endDropPoint.transform.position;
            camera.transform.rotation = bagInstance.packCenterCamRot.transform.rotation;
        }

        protected virtual void OnRVWatched()
        {
            offerNewBagUIInstance.OnRVWatched -= OnRVWatched;
            controller.GrantRewardOfferPack();
        }

        protected override void StateDisable()
        {
            offerNewBagUIInstance.HideImmediately();
            if (HasSetup == false) return;
            controller.PackNameTxt.gameObject.SetActive(false);
            controller.FreeTxt.gameObject.SetActive(false);
            camera.DOKill();
            camera.transform.DORotateQuaternion(bagInstance.endCamDropRot.transform.rotation, camRotateToEndRotDuration);
            bagInstance.packTransform.position = bagInstance.endDropPoint.transform.position;
        }

        protected override void StateUpdate()
        {
            //Do nothing
        }
    }
}
