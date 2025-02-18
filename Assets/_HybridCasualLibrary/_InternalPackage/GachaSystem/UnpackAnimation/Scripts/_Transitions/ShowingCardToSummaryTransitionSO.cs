using System;
using UnityEngine;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    using StateMachine;

    [CreateAssetMenu(fileName = "ShowingCard - SummaryTransitionSO", menuName = "LatteGames/ScriptableObject/GachaSystem/TransitionSO/ShowingCardToSummaryTransitionSO")]
    public class ShowingCardToSummaryTransitionSO : TransitionSO
    {
        readonly ShowingCardToSummaryEvent showingCardEvent = new();

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

        protected class ShowingCardToSummaryEvent : OpenPackAnimationSM.MouseClickEvent
        {
            internal float minShowingCardTime = 1f;
            internal AbstractCardController cardController;
            float elapsedTime = 0;
            bool nextStateEnabled => elapsedTime >= minShowingCardTime;
            protected virtual bool isMatchCondition => controller.RemainingItemAmount <= 0;

            public override void Enable()
            {
                elapsedTime = 0;
                base.Enable();
            }

            public override void Update()
            {
                base.Update();
                if (nextStateEnabled) return;
                elapsedTime += Time.deltaTime;
            }

            protected override void HandleMouseClicked()
            {
                if (nextStateEnabled == false) return;
                if (cardController.IsAnimationEnded == false) return;
                if (isMatchCondition == false) return;
                base.HandleMouseClicked();
            }
        }
    }
}