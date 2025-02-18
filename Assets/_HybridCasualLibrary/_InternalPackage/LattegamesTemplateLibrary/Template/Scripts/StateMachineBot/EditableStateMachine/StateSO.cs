using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace LatteGames.EditableStateMachine
{
    using LatteGames.StateMachine;

    public abstract class StateSO : SerializableScriptableObject
    {
        [SerializeField] protected List<TransitionSO> transitions;
        protected StateMachine.State state;
        public virtual StateMachine.State State
        {
            get
            {
                if (state == null)
                {
                    state = new StateMachine.State(new StateMachine.ActionBehaviour(
                        onEnable: StateEnable,
                        onDisable: StateDisable,
                        onUpdate: StateUpdate));
                    state.Transitions = transitions.Select(transition => transition.Transition).ToList();
                }
                return state;
            }
        }

        /// <summary>
        /// Called before StateEnable
        /// </summary>
        /// <param name="parameters">Setup parameters</param>
        public abstract void SetupState(object[] parameters = null);
        protected abstract void StateEnable();
        protected abstract void StateUpdate();
        protected abstract void StateDisable();
    }
}