using System;
using UnityEngine;

namespace LatteGames.UnpackAnimation
{
    using StateMachine;

    [CreateAssetMenu(fileName = "ShowingCard - CardFlyingUpTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/ShowingCardToCardFlyingUpTransitionSO")]
    public class ShowingCardToCardFlyingUpTransitionSO : ShowingCardToSummaryTransitionSO
    {
        readonly ShowingCardToFlyingUpEvent showingCardEvent = new();

        public override StateMachine.State.Transition Transition
        {
            get
            {
                if (transition == null)
                {
                    transition = new StateMachine.State.Transition(showingCardEvent, targetState.State);
                }
                return base.Transition;
            }
        }

        public override void SetupTransition(object[] parameters)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            showingCardEvent.controller = (OpenPackAnimationSM)parameters[0];
            showingCardEvent.cardController = (AbstractCardController)parameters[1];
            showingCardEvent.minShowingCardTime = (float)Convert.ToDouble(parameters[2]);
        }

        protected class ShowingCardToFlyingUpEvent : ShowingCardToSummaryEvent
        {
            protected override bool isMatchCondition => controller.RemainingItemAmount > 0 && controller.CurrentSubPackInfo.cardPlace != CardPlace.NonPack;
        }
    }
}