using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace A2
{
    /*
     * This is a script type singleton. Controls the player movement (horizontal and jumping). Checks if the player is graounded and changes gravity according to the player jumping or grounded.
     */
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerMovement : MonoBehaviour
    {
        public bool DEBUG;

        [HideInInspector]
        public      Rigidbody2D     rigidBody;
        [HideInInspector]
        public      Vector2         playerDirection;
        private     SpriteRenderer  spriteRenderer;

        [Header("Movement variables")]
        public      float           playerSpeedForward = 5f;
        public      float           playerSpeedBackwards = 1.5f;
        private     bool            playerGoingBackwards;
        private     float           normalGravity;
        private     float           fallingGravity;
        private     float           scalingGravityFactor = 1f;


        [Header("Jump variables")]
        public      float           jumpForce = 5f;
        public      LayerMask       groundLayer;
        public      float           groundDistanceToJump = 0.3f;
        [HideInInspector]
        public      bool            isGrounded;
        private     bool            isJumping;
        public      Transform       feetPosition;


        private void Awake()
        {
            //Gets the components in the GO
            rigidBody = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            //Sets the default gravity and the falling one
            normalGravity = rigidBody.gravityScale;
            fallingGravity = normalGravity + scalingGravityFactor;

            //Sets the script as a singleton
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        //Sets an instance of the script
        public static PlayerMovement Instance
        {
            get;
            private set;
        }
        void Update()
        {
            ListenForInputs();
        }

        private void FixedUpdate()
        {
            ProcessInputs();
        }

        private void ListenForInputs()
        {
            //Used to check if the player is grounded by calculating the distance to the groundlayer, used to prevent double jumping.
            isGrounded = Physics2D.OverlapCircle(feetPosition.position, groundDistanceToJump, groundLayer);
            float directionX = Input.GetAxis("Horizontal");


            //Will set a different speed depending if the player is going backwards or forward
            if (directionX < 0)
            {
                playerGoingBackwards = true;
            }
            else
            {
                playerGoingBackwards = false;
            }

            playerDirection = new Vector2(directionX, 0);

            //Checks if player is grounded so they can jump
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                isJumping = true;
            }
        }

        //Will process the player`s velocity in case it is going forward or backwards and sets the correspoding velocity 
        //to the rigidbody
        private void ProcessInputs()
        {
            //Uses the current velocity in the rigidbody
            Vector2 currentVelocity = rigidBody.velocity;

            //Used in case the player is going backwards
            if (playerGoingBackwards)
            {
                if (DEBUG)
                {
                    Debug.Log("PLAYER: Going backwards setting speed to: " + playerSpeedBackwards);
                }

                //Adds the corresponding velocity to the RB and flips the spriteRenderer
                rigidBody.velocity = new Vector2(playerDirection.x * playerSpeedBackwards, currentVelocity.y);
                spriteRenderer.flipX = true;
            }
            else //Used in case the player is going forward
            {
                if (DEBUG)
                {
                    Debug.Log("PLAYER: Going forward setting speed to: " + playerSpeedForward);
                }

                //Adds the corresponding velocity to the RB and "unflips" the spriterender
                rigidBody.velocity = new Vector2(playerDirection.x * playerSpeedForward, currentVelocity.y);
                spriteRenderer.flipX = false;
            }


            //Used in case the player is jumping
            if (isJumping)
            {
                if (DEBUG)
                {
                    Debug.Log("PLAYER: Player is jumping");
                }

                //AddForce to the rigidbody in an impulse mode so the player jumps
                rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isJumping = false;
            }

            //If the player is humping the gravity scale will go higher to make the fall feel tighter
            if (rigidBody.velocity.y < 0)
            {
                rigidBody.gravityScale = fallingGravity;
            }
            else if (rigidBody.velocity.y >= 0) //When the player is touching the ground the gravity returns to default value
            {
                rigidBody.gravityScale = normalGravity;
            }
            //Debugs a ray to check the player direction
            if (DEBUG)
            {
                Debug.DrawRay(transform.position, rigidBody.velocity, Color.red);
            }
        }

        //Debugs the circle with which isGrounded bool is calculated (distance to the ground).
        private void OnDrawGizmos()
        {
            if (DEBUG)
            {
                Gizmos.color = new Color(.165f, .824f, .871f, .5f);
                Gizmos.DrawSphere(feetPosition.position, groundDistanceToJump);
            }
        }
    }
}
