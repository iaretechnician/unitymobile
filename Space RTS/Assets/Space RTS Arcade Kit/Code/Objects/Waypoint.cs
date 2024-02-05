﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    public bool DestroyOnArrival = false;

    private void OnTriggerEnter(Collider other)
    {
        if (DestroyOnArrival)
            GameObject.Destroy(this.gameObject);
    }
}
