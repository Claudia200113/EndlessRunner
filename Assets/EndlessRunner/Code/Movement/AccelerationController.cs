using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A2
{
    //Sets de acceleration globally for the map to move and accelerate over time
    public class AccelerationController : MonoBehaviour
    {
        [HideInInspector]
        public float globalSpeed;
        public float maxSpeed = 30;
        public float acceleration = .5f;

        //Instanciates the script
        public static AccelerationController Instance
        {
            get;
            private set;
        }

        //Sets the script to be a singleton.
        private void Awake()
        {
            //If there is an instance it destroys it 
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Update()
        {
            AccelerationSet();
        }

        //Calculates acceleration
        private void AccelerationSet()
        {
            if (globalSpeed < maxSpeed)
            {
                globalSpeed += acceleration * Time.deltaTime;
            }

        }
    }
}
