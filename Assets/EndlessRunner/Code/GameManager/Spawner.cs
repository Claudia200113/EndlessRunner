using A2;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static A2.PoolLogic;
using static A2.Spawner;

namespace A2
{
    //Used a tutorial to understand needed logic: https://youtu.be/Ah3epb2HGCw?si=S67nULCY4iM7qyxy
    /* This script controls the spamwning of pooled objects -> WORKS WITH THE POOLING SCRIPT!.
     * Has a class that takes poolType, maxTimeSpawn, minTimeSpawn, and spawn location.
     * Will go through the pools to spawn all objects and set them unactive, then will set active the objects according to the 
     * variables given in the setup
     */
    public class Spawner : MonoBehaviour
    {
        public bool DEBUG;
        public List<SpawnerSetup> spawnerSetups;

        [System.Serializable] //Makes it visible in the inspector
        public class SpawnerSetup
        {
            //This is taken from the PoolLogic script so there are no errors while setting the pools and spawning them
            public PoolLogic.PoolType poolType;
            public float minTimeSpawn;
            public float maxTimeSpawn;
            public Transform spawnLocation;
        }

        void Start()
        {
            //Checks each pool to spawn the object according to a random interval of time
            goThroughPools();
        }

        //Checks that spawnersetup is greater than 1, if so, will go through the list and spawn the items. 
        private void goThroughPools()
        {
            //Makes sure there are pools set
            if (spawnerSetups.Count == 0)
            {
                Debug.LogError("Spawners weren't set, needs fixing");
            }
            else
            {
                //Go through each pool existing and spawn the objects
                foreach (var pool in spawnerSetups)
                {
                    StartCoroutine(SpawnObjects(pool));
                }
            }
        }
        // Spawns the object by calling the pool script. First sets the location to the given one in the inspector, sets the spawning
        //time between the variables given in inspector, and finally calls the pool script to get the object from the corresponding pool.
        IEnumerator SpawnObjects(SpawnerSetup spawnerSetup)
        {
            while (true)
            {
                if (DEBUG)
                {
                    Debug.Log("SPAWNER: Instancing a new prefab type:" + spawnerSetup.poolType);
                }

                Vector3 locToSpawn = spawnerSetup.spawnLocation.localPosition;
                yield return new WaitForSeconds(Random.Range(spawnerSetup.minTimeSpawn, spawnerSetup.maxTimeSpawn));
                PoolLogic.Instance.GetObject(spawnerSetup.poolType, locToSpawn);
            }
        }
    }
}
