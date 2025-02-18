using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use LatteGames.StateMachine instead")]
    public class EnterTrigger : StateEvent
    {
        [SerializeField] private TriggerEvent trigger = null;
        [SerializeField] private string tagCompare = "";

        private void Awake() {
            trigger.OnTriggerEnteredEvent += TriggerEntered;
        }

        private void OnDestroy() {
            trigger.OnTriggerEnteredEvent -= TriggerEntered;
        }

        private void TriggerEntered(Collider obj)
        {
            if(!string.IsNullOrEmpty(tagCompare))
            {
                if(tagCompare != obj.tag)
                    return;
            }
            Trigger();
        }
    }
}