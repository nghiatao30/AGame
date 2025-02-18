using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    using HyrphusQ.Events;
    using StateMachine;

    [CreateAssetMenu(fileName = "OfferBag - CardFlyingUpTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/OfferBagToCardFlyingUpTransitionSO")]
    public class OfferBagToCardFlyingUpTransitionSO : TransitionSO
    {
        readonly OfferBagToCardFlyingUpEvent offerBagEvent = new();

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

            if (parameters[1] is not OpenPackAnimationSM) return;
        }

        protected class OfferBagToCardFlyingUpEvent : StateMachine.Event
        {
            internal OfferNewBagUI offerUI;
            internal System.Action callback;

            public override void Enable()
            {
                SubEvents();
                base.Enable();
            }

            public override void Disable()
            {
                UnsubEvents();
                base.Disable();
            }

            protected virtual void SubEvents()
            {
                if (offerUI == null) return;
                offerUI.OnRVWatched += HandleEventRaised;
            }

            protected virtual void UnsubEvents()
            {
                if (offerUI == null) return;
                offerUI.OnRVWatched -= HandleEventRaised;
            }

            protected void HandleEventRaised()
            {
                if (offerUI == null) return;
                Trigger();
                callback?.Invoke();
            }
        }
    }
}