using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace A2
{
    /* Controls the laser through a state machine and enums. 
     * Counts with three states: waiting, firing, and cooldown. To change between states counters are used.
     * Laser hits is managed through an array of RaycastHit2D */

    [RequireComponent(typeof(LineRenderer))]
    public class Laser : MonoBehaviour
    {
        public bool DEBUG;
        private LaserState currentState;
        private LineRenderer lineRendererLaser;
        private float playerDirection;
        private HashSet<Collider2D> hitObjectsLaser = new HashSet<Collider2D>();

        //If hitting multiple enemies, damage scales down by this factor.

        [Header("Fire properties")]
        public float fireTime = 2f;
        public float distanceToFire = 6f;
        public float damageToDeal = 2f;
        private float damage;
        public float damageDownwardMultiplier = 3f;
        public ContactFilter2D contactFilter;

        [Header("Cooldow")]
        public float cooldownTime = 5f;

        //Controls the timers that change between states. DO NOT MODIFY.
        private float counterFireTime = 0;
        private float counterCooldown = 0;
        private bool firing = false;

        //Sets the states of the laser
        public enum LaserState
        {
            Waiting,
            Fire,
            Cooldown
        }

        //Sets the laser's starting state to waiting and assigns the sprite renderer
        private void Awake()
        {
            //sets damage to have the value of damage to deal
            damage = damageToDeal;
            //sets the default state for the laser
            currentState = LaserState.Waiting;
            lineRendererLaser = GetComponent<LineRenderer>();
        }
        private void Update()
        {
            SetStates();

            //Checks the direction the player is facing, this is used to set the X the laser will be facing
            playerDirection = PlayerMovement.Instance.playerDirection.x >= 0 ? 1 : -1;

            //Debugs the laser direction and range through a DrawRay
            if (DEBUG)
            {
                Debug.DrawRay(transform.position, new Vector2(playerDirection * distanceToFire, 0), Color.green);
            }
        }

        //Uses fixupdate to calculate the laser firing
        private void FixedUpdate()
        {
            if (firing && currentState == LaserState.Fire)
            {
                LaserFire();
            }
        }

        //Sets and switches between the possible states of the laser
        public void SetStates()
        {
            //Used to check the laser's state, BUT will print each second so only use it to find bugs
            if (DEBUG)
            {
                //Debug.Log(currentState);
            }

            switch (currentState)
            {
                case LaserState.Waiting:
                    LaserReady();
                    break;

                case LaserState.Fire:
                    LaserFiringUpdate();
                    break;

                case LaserState.Cooldown:
                    LaserCooldown();
                    break;
            }

        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------ LASER: WAITING

        //Listens for the input of middle mouse button, if recieved changes the currentstate to Fire
        void LaserReady()
        {
            if (Input.GetMouseButtonDown(2))
            {
                counterFireTime = 0;
                currentState = LaserState.Fire;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------ LASER: FIRE

        //This is set different from UpdateCounters() as the laser need to be divided in update and fixed update as it uses physics
        private void LaserFiringUpdate()
        {
            //Checks the state of the laser
            if (currentState == LaserState.Fire)
            {
                //If laser is firing, will start to count and set bool "firing" to true
                counterFireTime += Time.deltaTime;
                firing = true;

                //If counter has passed the fire time, state machine passes to cooldown state and firing bool is set to false
                //This will make the laser respect the expected time for firing case
                if (counterFireTime > fireTime)
                {
                    firing = false;
                    counterCooldown = 0;
                    currentState = LaserState.Cooldown;
                }
            }
        }

        private void LaserFire()
        {
            //Gets the direction the player is facing (using the input)
            Vector2 playerDirAndDistance = new Vector2(playerDirection * distanceToFire, 0);

            //If "firing" is true will enter the state, bool "firing" is set in UpdateCounters()
            if (firing)
            {
                //Creates a RaycastHit2D array which will save the objects with which it collides
                RaycastHit2D[] results = new RaycastHit2D[5];
                int hits = Physics2D.Raycast(transform.position, playerDirAndDistance, contactFilter, results, distanceToFire);


                //Goes through the hits array, and in case they aren't null calls the method LaserDealDamage on each GO that was hit
                for (int i = 0; i < hits; i++)
                {
                    if (results[i].collider == null)
                    {
                        Debug.Log("LASER: RaycastHit is null, no objects were hit");
                    }
                    else
                    {
                        //Changes the damage so the first object will recieve the highest damage and from there it will go down according to the multiplier
                        damage += damageDownwardMultiplier;

                        if (DEBUG)
                        {
                            Debug.Log("LASER: Hit object, dealing damage: " + results[i].collider.gameObject.name);
                        }

                        //Calls damage and spawns the HitFX in each component of the array
                        LaserDealDamage(results[i].collider);

                        if (!hitObjectsLaser.Contains(results[i].collider))
                        {
                            hitObjectsLaser.Add(results[i].collider);
                            StartCoroutine(HitFx(results[i].collider));
                        }

                    }
                }
                //Sets the art if the line renderer
                SetLaserArt(results);
            }
        }

        //Gets the Health script in the gameobject and deal damage to it
        void LaserDealDamage(Collider2D hit)
        {
            //Checks if a gameobject was reached with the raycast hit
            if (hit != null)
            {
                //Gets the health script in the gameobject
                Health health = hit.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    //Deals damage to the health script
                    health.TakeDamage(damage);
                }

                if (health == null)
                {
                    Debug.Log("LASER: Object does not have a health script " + hit.gameObject.name);
                }

                if (DEBUG)
                {
                    Debug.Log("LASER: (LaserDealDamage) -> Object " + hit.gameObject.name + "was hit, will take damage");
                }
            }
        }

        //Sets the line renderer paramenters: color, width, vertices
        public void SetLaserArt(RaycastHit2D[] results)
        {
            Vector2 playerDirAndDistance = new Vector2(playerDirection * distanceToFire, 0);
            //Gets the direction the player is facing (using the input)
            //Vector2 playerDirection = PlayerMovement.Instance.GetPlayerDirection();
            lineRendererLaser.startColor = Color.blue;
            lineRendererLaser.endColor = Color.red;

            //Lerps the color in relation to the active time of the laser.
            Color currentColor = Color.Lerp(lineRendererLaser.startColor, lineRendererLaser.endColor, counterFireTime);
            lineRendererLaser.startColor = currentColor;
            lineRendererLaser.endColor = currentColor;

            //Sets the width of the laser (line renderer)
            lineRendererLaser.startWidth = 0.1f;
            lineRendererLaser.endWidth = 0.1f;

            // Set the number of vertices
            lineRendererLaser.positionCount = 2;


            // Set the positions of the vertices
            lineRendererLaser.SetPosition(0, transform.position);
            lineRendererLaser.SetPosition(1, transform.position + (Vector3)playerDirAndDistance);

        }

        //Will spawn the HitFx prefab and return it to the queue after .2 seconds. MAGIC NUMBER HERE!
        private IEnumerator HitFx(Collider2D hit)
        {
            var hitFXSpawned = PoolLogic.Instance.GetObject(PoolLogic.PoolType.HitFx, hit.transform.position);
            yield return new WaitForSeconds(.2f);
            PoolLogic.Instance.ReturnToQueue(PoolLogic.PoolType.HitFx, hitFXSpawned);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------ LASER: COOLDOWN

        private void LaserCooldown()
        {
            //Restarts the damage to eliminate the multiplier value
            damageToDeal = damage;
            //Clears the hashset of hit fx
            hitObjectsLaser.Clear();

            DeactivateLineRenderer();
            if (currentState == LaserState.Cooldown)
            {
                //If laser is coolingdown, will start to count and set bool "coolingdown" to true
                counterCooldown += Time.deltaTime;

                //If counter has passed the cooldown time, state machine passes to waiting state and bool "coolingdown" is set to false
                //This will make the laser respect the expected time for cooling case
                if (counterCooldown > cooldownTime)
                {
                    currentState = LaserState.Waiting;
                }

            }

        }

        //Changes the color of the laser so it "turns off" in the game view
        public void DeactivateLineRenderer()
        {
            Color seeThrough = new Color(1, 1, 1, 0);
            lineRendererLaser.startColor = seeThrough;
            lineRendererLaser.endColor = seeThrough;
        }
    }
}
