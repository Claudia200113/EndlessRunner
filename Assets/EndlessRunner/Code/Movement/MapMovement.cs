using A2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace A2
{
    //Translates the map according to the global speed
    public class MapMovement : MonoBehaviour
    {
        public bool DEBUG;
        private float globalSpeed;

        //Moves the method that translates the map
        void FixedUpdate()
        {
            MoveMap();
        }

        //Translates the map using the general acceleration script
        private void MoveMap()
        {
            //Gets the global speed from the accelerationController script
            globalSpeed = AccelerationController.Instance.globalSpeed;
            transform.Translate(-globalSpeed * Time.deltaTime, 0, 0);
        }


    }
}

