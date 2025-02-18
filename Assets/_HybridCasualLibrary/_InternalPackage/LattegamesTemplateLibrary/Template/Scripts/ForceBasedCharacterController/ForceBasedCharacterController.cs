using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace LatteGames
{
    public class ForceBasedCharacterController : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb = null;
        [SerializeField] private float speed = 10;
        [SerializeField] private float adaptiveForceMultiplierMax = 5;
        [SerializeField] private float adaptiveForceIncreaseSpeed = 2;
        [SerializeField] private float rotationSpeed = 360;
        
        private float currentAdaptiveForceMultiplier = 1;
        private Vector3 targetXZVelocity;

        private ParentConstraint constraint;
        private void Awake() {
            rb.transform.SetParent(transform.parent);
            rb.gameObject.AddComponent<Link>().Controller = this;
            rb.gameObject.name = $"{gameObject.name}_{rb.gameObject.name}";
            constraint = gameObject.AddComponent<ParentConstraint>();
            constraint.AddSource(new ConstraintSource(){
                sourceTransform = rb.transform,
                weight = 1
            });
        }

        private void OnDestroy() {
            if(rb != null)
                Destroy(rb.gameObject);
        }

        public void SetMoveDirection(Vector3 moveVector)
        {
            targetXZVelocity = moveVector - Vector3.up*Vector3.Dot(moveVector, Vector3.up);
        }

        private void FixedUpdate() {
            var rbXZVelocity = rb.velocity - Vector3.up*Vector3.Dot(rb.velocity, Vector3.up);

            var force = (speed*targetXZVelocity - rbXZVelocity)*currentAdaptiveForceMultiplier/Time.fixedDeltaTime;

            if(OnGround())
                rb.AddForce(force);
            
            if(rbXZVelocity == speed*targetXZVelocity)
                currentAdaptiveForceMultiplier = 1;
            else
                currentAdaptiveForceMultiplier = Mathf.Clamp(Time.fixedDeltaTime*adaptiveForceIncreaseSpeed,1,adaptiveForceMultiplierMax);

            if(targetXZVelocity.magnitude > 0.01f)
                rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(targetXZVelocity), Time.fixedDeltaTime*rotationSpeed);
            rb.angularVelocity = Vector3.zero;
        }

        public bool OnGround()
        {
            return Physics.SphereCast(transform.position + transform.up*0.3f, 0.2f, -transform.up,out RaycastHit hit ,0.2f);
        }

        private void OnEnable() {
            if(constraint == null) 
                return;
            constraint.constraintActive = true;
            var cl = rb.GetComponent<Collider>();
            if(cl != null)
                cl.enabled = true;
            rb.isKinematic = false;
        }

        private void OnDisable() {
            if(constraint == null) 
                return;
            constraint.constraintActive = false;
            var cl = rb.GetComponent<Collider>();
            if(cl != null)
                cl.enabled = false;
            rb.isKinematic = true;
        }

        public class Link: MonoBehaviour
        {
            public ForceBasedCharacterController Controller;
        }

        public void SetRotation(Quaternion quaternion)
        {
            var fwd = quaternion*Vector3.forward;
            fwd = Vector3.ProjectOnPlane(fwd, Vector3.up);
            rb.rotation = Quaternion.LookRotation(fwd, Vector3.up);
        }
    }
}