using A2;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

namespace A2
{
    //Moves the healing components in scene, in case of colliding with the player goes back to the queue

    [RequireComponent(typeof(CircleCollider2D))]
    public class Healing : MonoBehaviour
    {
        public bool DEBUG;
        public float destroyAfterSeconds = 1f;
        public float horizontalSpeed = 2f;
        public int healthGained = 5;

        private new CircleCollider2D collider;
        private SpriteRenderer spriteRenderer;
        void Start()
        {
            //Periodically returns the object to queue 
            StartCoroutine(ReturnToQueueTime());
            collider = GetComponent<CircleCollider2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        private void Update()
        {
            //Debugs the direction the heart is moving to
            if (DEBUG)
            {
                UnityEngine.Debug.DrawRay(transform.position, new Vector3(-horizontalSpeed, 0, 0) * .5f, Color.red);
            }
        }

        //Calls method that moves the asset
        private void FixedUpdate()
        {
            MoveHealth();
        }

        //Detects collisions and returns the object to the pool queue
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Adds health to the object with which the GO collided
            AddHealth(collision);
            //Sets fx
            StartCoroutine(SetVisuals(collision));
        }

        //Returns to the queue after a certain amount of time. Works with script PoolLogic()
        IEnumerator ReturnToQueueTime()
        {
            yield return new WaitForSeconds(destroyAfterSeconds);
            PoolLogic.Instance.ReturnToQueue(PoolLogic.PoolType.Health, gameObject);
        }

        //Translates the asset in the scene
        private void MoveHealth()
        {
            transform.Translate(-horizontalSpeed * Time.deltaTime, 0, 0);
        }

        //Sets the FX
        private IEnumerator SetVisuals(Collider2D collision)
        {
            //Sets the FX
            var healingFxObject = PoolLogic.Instance.GetObject(PoolLogic.PoolType.HealthFx, transform.position);
            //disables the renderer of the health, this is done so the FX can play and with a only scrip the GO are sent back to the queue, both FX and health
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.7f);
            //Return health FX to the queue
            PoolLogic.Instance.ReturnToQueue(PoolLogic.PoolType.HealthFx, healingFxObject);
            yield return new WaitForSeconds(.2f);
            //Return health to the queue
            PoolLogic.Instance.ReturnToQueue(PoolLogic.PoolType.Health, gameObject);

        }

        private void AddHealth(Collider2D collision)
        {

            //Gets the health component
            Health health = collision.gameObject.GetComponent<Health>();
            //If the object has a health script
            if (health != null)
            {
                //Adds health
                health.GainHealth(healthGained);

                if (DEBUG)
                {
                    UnityEngine.Debug.Log("HEALTH ON COLLISION: Collided, will add health to:" + collision.gameObject.name);
                }
            }
            else
            {
                if (DEBUG)
                {
                    UnityEngine.Debug.Log("HEALTH ON COLLISION: Collided but:" + collision.gameObject.name + " has no health script");
                }
            }


        }
    }
}

