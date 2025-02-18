using System;
using UnityEngine;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    using StateMachine;

    [CreateAssetMenu(fileName = "Prepare - CardDropDownTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/PrepareToCardDropDownTransitionSO")]
    public class PrepareToCardDropDownTransitionSO : TransitionSO
    {
        readonly PrepareToCardDropDownEvent prepareEvent = new();
        public override StateMachine.State.Transition Transition
        {
            get
            {
                if (transition == null)
                {
                    transition = new StateMachine.State.Transition(prepareEvent, targetState.State);
                }
                return base.Transition;
            }
        }

        public override void SetupTransition(object[] parameters)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            prepareEvent.controller = (OpenPackAnimationSM)parameters[0];
        }

        protected class PrepareToCardDropDownEvent : StateMachine.Event
        {
            internal OpenPackAnimationSM controller;

            public override void Enable()
            {
                base.Enable();
            }

            public override void Update()
            {
                base.Update();
                if (Enabled == false) return;
                if (CheckCondition())
                {
                    Trigger();
                }
            }

            protected bool CheckCondition()
            {
                if (controller == null) return false;
                return controller.CurrentSubPackInfo.cardPlace == CardPlace.NonPack;
            }
        }

    }
}