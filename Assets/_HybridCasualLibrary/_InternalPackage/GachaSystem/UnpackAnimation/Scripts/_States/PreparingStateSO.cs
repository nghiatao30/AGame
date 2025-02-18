using UnityEngine;
using DG.Tweening;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    [CreateAssetMenu(fileName = "PreparingStateSO", menuName = "LatteGames/ScriptableObject/GachaSystem/StateSO/PreparingStateSO")]
    public class PreparingStateSO : StateSO
    {
        protected OpenPackAnimationSM controller;

        protected virtual bool HasSetup
        {
            get
            {
                if (controller == null) return false;
                return true;
            }
        }

        public override void SetupState(object[] parameters)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            controller = (OpenPackAnimationSM)parameters[0];
            foreach (var transition in transitions)
            {
                transition.SetupTransition(new object[] { controller });
            }
        }

        protected override void StateEnable()
        {
            if (HasSetup == false) return;
            controller.StartOpenSubPack();
        }

        protected override void StateDisable()
        {
            if (HasSetup == false) return;
        }

        protected override void StateUpdate()
        {
            //Do nothing
        }
    }
}