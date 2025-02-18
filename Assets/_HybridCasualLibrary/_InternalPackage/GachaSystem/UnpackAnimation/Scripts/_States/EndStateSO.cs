using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames.EditableStateMachine;

namespace LatteGames.UnpackAnimation
{
    [CreateAssetMenu(fileName = "EndStateSO", menuName = "LatteGames/ScriptableObject/GachaSystem/StateSO/EndStateSO")]

    public class EndStateSO : StateSO
    {
        protected OpenPackAnimationSM controller;

        public override void SetupState(object[] parameters = null)
        {
            if (parameters[0] is not OpenPackAnimationSM) return;
            controller = (OpenPackAnimationSM)parameters[0];
        }

        protected override void StateDisable()
        {
            //Do nothing
        }

        protected override void StateEnable()
        {
            controller.StopStateMachine();
        }

        protected override void StateUpdate()
        {
            //Do nothing
        }
    }
}