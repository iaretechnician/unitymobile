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
/// Manager for all upgradable scripts (Engine, Brake, Handling).
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Customization/RCCP Vehicle Upgrade Upgrade Manager")]
public class RCCP_VehicleUpgrade_UpgradeManager : RCCP_UpgradeComponent, IRCCP_UpgradeComponent {

    public RCCP_VehicleUpgrade_Engine engine;        //  Upgradable engine component.
    public RCCP_VehicleUpgrade_Brake brake;      //  Upgradable brake component.
    public RCCP_VehicleUpgrade_Handling handling;        //  Upgradable handling component.
    public RCCP_VehicleUpgrade_Speed speed;        //  Upgradable speed component.
    public RCCP_VehicleUpgrade_NOS nos;        //  Upgradable nos component.

    //  Current upgraded engine level.
    public int EngineLevel {

        get {

            if (engine != null)
                return engine.EngineLevel;

            return 0;

        }

    }

    //  Current upgraded brake level.
    public int BrakeLevel {

        get {

            if (engine != null)
                return brake.BrakeLevel;

            return 0;

        }

    }

    //  Current upgraded handling level.
    public int HandlingLevel {

        get {

            if (engine != null)
                return handling.HandlingLevel;

            return 0;

        }

    }

    //  Current upgraded speed level.
    public int SpeedLevel {

        get {

            if (speed != null)
                return speed.SpeedLevel;

            return 0;

        }

    }

    //  Current upgraded nos level.
    public int NOSLevel {

        get {

            if (nos != null)
                return nos.NosLevel;

            return 0;

        }

    }

    public override void Awake() {

        base.Awake();

        //  Getting engine, brake, handling, speed, and nos upgrade components.
        engine = GetComponentInChildren<RCCP_VehicleUpgrade_Engine>(true);
        brake = GetComponentInChildren<RCCP_VehicleUpgrade_Brake>(true);
        handling = GetComponentInChildren<RCCP_VehicleUpgrade_Handling>(true);
        speed = GetComponentInChildren<RCCP_VehicleUpgrade_Speed>(true);
        nos = GetComponentInChildren<RCCP_VehicleUpgrade_NOS>(true);

        //  Getting defalut values of the car controller.
        if (engine)
            engine.defEngine = CarController.Engine.maximumTorqueAsNM;

        if (brake)
            brake.defBrake = CarController.FrontAxle.maxBrakeTorque;

        if (handling)
            handling.defHandling = CarController.Stability.tractionHelperStrength;

        if (speed)
            speed.defSpeed = CarController.Differential.finalDriveRatio;

    }

    public void Initialize() {

        if (engine) {

            //  Setting upgraded engine torque if saved.
            engine.EngineLevel = Loadout.engineLevel;
            engine.Initialize();

        }

        if (brake) {

            //  Setting upgraded brake torque if saved.
            brake.BrakeLevel = Loadout.brakeLevel;
            brake.Initialize();

        }

        if (handling) {

            //  Setting upgraded handling strength if saved.
            handling.HandlingLevel = Loadout.handlingLevel;
            handling.Initialize();

        }

        if (speed) {

            //  Setting upgraded speed if saved.
            speed.SpeedLevel = Loadout.speedLevel;
            speed.Initialize();

        }

        if (nos) {

            //  Setting upgraded nos if saved.
            nos.NosLevel = Loadout.nosLevel;
            nos.Initialize();

        }

    }

    /// <summary>
    /// Upgrades the engine torque.
    /// </summary>
    public void UpgradeEngine() {

        //  If engine is missing, return.
        if (!engine)
            return;

        //  If level is maximum, return.
        if (EngineLevel >= 5)
            return;

        //  Upgrading.
        engine.EngineLevel++;
        engine.UpdateStats();

        //  Refreshing the loadout.
        Refresh(this);

        //  Saving the loadout.
        if (CarController.Customizer.autoSave)
            Save();

    }

    /// <summary>
    /// Upgrades the brake torque.
    /// </summary>
    public void UpgradeBrake() {

        //  If brake is missing, return.
        if (!brake)
            return;

        //  If level is maximum, return.
        if (BrakeLevel >= 5)
            return;

        //  Upgrading.
        brake.BrakeLevel++;
        brake.UpdateStats();

        //  Refreshing the loadout.
        Refresh(this);

        //  Saving the loadout.
        if (CarController.Customizer.autoSave)
            Save();

    }

    /// <summary>
    /// Upgrades the traction helper (Handling).
    /// </summary>
    public void UpgradeHandling() {

        //  If handling is missing, return.
        if (!handling)
            return;

        //  If level is maximum, return.
        if (HandlingLevel >= 5)
            return;

        //  Upgrading.
        handling.HandlingLevel++;
        handling.UpdateStats();

        //  Refreshing the loadout.
        Refresh(this);

        //  Saving the loadout.
        if (CarController.Customizer.autoSave)
            Save();

    }

    /// <summary>
    /// Upgrades the maximum speed.
    /// </summary>
    public void UpgradeSpeed() {

        //  If speed is missing, return.
        if (!speed)
            return;

        //  If level is maximum, return.
        if (SpeedLevel >= 5)
            return;

        //  Upgrading.
        speed.SpeedLevel++;
        speed.UpdateStats();

        //  Refreshing the loadout.
        Refresh(this);

        //  Saving the loadout.
        if (CarController.Customizer.autoSave)
            Save();

    }

