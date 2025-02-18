using System;
using System.Collections.Generic;

namespace LatteGames.StateMachine
{
    public class StateMachine
    {
        public class Controller: State.IEventCallback
        {
            private State currentState;

            public State CurrentState { get => currentState; }

            public void Start(State state)
            {
                Stop();
                currentState = state;
                currentState.Start(this);
            }

            public void StateChanged(State newState)
            {
                Start(newState);
            }

            public void Update()
            {
                if(currentState != null)
                    currentState.Update();
            }

            public void Stop()
            {
                if(currentState != null)
                    currentState.Stop();
            }
        }
        public class State
        {
            public event Action<State> StateStarted = delegate {};
            public event Action<State> StateEnded = delegate {};

            private Behaviour behaviour;
            private IEventCallback eventCallback;
            private List<Transition> transitions = new List<Transition>();

            public Behaviour Behaviour { get => behaviour; set => behaviour = value; }
            public List<Transition> Transitions { get => transitions; set => transitions = value; }

            public State(Behaviour behaviour)
            {
                this.behaviour = behaviour;
            }

            public void Start(IEventCallback callback)
            {
                this.transitions.ForEach(transition => transition.EventTriggered += OnTransitionTrigger);

                this.behaviour.Enable();
                this.transitions.ForEach(transition => transition.TriggerEvent.Enable());
                this.transitions.ForEach(transition => transition.Sub());

                this.eventCallback = callback;

                StateStarted(this);
            }

            private void OnTransitionTrigger(Transition transition)
            {
                eventCallback.StateChanged(transition.TargetState);
            }

            public void Update()
            {
                this.behaviour.Update();
                this.transitions.ForEach(transition => transition.TriggerEvent.Update());
            }

            public void Stop()
            {
                this.behaviour.Disable();
                this.transitions.ForEach(transition => transition.TriggerEvent.Disable());

                this.transitions.ForEach(transition => transition.UnSub());
                this.transitions.ForEach(transition => transition.EventTriggered -= OnTransitionTrigger);
                
                eventCallback = null;

                StateEnded(this);
            }

            public interface IEventCallback
            {
                void StateChanged(State targetState);
            }

            public class Transition
            {
                public event Action<Transition> EventTriggered = delegate {};

                private Event triggerEvent;
                private State targetState;

                public Transition(Event triggerEvent, State targetState)
                {
                    this.triggerEvent = triggerEvent;
                    this.targetState = targetState;
                }

                public Event TriggerEvent { get => triggerEvent; }
                public State TargetState { get => targetState; }

                public void Sub()
                {
                    triggerEvent.Triggered += OnTriggered;
                }

                public void UnSub()
                {
                    triggerEvent.Triggered -= OnTriggered;
                }

                private void OnTriggered()
                {
                    EventTriggered(this);
                }
            }
        }
        public class Behaviour
        {
            public bool Enabled {get; private set;}
            public virtual void Enable()
            {
                Enabled = true;
            }
            public virtual void Disable()
            {
                Enabled = false;
            }
            public virtual void Update(){}
        }
        public class Event
        {
            public event Action Triggered = delegate {};
            
            public bool Enabled {get; private set;}
            public virtual void Enable()
            {
                Enabled = true;
            }
            public virtual void Disable()
            {
                Enabled = false;
            }
            public virtual void Update(){}

            protected void Trigger()
            {
                Triggered();
            }
        }
        public class PredicateEvent: Event
        {
            private Func<bool> predicate;

            public PredicateEvent(Func<bool> predicate)
            {
                this.predicate = predicate;
            }

            public override void Update()
            {
                base.Update();
                if(this.predicate())
                    Trigger();
            }
        }
        public class ManualEvent: Event
        {
            public void InvokeTrigger()
            {
                if(Enabled)
                    Trigger();
            }
        }
        public class ActionBehaviour: Behaviour
        {
            private Action onEnable;
            private Action onDisable;
            private Action onUpdate;

            public ActionBehaviour(){}

            public ActionBehaviour(
                Action onEnable, 
                Action onDisable, 
                Action onUpdate)
            {
                this.onEnable = onEnable;
                this.onDisable = onDisable;
                this.onUpdate = onUpdate;
            }

            public Action OnEnable { get => onEnable; set => onEnable = value; }
            public Action OnDisable { get => onDisable; set => onDisable = value; }
            public Action OnUpdate { get => onUpdate; set => onUpdate = value; }

            public override void Enable()
            {
                base.Enable();
                onEnable?.Invoke();
            }
            public override void Update()
            {
                base.Update();
                onUpdate?.Invoke();
            }
            public override void Disable()
            {
                base.Disable();
                onDisable?.Invoke();
            }
        }
    }
}