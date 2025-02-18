using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.UnpackAnimation
{
    using StateMachine;

    [CreateAssetMenu(fileName = "Summary - EndTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/SummaryToEndTransitionSO")]
    public class SummaryToEndTransitionSO : SummaryToPrepareTransitionSO
    {
        readonly SummaryToEndEvent summaryEvent = new();
        public override StateMachine.State.Transition Transition
        {
            get
            {
                if (transition == null)
                {
                    transition = new StateMachine.State.Transition(summaryEvent, targetState.State);
                }
                return base.Transition;
            }
        }

        public override void SetupTransition(object[] parameters)
        {
            if (parameters[0] is not OpenPackAnimationSM openPackController) return;
            summaryEvent.controller = openPackController;
            summaryEvent.summaryUI = (SummaryUI)parameters[1];
            summaryEvent.minShowingCardTime = (float)Convert.ToDouble(parameters[2]);
        }

        protected class SummaryToEndEvent : SummaryToPrepareEvent
        {
            protected override bool matchCondition => controller != null && controller.IsLastSubPack;
        }
    }
}