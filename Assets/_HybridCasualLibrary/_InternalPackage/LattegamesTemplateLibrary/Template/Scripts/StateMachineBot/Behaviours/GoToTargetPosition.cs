using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames.StateMachine;
using LatteGames;
using UnityEngine.AI;

[System.Serializable]
public class GoToTargetPosition : StateMachine.Behaviour
{
    public interface IMoveActor
    {
        void SetMoveDirection(Vector3 zero);
        Vector3 GetPosition();
    }

    [SerializeField] private InternalLogger logger;

    private IMoveActor actor;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private NavMeshAgent navMeshAgent = null;
    [SerializeField] private float reachThreshold = 0.4f;

    private NavMeshPath navMeshPath;
    public Vector3 TargetPosition {get=> GetTargetPosition();}

    private Vector3 GetTargetPosition()
    {
        RaycastHit hitDownInfo;
        if(Physics.Raycast(targetPosition.position, Vector3.down, out hitDownInfo))
            return hitDownInfo.point;
        return targetPosition.position;
    }

    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }
    public bool ReachTargetPosition {get; private set;}
    public bool CanRun {get; private set;}
    public bool PathFound {get; private set;}

    public void Init(IMoveActor actor){
        this.navMeshPath = new NavMeshPath();
        this.actor = actor;
    }
    public override void Enable()
    {
        base.Enable();
    }
    public override void Disable()
    {
        base.Disable();
        actor.SetMoveDirection(Vector3.zero);
    }
    public override void Update()
    {
        base.Update();
        var realTargetPosition = TargetPosition;
        PathFound = true;
        if(navMeshAgent.isOnNavMesh)
        {
            PathFound = false;
            navMeshAgent.Warp(actor.GetPosition());
            if(navMeshAgent.isOnNavMesh)
            {
                if(navMeshAgent.CalculatePath(TargetPosition, navMeshPath))
                {
                    if(navMeshPath.corners.Length > 1)
                    {
                        realTargetPosition = navMeshPath.corners[1];
                        var endPointOnNav = navMeshPath.corners[navMeshPath.corners.Length - 1];
                        RaycastHit endPointOut;
                        if(Physics.Raycast(endPointOnNav + Vector3.up*0.1f, Vector3.down, out endPointOut))
                            PathFound = (TargetPosition - endPointOut.point).magnitude < 0.01f;
                    }
                }
            }
        }
        var movingVector =(realTargetPosition - actor.GetPosition()).normalized;
        var movingMag = Mathf.Clamp01((realTargetPosition - actor.GetPosition()).magnitude);
        actor.SetMoveDirection(movingVector*movingMag);
        var projectedActorPosition = ProjectPosition(actor.GetPosition());
        var projectedTargetPosition = ProjectPosition(targetPosition.position);
        ReachTargetPosition = (projectedActorPosition - projectedTargetPosition).magnitude < reachThreshold;
        CanRun = movingMag > 0.1f;

    }

    private Vector3 ProjectPosition(Vector3 pos)
    {
        RaycastHit hit;
        if(Physics.Raycast(pos, Vector3.down, out hit))
            return hit.point;
        return pos;
    }

    public bool IsClearPathFound()
    {
        if(!navMeshAgent.enabled) return false;
        var found = true;
        if(navMeshAgent.isOnNavMesh)
        {
            found = false;
            navMeshAgent.Warp(actor.GetPosition());
            if(navMeshAgent.isOnNavMesh)
            {
                if(navMeshAgent.CalculatePath(TargetPosition, navMeshPath))
                {
                    var endPointOnNav = navMeshPath.corners[navMeshPath.corners.Length - 1];
                    RaycastHit endPointOut;
                    if(Physics.Raycast(endPointOnNav + Vector3.up*0.1f, Vector3.down, out endPointOut))
                        found = (TargetPosition - endPointOut.point).magnitude < 0.01f;
                }
            }
        }
        return found;
    }

    public Vector3 GetNavMeshEndPoint()
    {
        if(navMeshAgent == null)
            return Vector3.zero;
        if(navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.Warp(actor.GetPosition());
            if(navMeshAgent.isOnNavMesh)
            {
                if(navMeshAgent.CalculatePath(TargetPosition, navMeshPath))
                {
                    return navMeshPath.corners[navMeshPath.corners.Length - 1];
                }
            }
        }
        return actor?.GetPosition()??Vector3.zero;
    }
}
