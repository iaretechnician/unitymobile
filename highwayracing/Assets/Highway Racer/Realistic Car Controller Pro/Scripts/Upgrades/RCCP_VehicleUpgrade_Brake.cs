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
/// Upgrades brake torque of the car controller.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Customization/RCCP Vehicle Upgrade Brake")]
public class RCCP_VehicleUpgrade_Brake : RCCP_Component {

    private int _brakeLevel = 0;

    /// <summary>
    /// Current brake level. Maximum is 5.
    /// </summary>
    public int BrakeLevel {
        get {
            return _brakeLevel;
        }
        set {
            if (value <= 5)
                _brakeLevel = value;
        }
    }

    /// <summary>
    /// Default brake torque of the vehicle.
    /// </summary>
    [HideInInspector] public float defBrake = 0f;

    /// <summary>
    /// Maximum upgradable brake torque of the vehicle.
    /// </summary>
    [Range(-1f, 10000f)] public float maxUpgradedBrakeTorque = -1f;

    /// <summary>
    /// Updates brake torque and initializes it.
    /// </summary>
    public void Initialize() {

        if (CarController.AxleManager.Axles == null || CarController.AxleManager.Axles != null && CarController.AxleManager.Axles.Count < 1) {

            Debug.LogError("Axles couldn't found in your vehicle. RCCP_VehicleUpgrade_Brake needs it to upgrade the brake level.");
            enabled = false;
            return;

        }

        if (maxUpgradedBrakeTorque <= 0f)
            CalculateInitialValues();

        for (int i = 0; i < CarController.AxleManager.Axles.Count; i++)
            CarController.AxleManager.Axles[i].maxBrakeTorque = Mathf.Lerp(defBrake, maxUpgradedBrakeTorque, BrakeLevel / 5f);

    }

    /// <summary>
    /// Updates brake torque and save it.
    /// </summary>
    public void UpdateStats() {

        if (CarController.AxleManager.Axles == null || CarController.AxleManager.Axles != null && CarController.AxleManager.Axles.Count < 1) {

            Debug.LogError("Axles couldn't found in your vehicle. RCCP_VehicleUpgrade_Brake needs it to upgrade the brake level.");
            enabled = false;
            return;

        }

        if (maxUpgradedBrakeTorque <= 0f)
            CalculateInitialValues();

        for (int i = 0; i < CarController.AxleManager.Axles.Count; i++)
            CarController.AxleManager.Axles[i].maxBrakeTorque = Mathf.Lerp(defBrake, maxUpgradedBrakeTorque, BrakeLevel / 5f);

    }

    private void Update() {

        if (maxUpgradedBrakeTorque <= 0f)
            CalculateInitialValues();

        //  Make sure max brake is not smaller.
        if (maxUpgradedBrakeTorque < 0)
            maxUpgradedBrakeTorque = 0;

    }

    private void Reset() {

        CalculateInitialValues();

    }

    private void CalculateInitialValues() {

        if (!GetComponentInParent<RCCP_CarController>(true).GetComponentInChildren<RCCP_Axles>(true)) {

            Debug.LogError("Axles couldn't found in your vehicle. RCCP_VehicleUpgrade_Brake needs it to upgrade the brake level.");
            enabled = false;
            return;

        }

        float averageBrakeforce = 0f;

        for (int i = 0; i < GetComponentInParent<RCCP_CarController>(true).GetComponentInChildren<RCCP_Axles>(true).Axles.Count; i++)
            averageBrakeforce += GetComponentInParent<RCCP_CarController>(true).GetComponentInChildren<RCCP_Axles>(true).Axles[i].maxBrakeTorque;

        averageBrakeforce /= GetComponentInParent<RCCP_CarController>(true).GetComponentInChildren<RCCP_Axles>(true).Axles.Count;

        maxUpgradedBrakeTorque = averageBrakeforce + 800f;

    }

}
