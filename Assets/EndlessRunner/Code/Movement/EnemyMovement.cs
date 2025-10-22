using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace A2
{
    //Controls the movement of the enemy and returns the GO to the pool queue

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyMovement : MonoBehaviour
    {
        public bool DEBUG;
        public float horizontalSpeed = 1f;
        public float destroyAfterSeconds = 5f;
        private new Rigidbody2D rigidbody;
        private SpriteRenderer spriteRenderer;

        //Gets Rigidbody assigned
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        //Debugs the direction the enemy is moving towards
        private void Update()
        {
            if (DEBUG)
            {
                Debug.DrawRay(transform.position, new Vector3(-horizontalSpeed, 0, 0) * .5f, Color.red);
            }
        }
        //Calls method that moves the enemy
        private void FixedUpdate()
        {
            MoveEnemy();
        }

        //Returns the GO to the queue after a certaing amount of time
        private void Start()
        {
            StartCoroutine(ReturnToQueueTime());
        }

        //Translates the enemy position
        private void MoveEnemy()
        {

            transform.Translate(-horizontalSpeed * Time.deltaTime, 0, 0);

        }

        //Returns the GO the the queue after certain time
        IEnumerator ReturnToQueueTime()
        {
            yield return new WaitForSeconds(destroyAfterSeconds);
            PoolLogic.Instance.ReturnToQueue(PoolLogic.PoolType.Enemy, gameObject);
        }

        //When colliding will send the GO back to the queue and start the coroutine set art
        private void OnCollisionEnter2D(Collision2D collision)
        {
            StartCoroutine(Collided(collision));

        }

        //Will set art on death and get the objects back to the queue
        private IEnumerator Collided(Collision2D collision)
        {
            if (collision.gameObject.tag != "Boundries")
            {
                //Instanciates de fx 
                var deathFXSpawned = PoolLogic.Instance.GetObject(PoolLogic.PoolType.DeathFx, transform.position);
                //Sets the sprite renderer to false so the sprite is no longer visible
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(1f);
                //Returns Fx to queue
                PoolLogic.Instance.ReturnToQueue(PoolLogic.PoolType.DeathFx, deathFXSpawned);
                yield return new WaitForSeconds(.2f);
                //Returns enemy prefab to pool
                PoolLogic.Instance.ReturnToQueue(PoolLogic.PoolType.Enemy, gameObject);
            }
        }
    }
}
