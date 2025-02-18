using UnityEngine;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    using StateMachine;

    [CreateAssetMenu(fileName = "CardFlyingUp - ShowingCardTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/CardFlyingUpToShowingCardTransitionSO")]
    public class CardFlyingUpToShowingCardTransitionSO : TransitionSO
    {
        readonly CardFlyingUpEvent cardFlyingUpEvent = new();

        public override StateMachine.State.Transition Transition
        {
            get
            {
                if (transition == null)
                {
                    transition = new StateMachine.State.Transition(cardFlyingUpEvent, targetState.State);
                }
                return base.Transition;
            }
        }

        public override void SetupTransition(object[] parameters)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            cardFlyingUpEvent.controller = (OpenPackAnimationSM)parameters[0];
            cardFlyingUpEvent.cardFXRect = (RectTransform)parameters[1];
            cardFlyingUpEvent.targetCardPos = cardFlyingUpEvent.cardFXRect.anchoredPosition;
        }

        class CardFlyingUpEvent : OpenPackAnimationSM.MouseClickEvent
        {
            internal RectTransform cardFXRect;
            internal Vector2 targetCardPos;

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
                return (cardFXRect.anchoredPosition - targetCardPos).magnitude < 0.01f;
            }
        }
    }
}