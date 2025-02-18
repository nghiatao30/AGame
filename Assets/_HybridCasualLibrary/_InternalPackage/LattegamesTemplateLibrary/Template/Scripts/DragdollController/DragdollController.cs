using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames{
    public class DragdollController : MonoBehaviour
    {
        [SerializeField] private Rigidbody pelvis = null;
        [SerializeField] private Rigidbody lHip = null;
        [SerializeField] private Rigidbody lKnee = null;
        [SerializeField] private Rigidbody rHip = null;
        [SerializeField] private Rigidbody rKnee = null;
        [SerializeField] private Rigidbody lArm = null;
        [SerializeField] private Rigidbody lElbow = null;
        [SerializeField] private Rigidbody rArm = null;
        [SerializeField] private Rigidbody rElbow = null;
        [SerializeField] private Rigidbody mSpine = null;
        [SerializeField] private Rigidbody head = null;

        public Rigidbody RootRb => pelvis;

        private Rigidbody[] rigidbodies => new Rigidbody[]{
            pelvis,
            lHip,
            lKnee,
            rHip,
            rKnee,
            lArm,
            lElbow,
            rArm,
            rElbow,
            mSpine,
            head,
        };

        public void AddForce(Vector3 force, Vector3 position, float falloff, ForceMode mode)
        {
            foreach (var rb in rigidbodies)
                rb.AddForceAtPosition(force * Mathf.Clamp01(1 - (rb.position - position).magnitude/falloff), position, mode);
        }

        public void DestroyAllRigidbody()
        {
            foreach (var rb in rigidbodies)
            {
                Joint joint = rb.GetComponent<Joint>();
                DestroyImmediate(joint);
                DestroyImmediate(rb);
            }
        }

        private void OnEnable() {
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false;
                rb.GetComponent<Collider>().enabled = true;
            }
        }

        private void OnDisable() {
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = true;
                rb.GetComponent<Collider>().enabled = false;
            }
        }

        private void OnValidate() {
            try
            {
                if (enabled)
                    OnEnable();
                else
                    OnDisable();
            }
            catch
            {

            }
        }

        [ContextMenu("Auto Detect")]
        private void AutoDetect()
        {
            var animator = GetComponentInChildren<Animator>();
            pelvis = animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
            lHip = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).GetComponent<Rigidbody>();
            lKnee = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).GetComponent<Rigidbody>();
            rHip = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).GetComponent<Rigidbody>();
            rKnee = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).GetComponent<Rigidbody>();
            lArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).GetComponent<Rigidbody>();
            lElbow = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).GetComponent<Rigidbody>();
            rArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm).GetComponent<Rigidbody>();
            rElbow = animator.GetBoneTransform(HumanBodyBones.RightLowerArm).GetComponent<Rigidbody>();
            mSpine = animator.GetBoneTransform(HumanBodyBones.Chest).GetComponent<Rigidbody>();
            head = animator.GetBoneTransform(HumanBodyBones.Head).GetComponent<Rigidbody>();
        }

        [ContextMenu("Clear Ragdoll")]
        private void ClearRagdoll()
        {
            foreach(var rgbody in rigidbodies)
            {
                DestroyImmediate(rgbody.GetComponent<Collider>());
                var joint = rgbody.GetComponent<CharacterJoint>();
                if (joint != null) DestroyImmediate(joint);
                DestroyImmediate(rgbody);
            }
        }
    }
}