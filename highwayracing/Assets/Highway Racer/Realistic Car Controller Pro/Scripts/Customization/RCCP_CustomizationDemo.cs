//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Customization demo used in the demo scene. Enables disables cameras and canvases.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Customization/RCCP Customization Demo")]
public class RCCP_CustomizationDemo : MonoBehaviour {

    private static RCCP_CustomizationDemo instance;
    public static RCCP_CustomizationDemo Instance {

        get {

            if (instance == null)
                instance = FindObjectOfType<RCCP_CustomizationDemo>();

            return instance;

        }

    }

    public RCCP_CarController vehicle;
    public RCCP_ShowroomCamera showroomCamera;
    public RCCP_Camera RCCCamera;
    public GameObject modificationCanvas;
    public Transform location;

    public void EnableCustomization(RCCP_CarController carController) {

        vehicle = carController;

        if (RCCCamera)
            RCCCamera.gameObject.SetActive(false);

        if (showroomCamera)
            showroomCamera.gameObject.SetActive(true);

        if (modificationCanvas)
            modificationCanvas.SetActive(true);

        if (location)
            RCCP.Transport(vehicle, location.position, location.rotation);

        RCCP.SetControl(vehicle, false);

    }

    public void DisableCustomization() {

        if (RCCCamera)
            RCCCamera.gameObject.SetActive(true);

        if (showroomCamera)
            showroomCamera.gameObject.SetActive(false);

        if (modificationCanvas)
            modificationCanvas.SetActive(false);

        if (vehicle)
            RCCP.SetControl(vehicle, true);

        vehicle = null;

    }

}
