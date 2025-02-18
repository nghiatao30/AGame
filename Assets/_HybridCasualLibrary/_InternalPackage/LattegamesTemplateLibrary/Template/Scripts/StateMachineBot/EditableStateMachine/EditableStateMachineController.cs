using System.Collections.Generic;
using UnityEngine;


namespace LatteGames.EditableStateMachine
{
    using LatteGames.StateMachine;

    public class EditableStateMachineController : MonoBehaviour
    {
        [SerializeField] protected List<StateSO> states;

        protected bool isRunning = false;
        protected StateMachine.Controller stateMachineController = new StateMachine.Controller();

        protected virtual void Awake()
        {
            foreach (var state in states)
            {
                state.SetupState();
            }
        }

        public virtual void StartStateMachine()
        {
            if (states.Count <= 0) return;
            isRunning = true;
            stateMachineController.StateChanged(states[0].State);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (stateMachineController == null) return;
            if (isRunning == false) return;
            stateMachineController.Update();
        }

        public virtual void StopStateMachine()
        {
            isRunning = false;
        }
    }
}