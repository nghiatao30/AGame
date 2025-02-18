using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LatteGames.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use LatteGames.StateMachine instead")]
    public class Chasing : StateBehaviour
    {
        [SerializeField] private TriggerEvent visionTrigger = null;
        [SerializeField] private string enemyTag = "";
        [SerializeField] private NavMeshAgent navMeshAgent = null;
        [SerializeField] private ForceBasedCharacterController characterController = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private string MovingBlendKey = "MovingBlendKey";
        [SerializeField] private float ChasingSpeed = 1;

        private NavMeshPath navMeshPath;
        private Transform target;

        private void Awake() {
            navMeshPath = new NavMeshPath();
        }

        public override void OnBehaviourEnable()
        {
            base.OnBehaviourEnable();
            var entered = visionTrigger.GetEnteredObjects();
            target = entered.Find(c => c.tag == enemyTag)?.transform;
        }

        public override void UpdateBehaviour()
        {
            var currentPos = transform.position;
            var movingVector = Vector3.zero;
            if(target != null)
            {
                if(navMeshAgent.isOnNavMesh)
                {
                    navMeshAgent.Warp(currentPos);
                    var found = navMeshAgent.CalculatePath(target.position, navMeshPath);
                    if(found && navMeshPath.corners.Length > 1)
                        movingVector = (navMeshPath.corners[1] - currentPos).normalized * ChasingSpeed;
                }
                else
                {
                    movingVector = (target.position - currentPos).normalized * ChasingSpeed;
                }
            }
            characterController.SetMoveDirection(movingVector);
            animator.SetFloat(MovingBlendKey, movingVector.magnitude);
        }
    }
}