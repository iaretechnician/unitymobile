//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Buðra Özdoðanlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Customization loadout.
/// </summary>
[System.Serializable]
public class RCCP_CustomizationData {

    public bool initialized = false;

    public float suspensionDistanceFront;
    public float suspensionDistanceRear;

    public float suspensionSpringForceFront;
    public float suspensionSpringForceRear;

    public float suspensionDamperFront;
    public float suspensionDamperRear;

    public float suspensionTargetFront;
    public float suspensionTargetRear;

    public float cambersFront;
    public float cambersRear;

    public float gearShiftingThreshold;
    public float clutchThreshold;

    public bool counterSteering;
    public bool steeringLimiter;

    public bool ABS;
    public bool ESP;
    public bool TCS;
    public bool SH;
    public bool NOS;
    public bool revLimiter;
    public bool automaticTransmission;

    public Color headlightColor;
    public Color wheelSmokeColor;

}
