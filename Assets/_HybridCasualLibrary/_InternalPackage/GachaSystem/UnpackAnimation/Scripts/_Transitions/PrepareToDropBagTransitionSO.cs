using System;
using UnityEngine;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    using StateMachine;

    [CreateAssetMenu(fileName = "Prepare - DropBagTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/PrepareToDropBagTransitionSO")]
    public class PrepareToDropBagTransitionSO : TransitionSO
    {
        readonly PrepareToDropBagEvent prepareEvent = new();
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

        protected class PrepareToDropBagEvent : StateMachine.Event
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
                return controller.CurrentSubPackInfo.cardPlace != CardPlace.NonPack;
            }
        }

    }
}