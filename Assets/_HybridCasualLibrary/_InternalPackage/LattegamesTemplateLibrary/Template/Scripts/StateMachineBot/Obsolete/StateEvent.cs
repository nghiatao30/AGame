using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use LatteGames.StateMachine instead")]
    public class StateEvent : MonoBehaviour
    {
        public event Action Triggered = delegate {};
        protected void Trigger(){
            Triggered();
        }
        public virtual void EnableTrigger(){}
        public virtual void DisableTrigger(){}
    }
}