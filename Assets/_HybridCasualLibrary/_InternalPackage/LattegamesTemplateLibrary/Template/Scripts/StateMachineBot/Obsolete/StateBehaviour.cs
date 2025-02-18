using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use LatteGames.StateMachine instead")]
    public abstract class StateBehaviour : MonoBehaviour
    {
        public abstract void UpdateBehaviour();
        public virtual void OnBehaviourEnable(){}
        public virtual void OnBehaviourDisable(){}
    }
}