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
/// Base class for main controller (RCCP_CarController).
/// </summary>
public class RCCP_MainComponent : MonoBehaviour {

    /// <summary>
    /// Main car controller.
    /// </summary>
    public RCCP_CarController CarController {

        get {

            return _carController;

        }
        set {

            _carController = value;

        }

    }

    #region COMPONENTS

    //  Rigidbody.
    public Rigidbody Rigid {

        get {

            if (_rigid == null)
                _rigid = GetComponent<Rigidbody>();

            return _rigid;

        }

    }

    //  Engine.
    public RCCP_Engine Engine {

        get {

            if (_engine == null)
                _engine = GetComponentInChildren<RCCP_Engine>(true);

            return _engine;

        }
        set {

            _engine = value;

        }

    }

    //  Clutch
    public RCCP_Clutch Clutch {

        get {

            if (_clutch == null)
                _clutch = GetComponentInChildren<RCCP_Clutch>(true);

            return _clutch;

        }
        set {

            _clutch = value;

        }

    }

    //  Gearbox.
    public RCCP_Gearbox Gearbox {

        get {

            if (_gearbox == null)
                _gearbox = GetComponentInChildren<RCCP_Gearbox>(true);

            return _gearbox;

        }
        set {

            _gearbox = value;

        }

    }

    //  Differential.
    public RCCP_Differential Differential {

        get {

            if (_differential == null)
                _differential = GetComponentInChildren<RCCP_Differential>(true);

            return _differential;

        }
        set {

            _differential = value;

        }

    }

    //  Axles.
    public RCCP_Axles AxleManager {

        get {

            if (_axles == null)
                _axles = GetComponentInChildren<RCCP_Axles>(true);

            return _axles;

        }
        set {

            _axles = value;

        }

    }

    //  Front axle.
    public RCCP_Axle FrontAxle {

        get {

            if (AxleManager == null)
                return null;

            if (AxleManager.Axles == null)
                return null;

            if (AxleManager.Axles.Count < 2)
                return null;

            float[] indexes = new float[AxleManager.Axles.Count];

            for (int i = 0; i < AxleManager.Axles.Count; i++)
                indexes[i] = AxleManager.Axles[i].leftWheelCollider.transform.localPosition.z;

            int biggestIndex = 0;
            int lowestIndex = 0;

            for (int i = 0; i < indexes.Length; i++) {

                if (indexes[i] >= biggestIndex)
                    biggestIndex = i;

                if (indexes[i] <= lowestIndex)
                    lowestIndex = i;

            }

            _axleFront = AxleManager.Axles[biggestIndex];

            return _axleFront;

        }

    }

    //  Rear axle.
    public RCCP_Axle RearAxle {

        get {

            if (AxleManager == null)
                return null;

            if (AxleManager.Axles == null)
                return null;

            if (AxleManager.Axles.Count < 2)
                return null;

            float[] indexes = new float[AxleManager.Axles.Count];

            for (int i = 0; i < AxleManager.Axles.Count; i++)
                indexes[i] = AxleManager.Axles[i].leftWheelCollider.transform.localPosition.z;

            int biggestIndex = 0;
            int lowestIndex = 0;

            for (int i = 0; i < indexes.Length; i++) {

                if (indexes[i] >= biggestIndex)
                    biggestIndex = i;

                if (indexes[i] <= lowestIndex)
                    lowestIndex = i;

            }

            _axleRear = AxleManager.Axles[lowestIndex];

            return _axleRear;

        }

    }

    //  All wheelcolliders.
    public RCCP_WheelCollider[] AllWheelColliders {

        get {


            if (_allWheelColliders == null || (_allWheelColliders != null && _allWheelColliders.Length < 1))
                _allWheelColliders = GetComponentsInChildren<RCCP_WheelCollider>(true);

            return _allWheelColliders;

        }
        set {

            _allWheelColliders = value;

        }

    }

    //  Aerodynamics.
    public RCCP_AeroDynamics AeroDynamics {

        get {

            if (_aero == null)
                _aero = GetComponentInChildren<RCCP_AeroDynamics>(true);

            return _aero;

        }
        set {

            _aero = value;

        }

    }

    //  Inputs.
    public RCCP_Input Inputs {

        get {

            if (_inputs == null)
                _inputs = GetComponentInChildren<RCCP_Input>(true);

            return _inputs;

        }
        set {

            _inputs = value;

        }

    }

    //  Audio.
    public RCCP_Audio Audio {

        get {

            if (_audio == null)
                _audio = GetComponentInChildren<RCCP_Audio>(true);

            return _audio;

        }
        set {

            _audio = value;

        }

    }

