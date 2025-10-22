using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace A2
{
    /*Controls de player animations using the input and a reference to get the velocity of the player's rigidbody. 
     * This is passed to the animator that uses a blend tree to set the corresponding clip animation */

    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : MonoBehaviour
    {
        private Vector2 moveInput;
        private Rigidbody2D rigid;
        private Animator animator;

        private void Start()
        {
            //Gets the components from the GO

            rigid = GetComponentInParent<Rigidbody2D>();
            if (rigid == null)
            {
                Debug.LogWarning("PLAYER ANIMATION: No rigidbody was found on player asset");
            }

            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("PLAYER ANIMATION: No animator was found on player asset");
            }
        }
        void Update()
        {
            UpdateAnimation();
        }

        //Detects the input in horizontal axis and passes it to the animatior float. 
        //References the player's rigidbody to get the velocity in y and pass the value to the animator float.
        private void UpdateAnimation()
        {
            moveInput = new Vector2(Input.GetAxis("Horizontal"), PlayerMovement.Instance.rigidBody.velocity.y);
            animator.SetFloat("xMoveInput", moveInput.x);
            animator.SetFloat("yMoveInput", PlayerMovement.Instance.rigidBody.velocity.y);

            //if input is detected, bool in the animator is set
            if (moveInput.magnitude > 0.1f)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }
}
