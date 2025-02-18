using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    using StateMachine;

    [CreateAssetMenu(fileName = "OfferBag - EndTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/OfferBagToEndTransitionSO")]
    public class OfferBagToEndTransitionSO : OfferBagToCardFlyingUpTransitionSO
    {
        readonly OfferBagToEndEvent offerBagEvent = new();

        public override StateMachine.State.Transition Transition
        {
            get
            {
                if (transition == null)
                {
                    transition = new StateMachine.State.Transition(offerBagEvent, targetState.State);
                }
                return base.Transition;
            }
        }

        public override void SetupTransition(object[] parameters)
        {
            if (parameters[0] is not OfferNewBagUI) return;
            offerBagEvent.offerUI = (OfferNewBagUI)parameters[0];
        }

        protected class OfferBagToEndEvent : OfferBagToCardFlyingUpEvent
        {
            protected override void SubEvents()
            {
                if (offerUI == null) return;
                offerUI.OnNoThanksButtonClicked += HandleEventRaised;
            }

            protected override void UnsubEvents()
            {
                if (offerUI == null) return;
                offerUI.OnNoThanksButtonClicked -= HandleEventRaised;
            }
        }
    }
}