    //  Lights.
    public RCCP_Lights Lights {

        get {

            if (_lights == null)
                _lights = GetComponentInChildren<RCCP_Lights>(true);

            return _lights;

        }
        set {

            _lights = value;

        }

    }

    //  Stability.
    public RCCP_Stability Stability {

        get {

            if (_stability == null)
                _stability = GetComponentInChildren<RCCP_Stability>(true);

            return _stability;

        }
        set {

            _stability = value;

        }

    }

    //  Damage.
    public RCCP_Damage Damage {

        get {

            if (_damage == null)
                _damage = GetComponentInChildren<RCCP_Damage>(true);

            return _damage;

        }
        set {

            _damage = value;

        }

    }

    //  Particles.
    public RCCP_Particles Particles {

        get {

            if (_particles == null)
                _particles = GetComponentInChildren<RCCP_Particles>(true);

            return _particles;

        }
        set {

            _particles = value;

        }

    }

    //  Other addons.
    public RCCP_OtherAddons OtherAddonsManager {

        get {

            if (_otherAddons == null)
                _otherAddons = GetComponentInChildren<RCCP_OtherAddons>(true);

            return _otherAddons;

        }
        set {

            _otherAddons = value;

        }

    }

    //  Customizer.
    public RCCP_Customizer Customizer {

        get {

            return _customizer;

        }
        set {

            _customizer = value;

        }

    }

    //  Private fields for components.
    private RCCP_CarController _carController;
    private Rigidbody _rigid = null;
    private RCCP_Input _inputs = null;
    private RCCP_Engine _engine = null;
    private RCCP_Clutch _clutch = null;
    private RCCP_Gearbox _gearbox = null;
    private RCCP_Differential _differential = null;
    private RCCP_Axles _axles = null;
    private RCCP_Axle _axleFront = null;
    private RCCP_Axle _axleRear = null;
    private RCCP_AeroDynamics _aero = null;
    private RCCP_Audio _audio = null;
    private RCCP_Lights _lights = null;
    private RCCP_Stability _stability = null;
    private RCCP_Damage _damage = null;
    private RCCP_Particles _particles = null;
    private RCCP_OtherAddons _otherAddons = null;
    private RCCP_WheelCollider[] _allWheelColliders = null;
    private RCCP_Customizer _customizer = null;

    #endregion

#if UNITY_EDITOR
    [HideInInspector] public bool checkComponents = false;
#endif

    public virtual void Awake() {

        //  Finding and initializing all components attached to this vehicle (even if they are disabled).
        CarController = this as RCCP_CarController;

        IRCCP_Component[] components = GetComponentsInChildren<IRCCP_Component>(true);

        foreach (IRCCP_Component item in components)
            item.Initialize(CarController);

        IRCCP_UpgradeComponent[] upgradeComponents = GetComponentsInChildren<IRCCP_UpgradeComponent>(true);

        foreach (IRCCP_UpgradeComponent item in upgradeComponents)
            item.Initialize(CarController);

    }

    /// <summary>
    /// Resetting variables to default on enable / disable.
    /// </summary>
    /// <param name="carController"></param>
    public void ResetVehicle() {

        CarController.engineRPM = 0f;
        CarController.currentGear = 0;
        CarController.currentGearRatio = 1f;
        CarController.lastGearRatio = 1f;
        CarController.differentialRatio = 1f;
        CarController.speed = 0f;
        CarController.wheelRPM2Speed = 0f;
        CarController.tractionWheelRPM2EngineRPM = 0f;
        CarController.targetWheelSpeedForCurrentGear = 0f;
        CarController.maximumSpeed = 0f;
        CarController.producedEngineTorque = 0f;
        CarController.producedGearboxTorque = 0f;
        CarController.producedDifferentialTorque = 0f;
        CarController.direction = 1;
        CarController.engineStarting = false;
        CarController.engineRunning = false;
        CarController.shiftingNow = false;
        CarController.reversingNow = false;
        CarController.steerAngle = 0f;
        CarController.fuelInput_V = 0f;
        CarController.throttleInput_V = 0f;
        CarController.brakeInput_V = 0f;
        CarController.steerInput_V = 0f;
        CarController.handbrakeInput_V = 0f;
        CarController.clutchInput_V = 0f;
        CarController.gearInput_V = 0f;
        CarController.nosInput_V = 0f;
        CarController.throttleInput_P = 0f;
        CarController.brakeInput_P = 0f;
        CarController.steerInput_P = 0f;
        CarController.handbrakeInput_P = 0f;
        CarController.clutchInput_P = 0f;
        CarController.nosInput_P = 0f;
        CarController.lowBeamLights = false;
        CarController.highBeamLights = false;
        CarController.indicatorsLeftLights = false;
        CarController.indicatorsRightLights = false;
        CarController.indicatorsAllLights = false;

    }

}
