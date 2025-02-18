using System.Collections;
using System.Collections.Generic;
using LatteGames;
using LatteGames.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace LatteGames.StateMachine
{
    [System.Serializable]
    public class FollowPath : StateMachine.Behaviour
    {
        public interface IMovingActor
        {
            void SetMoveDirection(Vector3 velocity);
            Vector3 GetPosition();
        }

        private IMovingActor movingActor;

        [SerializeField] private PolyPath movingPath;
        [SerializeField] private float targetReachThreshold = 0.4f;
        [SerializeField] private NavMeshAgent navMeshAgent = null;
        [SerializeField] private bool loop = false;

        private Vector3 targetPosition;
        private NavMeshPath navMeshPath;
        public Vector3 TargetPosition => targetPosition;

        public PolyPath MovingPath { get => movingPath; }
        public NavMeshAgent NavMeshAgent { get => navMeshAgent; }

        private bool foundPath = true;
        public bool FoundPath => foundPath;
        private bool canRun = true;
        public bool CanRun => canRun;

        public void Init(IMovingActor movingActor){
            this.movingActor = movingActor;
            this.navMeshPath = new NavMeshPath();
        }
        public override void Enable()
        {
            base.Enable();
            UpdateTargetPosition();
        }
        public override void Disable()
        {
            base.Disable();
            movingActor.SetMoveDirection(Vector3.zero);
        }
        public override void Update()
        {
            base.Update();
            if(ReachCurrentTarget())
                UpdateTargetPosition();

            var realTargetPosition = targetPosition;
            foundPath = true;
            if(navMeshAgent.isOnNavMesh)
            {
                foundPath = false;
                navMeshAgent.Warp(movingActor.GetPosition());
                if(navMeshAgent.isOnNavMesh)
                {
                    if(navMeshAgent.CalculatePath(targetPosition, navMeshPath))
                    {
                        if(navMeshPath.corners.Length > 1)
                        {
                            realTargetPosition = navMeshPath.corners[1];
                        }
                        foundPath = (targetPosition - navMeshPath.corners[navMeshPath.corners.Length - 1]).magnitude < 0.01f;
                    }
                }
            }
            var movingVector =(realTargetPosition - movingActor.GetPosition()).normalized;
            var movingMag = Mathf.Clamp01((realTargetPosition - movingActor.GetPosition()).magnitude);
            canRun = movingMag > 0.1f;
            movingActor.SetMoveDirection(movingVector*movingMag);
        }

        public bool ReachCurrentTarget()
        {
            var actorP = ProjectPosition(movingActor.GetPosition());
            var crrTargetP = ProjectPosition(targetPosition);
            return (actorP - crrTargetP).magnitude < targetReachThreshold;
        }

        private Vector3 ProjectPosition(Vector3 position)
        {
            RaycastHit hit;
            if(Physics.Raycast(position, Vector2.down, out hit))
                return hit.point;
            return position;
        }

        private void UpdateTargetPosition()
        {
            var characterPosition = ProjectPosition(movingActor.GetPosition());
            var worldPositions = movingPath.GetWorldPosition().ConvertAll(p => ProjectPosition(p)); 
            //find closest point
            var closestIndex = 0;
            var firstPosition = worldPositions[closestIndex];
            var minDistance = (characterPosition - firstPosition).magnitude;
            for (int i = 1; i < movingPath.points.Count; i++)
            {
                var d = (characterPosition - worldPositions[i]).magnitude;
                if(d < minDistance)
                {
                    closestIndex = i;
                    minDistance = d;
                }
            }
            //change to next point if current is too close
            if((worldPositions[closestIndex] - characterPosition).magnitude < targetReachThreshold)
            {
                if(loop)
                    closestIndex = (closestIndex + 1)%movingPath.points.Count;
                else
                    closestIndex = Mathf.Clamp(closestIndex + 1, 0, movingPath.points.Count - 1);

            }
            targetPosition = worldPositions[closestIndex];
        }
    }
}