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
/// Upgrades nos state of the car controller.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Customization/RCCP Vehicle Upgrade NOS")]
public class RCCP_VehicleUpgrade_NOS : RCCP_Component {

    private int _nosLevel = 0;
    public int NosLevel {
        get {
            return _nosLevel;
        }
        set {
            if (value <= 1)
                _nosLevel = value;
        }
    }

    /// <summary>
    /// Updates nos and initializes it.
    /// </summary>
    public void Initialize() {

        if (!CarController.OtherAddonsManager || !CarController.OtherAddonsManager.Nos) {

            Debug.LogError("NOS couldn't found in your vehicle. RCCP_VehicleUpgrade_NOS needs it to upgrade the nos level.");
            enabled = false;
            return;

        }

        CarController.OtherAddonsManager.Nos.enabled = NosLevel == 1 ? true : false;

    }

    /// <summary>
    /// Updates nos and save it.
    /// </summary>
    public void UpdateStats() {

        if (!CarController.OtherAddonsManager || !CarController.OtherAddonsManager.Nos) {

            Debug.LogError("NOS couldn't found in your vehicle. RCCP_VehicleUpgrade_NOS needs it to upgrade the nos level.");
            enabled = false;
            return;

        }

        CarController.OtherAddonsManager.Nos.enabled = NosLevel == 1 ? true : false;

    }

    private void Reset() {

        if (GetComponentInParent<RCCP_CarController>(true).GetComponentInChildren<RCCP_OtherAddons>(true)) {

            if (!GetComponentInParent<RCCP_CarController>(true).GetComponentInChildren<RCCP_OtherAddons>(true).GetComponentInChildren<RCCP_Nos>(true)) {

                Debug.LogError("NOS couldn't found in your vehicle. RCCP_VehicleUpgrade_NOS needs it to upgrade the nos level.");
                enabled = false;
                return;

            }

        } else {

            Debug.LogError("NOS couldn't found in your vehicle. RCCP_VehicleUpgrade_NOS needs it to upgrade the nos level.");
            enabled = false;
            return;

        }

    }

}
