using UnityEngine;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    using LatteGames.StateMachine;

    [CreateAssetMenu(fileName = "WaitForOpen - CardFlyingUpTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/WaitForOpenToCardFlyingUpTransitionSO")]
    public class WaitForOpenToFlyingCardTransitionSO : TransitionSO
    {
        readonly OpenPackAnimationSM.MouseClickEvent waitForOpenEvent = new();

        public override StateMachine.State.Transition Transition
        {
            get
            {
                if(transition == null)
                {
                    transition = new StateMachine.State.Transition(waitForOpenEvent, targetState.State);
                }
                return base.Transition;
            }
        }

        public override void SetupTransition(object[] parameters)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            var controller = (OpenPackAnimationSM)parameters[0];
            waitForOpenEvent.controller = controller;
        }
    }
}