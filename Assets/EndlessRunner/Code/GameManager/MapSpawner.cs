using A2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A2
{
    //Spawns the map using the pool manager script  
    public class MapSpawner : MonoBehaviour
    {
        //Gets the position where the map should spawn
        public Transform posSpawn;
        public bool DEBUG;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //If the map enters the trigger the GO is retrieved from the pool
            if (collision.CompareTag("SpawnerMap"))
            {
                if (DEBUG)
                {
                    Debug.Log("MAP SPAWNER: Collision detected");
                }

                PoolLogic.Instance.GetObject(PoolLogic.PoolType.Map, posSpawn.localPosition);
            }
            else if (collision.CompareTag("MapRequeue"))
            {
                //If the map enters the requiered collider it will return the GO to the queue
                PoolLogic.Instance.ReturnToQueue(PoolLogic.PoolType.Map, transform.parent.gameObject);
            }
        }
    }

}
