//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCCP_TransportOnTrigger : MonoBehaviour {

    public Transform transportToHere;
    public bool resetVelocity = true;
    public bool resetRotation = true;

    private void OnTriggerEnter(Collider other) {

        if (other.isTrigger)
            return;

        //  Getting car controller.
        RCCP_CarController carController = other.GetComponentInParent<RCCP_CarController>();

        //  If trigger is not a vehicle, return.
        if (!carController)
            return;

        if (resetRotation)
            RCCP.Transport(carController, transportToHere.position, transportToHere.rotation, resetVelocity);
        else
            RCCP.Transport(carController, transportToHere.position, carController.transform.rotation, resetVelocity);

    }

}
