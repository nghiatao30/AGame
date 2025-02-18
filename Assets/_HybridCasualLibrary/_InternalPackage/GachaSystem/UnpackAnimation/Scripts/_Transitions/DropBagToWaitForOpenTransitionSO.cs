using System;
using UnityEngine;

namespace LatteGames.UnpackAnimation
{
    using LatteGames.EditableStateMachine;
    using LatteGames.StateMachine;

    [CreateAssetMenu(fileName = "DropBag - WaitForOpenTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/DropBagToWaitForOpenTransitionSO")]
    public class DropBagToWaitForOpenTransitionSO : TransitionSO
    {
        readonly DropBagEvent dropBagEvent = new();

        public override StateMachine.State.Transition Transition
        {
            get
            {
                if (transition == null)
                {
                    transition = new StateMachine.State.Transition(dropBagEvent, targetState.State);
                }
                return base.Transition;
            }
        }

        public override void SetupTransition(object[] parameters)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            dropBagEvent.controller = (OpenPackAnimationSM)parameters[0];
            dropBagEvent.dropBagTime = (float)Convert.ToDouble(parameters[1]);
        }

        class DropBagEvent : StateMachine.Event
        {
            internal OpenPackAnimationSM controller;
            internal float dropBagTime;
            float elapsedTime;

            public override void Enable()
            {
                base.Enable();
                if (Enabled == false) return;
                elapsedTime = 0;
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
                elapsedTime += Time.deltaTime;
                if (elapsedTime < dropBagTime) return false;
                return controller.CurrentSubPackInfo.cardPlace == CardPlace.NormalPack;
            }
        }
    }
}