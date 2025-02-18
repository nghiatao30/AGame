using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use LatteGames.StateMachine instead")]
    public class WaitTrigger : StateEvent
    {
        [SerializeField] private float time = 1;
        public override void EnableTrigger()
        {
            base.EnableTrigger();
            StopAllCoroutines();
            StartCoroutine(CommonCoroutine.Delay(time, false, ()=>Trigger()));
        }
    }
}