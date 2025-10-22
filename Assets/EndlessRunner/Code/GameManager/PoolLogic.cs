using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A2
{
    public class PoolLogic : MonoBehaviour
    {
        //Sets the necesarry pools and dictionarys to manage the pools and GOs
        public List<Pool> poolsList;
        [HideInInspector] public Dictionary<PoolType, Queue<GameObject>> singlePool;
        public bool DEBUG = false;

        //Limits the pool type that can be choosen to reduce errors
        public enum PoolType
        {
            Enemy,
            Obstacles,
            Map,
            Health,
            DeathFx,
            HitFx,
            HealthFx
        }

        [System.Serializable] //Makes the pool settings visible in the editor
        //Sets each pool to have three parameters: type, prefab to spawn, and size of the pool
        public class Pool
        {
            public PoolType typeOfPool;
            public GameObject prefab;
            public int poolSize;
            public Transform GOParent;
        }

        //Instanciates the script
        public static PoolLogic Instance
        {
            get;
            private set;
        }

        //Sets the script to be a singleton. Calls SetupPools().
        private void Awake()
        {
            //If there is a instance it destroys it 
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            //Instanciates the deactivated objects of each the pool
            SetupPools();
        }

        //Sets the pool size and instanciates the objects in scene
        private void SetupPools()
        {
            //Starts the dictionary for each pool
            singlePool = new Dictionary<PoolType, Queue<GameObject>>();

            if (poolsList.Count != 0)
            {
                //goes through the pool list and instanciate them
                foreach (Pool pool in poolsList)
                {
                    //Populates the queue of GO
                    Queue<GameObject> poolObjectQueue = new Queue<GameObject>();

                    for (int i = 0; i < pool.poolSize; i++)
                    {
                        //Instantiate the pool as a child of the given GOParent
                        GameObject obj = Instantiate(pool.prefab, pool.GOParent);
                        obj.SetActive(false);
                        //Adds the GO to the queue
                        poolObjectQueue.Enqueue(obj);
                    }

                    //Sets key-values in the dictionary
                    singlePool.Add(pool.typeOfPool, poolObjectQueue);
                }
            }
            else
            {
                Debug.LogError("POOL LOGIC: No pools were assigned");
            }
        }

        //Dequeues the objects and sets them active in a given position
        public GameObject GetObject(PoolType poolType, Vector3 position)
        {
            if (DEBUG)
            {
                Debug.Log("POOL LOGIC: Getting object type: " + poolType);
            }

            //Creates a reference to the GO that is dequeue, sets it active and moves it to the given location. Returns the GO reference
            GameObject objectToSpawn = singlePool[poolType].Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            return objectToSpawn;
        }

        //Deactivates the instance and returns it to the corresponding queue
        public void ReturnToQueue(PoolType poolType, GameObject obj)
        {
            obj.SetActive(false);
            singlePool[poolType].Enqueue(obj);
        }
    }
}

