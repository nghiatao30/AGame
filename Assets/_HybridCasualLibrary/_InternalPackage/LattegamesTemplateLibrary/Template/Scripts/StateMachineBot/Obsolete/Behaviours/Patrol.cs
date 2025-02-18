using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LatteGames.StateMachineBehaviour
{
    [ObsoleteAttribute("this is obsolete. Please use LatteGames.StateMachine instead")]
    public class Patrol : StateBehaviour
    {
        [SerializeField] private PolyPath patrolRoute = null;
        [SerializeField] private float reachCheckPointThreshold = 1.0f;
        [SerializeField] private NavMeshAgent navMeshAgent = null;
        [SerializeField] private ForceBasedCharacterController characterController = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private string MovingBlendKey = "MovingBlendKey";
        [SerializeField] public float PatrolSpeed = 0.2f;

        public Vector3 TargetPosition;
        private bool reachTarget = false;
        private NavMeshPath navMeshPath;

        private void Awake() {
            TargetPosition = transform.position;
            navMeshPath = new NavMeshPath();
        }

        public override void UpdateBehaviour()
        {
            UpdateTargetPoint();
            UpdateMoving();
        }

        private void UpdateMoving()
        {
            var currentPos = transform.position;
            var movingVector = Vector3.zero;
            if(navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.Warp(currentPos);
                var found = navMeshAgent.CalculatePath(TargetPosition, navMeshPath);
                if(found && navMeshPath.corners.Length > 1)
                    movingVector = (navMeshPath.corners[1] - currentPos).normalized * PatrolSpeed;
            }
            else
            {
                movingVector = (TargetPosition - currentPos).normalized * PatrolSpeed;
            }
            characterController.SetMoveDirection(movingVector);
            animator.SetFloat(MovingBlendKey, movingVector.magnitude);
        }

        private void UpdateTargetPoint()
        {
            var currentPos = transform.position;
            reachTarget = (TargetPosition - currentPos).magnitude <= reachCheckPointThreshold;
            if(reachTarget == false)
                return;
            var worldPos = patrolRoute.GetWorldPosition();
            var index = ClosestPointIndex(worldPos);
            if(index == -1)
                return;
            var closestPoint = worldPos[index];
            if((closestPoint - currentPos).magnitude > reachCheckPointThreshold)
            {
                TargetPosition = closestPoint;
            }
            else
            {
                for (int i = 1; i <= worldPos.Count; i++)
                {
                    closestPoint = worldPos[(index + i)%worldPos.Count];
                    if((closestPoint - currentPos).magnitude > reachCheckPointThreshold)
                        break;
                }
                TargetPosition = closestPoint;
            }
        }

        private int ClosestPointIndex(List<Vector3> wordPositions)
        {
            if(wordPositions.Count == 0)
                return -1;
            var currentPos = transform.position;
            int closestPoint = 0;
            float minDis = (wordPositions[closestPoint] - currentPos).magnitude;
            for (int i = 1; i < wordPositions.Count; i++)
            {
                float d = (wordPositions[i] - currentPos).magnitude;
                if(d < minDis)
                {
                    closestPoint = i;
                    minDis = d;
                }
            }
            return closestPoint;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(TargetPosition, 0.5f);
        }
    }
}