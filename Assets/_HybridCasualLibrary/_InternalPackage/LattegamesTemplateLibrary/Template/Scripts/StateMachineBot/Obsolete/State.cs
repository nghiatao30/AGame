using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use LatteGames.StateMachine instead")]
    public class State : MonoBehaviour
    {
        public event Action<State> ChangeState = delegate {};

        public StateBehaviour Behaviour = null;
        public List<EventTriggerTransition> TriggerTransitions = new List<EventTriggerTransition>();
        
        [Serializable]
        public class EventTriggerTransition
        {
            public StateEvent Event;
            public State NewState;

            private Action<State> callback;
            public void Subscribe(Action<State> callback){
                this.callback = callback;
                this.Event.Triggered += OnTriggered;
            }

            private void OnTriggered()
            {
                this.callback?.Invoke(NewState);
            }

            public void UnSub(){
                this.Event.Triggered -= OnTriggered;
            }
        }

        public void LeaveState()
        {
            Behaviour?.OnBehaviourDisable();
            foreach (var transition in TriggerTransitions)
                transition.Event.DisableTrigger();
        }

        public void EnterState()
        {
            Behaviour?.OnBehaviourEnable();
            foreach (var transition in TriggerTransitions)
                transition.Event.EnableTrigger();
        }

        private void Awake() {
            TriggerTransitions.ForEach(transition => transition.Subscribe(OnEventCallback));
        }

        private void OnDestroy() {
            TriggerTransitions.ForEach(transition => transition.UnSub());
        }

        private void OnEventCallback(State newState)
        {
            ChangeState(newState);
        }
    }
}