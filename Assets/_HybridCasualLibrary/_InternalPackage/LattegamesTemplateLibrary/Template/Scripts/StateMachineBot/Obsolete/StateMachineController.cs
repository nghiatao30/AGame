using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use LatteGames.StateMachine instead")]
    public class StateMachineController : MonoBehaviour
    {
        private State currentState = null;
        [SerializeField] private State entryState = null;

        private void Start() {
            if(entryState != null)
            {
                currentState = entryState;
                currentState.ChangeState += HandleTransitionEventTriggered;
                currentState.EnterState();
            }
        }

        private void HandleTransitionEventTriggered(State newState)
        {
            currentState.LeaveState();
            currentState.ChangeState -= HandleTransitionEventTriggered;
            currentState = newState;
            currentState.ChangeState += HandleTransitionEventTriggered;
            currentState.EnterState();
        }

        private void OnDestroy() {
            if(currentState != null)
            {
                currentState.LeaveState();
                currentState.ChangeState -= HandleTransitionEventTriggered;
            }
        }

        private void Update() {
            if(currentState == null)
                return;
            currentState.Behaviour.UpdateBehaviour();
        }
    }
}