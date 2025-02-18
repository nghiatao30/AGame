using System.Collections;
using System.Collections.Generic;
using LatteGames.EditableStateMachine;
using UnityEngine;

namespace LatteGames.UnpackAnimation
{
    using LatteGames.StateMachine;

    [CreateAssetMenu(fileName = "WaitForOpen - SummaryTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/WaitForOpenToSummaryTransitionSO")]
    public class WaitForOpenToSummaryTransitionSO : TransitionSO
    {
        readonly OpenPackAnimationSM.SkipEvent skipEvent = new();

        public override StateMachine.State.Transition Transition
        {
            get
            {
                if (transition == null)
                {
                    transition = new StateMachine.State.Transition(skipEvent, targetState.State);
                }
                return base.Transition;
            }
        }

        public override void SetupTransition(object[] parameters)
        {
            if (parameters[0] is not OpenPackAnimationSM openPackController) return;
            skipEvent.controller = openPackController;
        }
    }
}