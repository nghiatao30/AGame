using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Template
{
    public class JoystickCharacterControl : MonoBehaviour
    {
        [SerializeField] private VirtualJoyStickSharedData input = null;
        [SerializeField] private ForceBasedCharacterController characterController = null;
        [SerializeField] private DragdollController dragdollController = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private string movingBlendKey = "MovingBlendKey";

        private void Awake() {
            characterController.enabled = true;
            dragdollController.enabled = false;   
            animator.enabled = true;
        }

        private void FixedUpdate() {
            characterController.SetMoveDirection(new Vector3(
                input.NormalizedDrag.x,
                0,
                input.NormalizedDrag.y
            ));
            animator.SetFloat(movingBlendKey, input.NormalizedDrag.magnitude);
        }
    }
}