    /// <summary>
    /// Upgrades the nos.
    /// </summary>
    public void UpgradeNOS() {

        //  If nos is missing, return.
        if (!nos)
            return;

        //  If level is maximum, return.
        if (NOSLevel > 1)
            return;

        //  Upgrading.
        nos.NosLevel++;
        nos.UpdateStats();

        //  Refreshing the loadout.
        Refresh(this);

        //  Saving the loadout.
        if (CarController.Customizer.autoSave)
            Save();

    }

    /// <summary>
    /// Upgrades the engine torque.
    /// </summary>
    public void UpgradeEngineWithoutSave(int level) {

        //  If engine is missing, return.
        if (!engine)
            return;

        //  If level is maximum, return.
        if (level >= 5)
            return;

        //  Upgrading.
        engine.EngineLevel = level;
        engine.UpdateStats();

    }

    /// <summary>
    /// Upgrades the brake torque.
    /// </summary>
    public void UpgradeBrakeWithoutSave(int level) {

        //  If brake is missing, return.
        if (!brake)
            return;

        //  If level is maximum, return.
        if (level >= 5)
            return;

        //  Upgrading.
        brake.BrakeLevel = level;
        brake.UpdateStats();

    }

    /// <summary>
    /// Upgrades the traction helper (Handling).
    /// </summary>
    public void UpgradeHandlingWithoutSave(int level) {

        //  If handling is missing, return.
        if (!handling)
            return;

        //  If level is maximum, return.
        if (level >= 5)
            return;

        //  Upgrading.
        handling.HandlingLevel = level;
        handling.UpdateStats();

    }

    /// <summary>
    /// Upgrades the speed.
    /// </summary>
    public void UpgradeSpeedWithoutSave(int level) {

        //  If handling is missing, return.
        if (!speed)
            return;

        //  If level is maximum, return.
        if (level >= 5)
            return;

        //  Upgrading.
        speed.SpeedLevel = level;
        speed.UpdateStats();

    }

    /// <summary>
    /// Upgrades the nos.
    /// </summary>
    public void UpgradeNOSWithoutSave(int level) {

        //  If handling is missing, return.
        if (!nos)
            return;

        //  If level is maximum, return.
        if (level >= 5)
            return;

        //  Upgrading.
        nos.NosLevel = level;
        nos.UpdateStats();

    }

    public void Reset() {

        if (transform.Find("Engine")) {

            engine = transform.Find("Engine").gameObject.GetComponent<RCCP_VehicleUpgrade_Engine>();

        } else {

            GameObject newEngine = new GameObject("Engine");
            newEngine.transform.SetParent(transform);
            newEngine.transform.localPosition = Vector3.zero;
            newEngine.transform.localRotation = Quaternion.identity;
            engine = newEngine.AddComponent<RCCP_VehicleUpgrade_Engine>();

        }

        if (transform.Find("Brake")) {

            brake = transform.Find("Brake").gameObject.GetComponent<RCCP_VehicleUpgrade_Brake>();

        } else {

            GameObject newBrake = new GameObject("Brake");
            newBrake.transform.SetParent(transform);
            newBrake.transform.localPosition = Vector3.zero;
            newBrake.transform.localRotation = Quaternion.identity;
            brake = newBrake.AddComponent<RCCP_VehicleUpgrade_Brake>();

        }

        if (transform.Find("Handling")) {

            handling = transform.Find("Handling").gameObject.GetComponent<RCCP_VehicleUpgrade_Handling>();

        } else {

            GameObject newHandling = new GameObject("Handling");
            newHandling.transform.SetParent(transform);
            newHandling.transform.localPosition = Vector3.zero;
            newHandling.transform.localRotation = Quaternion.identity;
            handling = newHandling.AddComponent<RCCP_VehicleUpgrade_Handling>();

        }

        if (transform.Find("Speed")) {

            speed = transform.Find("Speed").gameObject.GetComponent<RCCP_VehicleUpgrade_Speed>();

        } else {

            GameObject newHandling = new GameObject("Speed");
            newHandling.transform.SetParent(transform);
            newHandling.transform.localPosition = Vector3.zero;
            newHandling.transform.localRotation = Quaternion.identity;
            speed = newHandling.AddComponent<RCCP_VehicleUpgrade_Speed>();

        }

        if (transform.Find("Nos")) {

            nos = transform.Find("Nos").gameObject.GetComponent<RCCP_VehicleUpgrade_NOS>();

        } else {

            GameObject newHandling = new GameObject("Nos");
            newHandling.transform.SetParent(transform);
            newHandling.transform.localPosition = Vector3.zero;
            newHandling.transform.localRotation = Quaternion.identity;
            nos = newHandling.AddComponent<RCCP_VehicleUpgrade_NOS>();

        }

    }

    /// <summary>
    /// Restores the settings to default.
    /// </summary>
    public void Restore() {

        //  Getting defalut values of the car controller.
        if (engine) {

            engine.EngineLevel = 0;
            CarController.Engine.maximumTorqueAsNM = engine.defEngine;

        }

        if (brake) {

            brake.BrakeLevel = 0;
            CarController.FrontAxle.maxBrakeTorque = brake.defBrake;

        }

        if (handling) {

            handling.HandlingLevel = 0;
            CarController.Stability.tractionHelperStrength = handling.defHandling;

        }

        if (speed) {

            speed.SpeedLevel = 0;
            CarController.Differential.finalDriveRatio = speed.defSpeed;

        }

        if (nos) {

            nos.NosLevel = 0;

            if (CarController.OtherAddonsManager && CarController.OtherAddonsManager.Nos)
                CarController.OtherAddonsManager.Nos.enabled = false;

        }

    }

}
