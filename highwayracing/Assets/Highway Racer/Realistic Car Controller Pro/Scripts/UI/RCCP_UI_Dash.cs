//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used to render dash and needle.
/// </summary>
public class RCCP_UI_Dash : MonoBehaviour {

    /// <summary>
    /// Actual dashboard camera.
    /// </summary>
    private Camera dashCamera;

    /// <summary>
    /// Needle of the RPM.
    /// </summary>
    public GameObject needle;

    /// <summary>
    /// Needle rotation multiplier.
    /// </summary>
    public float needleRotationMultiplier = 1f;

    /// <summary>
    /// Start angle of the needle.
    /// </summary>
    private float startingAngle = 0f;

    private void Awake() {

        //  Getting dash camera and starting angle.
        dashCamera = GetComponentInChildren<Camera>();
        startingAngle = needle.transform.localEulerAngles.z;

    }

    private void Update() {

        //  If no dash camera found, return.
        if (!dashCamera)
            return;

        //  If no player vehicle found, return.
        if (!RCCP_SceneManager.Instance.activePlayerVehicle)
            return;

        //  Assigning rotation of the needle based on player's vehicle engine RPM.
        needle.transform.localEulerAngles = new Vector3(needle.transform.localEulerAngles.x, needle.transform.localEulerAngles.y, startingAngle + RCCP_SceneManager.Instance.activePlayerVehicle.engineRPM * -needleRotationMultiplier);

        //  Make sure z rotation of the camera is always set to 0.
        dashCamera.transform.rotation = Quaternion.Euler(dashCamera.transform.eulerAngles.x, dashCamera.transform.eulerAngles.y, 0f);

    }

}
