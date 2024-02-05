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
/// Upgrades maximum speed of the car controller by decreasing final drive ratio on the differential.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Customization/RCCP Vehicle Upgrade Speed")]
public class RCCP_VehicleUpgrade_Speed : RCCP_Component {

    private int _speedLevel = 0;
    public int SpeedLevel {
        get {
            return _speedLevel;
        }
        set {
            if (value <= 5)
                _speedLevel = value;
        }
    }

    [HideInInspector] public float defSpeed = 0f;

    /// <summary>
    /// Updates speed and initializes it.
    /// </summary>
    public void Initialize() {

        if (!GetComponentInParent<RCCP_CarController>(true).GetComponentInChildren<RCCP_Differential>(true)) {

            Debug.LogError("Differential couldn't found in your vehicle. RCCP_VehicleUpgrade_Speed needs it to upgrade the speed level.");
            enabled = false;
            return;

        }

        CarController.Differential.finalDriveRatio = Mathf.Lerp(defSpeed, defSpeed * .85f, SpeedLevel / 5f);

    }

    /// <summary>
    /// Updates speed and save it.
    /// </summary>
    public void UpdateStats() {

        if (!GetComponentInParent<RCCP_CarController>(true).GetComponentInChildren<RCCP_Differential>(true)) {

            Debug.LogError("Differential couldn't found in your vehicle. RCCP_VehicleUpgrade_Speed needs it to upgrade the speed level.");
            enabled = false;
            return;

        }

        CarController.Differential.finalDriveRatio = Mathf.Lerp(defSpeed, defSpeed * .85f, SpeedLevel / 5f);

    }

    private void Reset() {

        if (!GetComponentInParent<RCCP_CarController>(true).GetComponentInChildren<RCCP_Differential>(true)) {

            Debug.LogError("Differential couldn't found in your vehicle. RCCP_VehicleUpgrade_Speed needs it to upgrade the speed level.");
            enabled = false;
            return;

        }

    }

}
