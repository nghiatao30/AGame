using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use LatteGames.StateMachine instead")]
    public class Idle : StateBehaviour
    {
        [SerializeField] private ForceBasedCharacterController characterController = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private string MovingBlendKey = "MovingBlendKey";
        public override void UpdateBehaviour()
        {
            characterController.SetMoveDirection(Vector3.zero);
            animator.SetFloat(MovingBlendKey, 0);
        }
    }
}