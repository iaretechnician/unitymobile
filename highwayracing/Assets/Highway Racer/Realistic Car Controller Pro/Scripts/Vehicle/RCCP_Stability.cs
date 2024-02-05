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
/// ABS = Anti-skid braking system.
/// ESP = Detects vehicle skidding movements, and actively counteracts them.
/// TCS = Detects if a loss of traction occurs among the vehicle's wheels.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Addons/RCCP Stability")]
public class RCCP_Stability : RCCP_Component {

    /// <summary>
    /// ABS = Anti-skid braking system.
    /// </summary>
    public bool ABS = true;

    /// <summary>
    /// ESP = Detects vehicle skidding movements, and actively counteracts them.
    /// </summary>
    public bool ESP = true;

    /// <summary>
    /// TCS = Detects if a loss of traction occurs among the vehicle's wheels.
    /// </summary>
    public bool TCS = true;

    /// <summary>
    /// Engage point of the ABS. Will be more sensitive on lower values.
    /// </summary>
    [Range(.01f, .5f)] public float engageABSThreshold = .35f;

    /// <summary>
    /// Engage point of the ESP. Will be more sensitive on lower values.
    /// </summary>
    [Range(.01f, .5f)] public float engageESPThreshold = .5f;

    /// <summary>
    /// Engage point of the TCS. Will be more sensitive on lower values.
    /// </summary>
    [Range(.01f, .5f)] public float engageTCSThreshold = .35f;

    /// <summary>
    /// ABS intensity.
    /// </summary>
    [Range(0f, 1f)] public float ABSIntensity = 1f;

    /// <summary>
    /// ESP intensity.
    /// </summary>
    [Range(0f, 1f)] public float ESPIntensity = 1f;

    /// <summary>
    /// TCS intensity.
    /// </summary>
    [Range(0f, 1f)] public float TCSIntensity = 1f;

    /// <summary>
    /// ABS engaged now.
    /// </summary>
    public bool ABSEngaged = false;

    /// <summary>
    /// ESP engaged now.
    /// </summary>
    public bool ESPEngaged = false;

    /// <summary>
    /// TCS engaged now.
    /// </summary>
    public bool TCSEngaged = false;

    /// <summary>
    /// Steering helper. Add forces to right or left side depends on vehicle velocity.
    /// </summary>
    public bool steeringHelper = true;

    /// <summary>
    /// Traction helper. Reduces stiffness of the front wheels if vehicle is skidding to avoid spins.
    /// </summary>
    public bool tractionHelper = true;

    /// <summary>
    /// Angular drag helper. Adjusts angular drag intensity depends on the vehicle speed.
    /// </summary>
    public bool angularDragHelper = false;

    /// <summary>
    /// Turn helper. Applies force to Y axis depends on steering angle.
    /// </summary>
    public bool turnHelper = false;

    /// <summary>
    /// Steering helper strength.
    /// </summary>
    [Range(0f, 1f)] public float steerHelperStrength = .1f;

    /// <summary>
    /// Traction helper strength.
    /// </summary>
    [Range(0f, 1f)] public float tractionHelperStrength = .1f;

    /// <summary>
    /// Angular drag helper strength.
    /// </summary>
    [Range(0f, 1f)] public float angularDragHelperStrength = .1f;

    /// <summary>
    /// Turn helper strength.
    /// </summary>
    [Range(0f, 1f)] public float turnHelperStrength = .1f;

    private Transform velocityDirection;
    private Transform steeringDirection;
    private float velocityAngle;

    private float oldRotation, angle, angularVelo;

    private void FixedUpdate() {

        if (ESP)
            UpdateESP();

        if (TCS)
            UpdateTCS();

        if (ABS)
            UpdateABS();

        if (steeringHelper)
            SteerHelper();

        if (tractionHelper)
            TractionHelper();

        if (angularDragHelper)
            AngularDragHelper();

        if (turnHelper)
            TurnHelper();

        if (RCCP_Settings.Instance.SelectedBehaviorType != null && RCCP_Settings.Instance.SelectedBehaviorType.driftMode)
            Drift();

    }

    private void Drift() {

        float rearWheelSlipAmountForward = 0f;
        float rearWheelSlipAmountSideways = 0f;

        if (CarController && CarController.RearAxle) {

            rearWheelSlipAmountForward = (CarController.RearAxle.leftWheelCollider.wheelSlipAmountForward + CarController.RearAxle.rightWheelCollider.wheelSlipAmountForward) * .5f;
            rearWheelSlipAmountSideways = (CarController.RearAxle.leftWheelCollider.wheelSlipAmountSideways + CarController.RearAxle.rightWheelCollider.wheelSlipAmountSideways) * .5f;

        }

        float pRearWheelSlipAmountForward = (rearWheelSlipAmountForward * rearWheelSlipAmountForward) * Mathf.Sign(rearWheelSlipAmountForward);
        float pRearWheelSlipAmountSideways = (rearWheelSlipAmountSideways * rearWheelSlipAmountSideways) * Mathf.Sign(rearWheelSlipAmountSideways);

        CarController.Rigid.AddRelativeTorque(Vector3.up * CarController.steerInput_P * CarController.direction * 1f, ForceMode.Acceleration);

        CarController.Rigid.AddForceAtPosition(transform.forward * 4500f * Mathf.Abs(pRearWheelSlipAmountSideways) * Mathf.Clamp01(Mathf.Abs(pRearWheelSlipAmountForward * 10f)) * CarController.direction, transform.position, ForceMode.Force);
        CarController.Rigid.AddForceAtPosition(transform.right * 1500f * pRearWheelSlipAmountSideways * Mathf.Clamp01(Mathf.Abs(Mathf.Clamp(pRearWheelSlipAmountForward, .5f, 1f) * 10f)) * CarController.direction, transform.position, ForceMode.Force);

    }

    /// <summary>
    /// ABS.
    /// </summary>
    private void UpdateABS() {

        //  Setting abs to false before checking it.
        ABSEngaged = false;

        //  If no axle found, return.
        if (CarController.AxleManager == null)
            return;

        //  Finding braked axles. If not found, return.
        if (CarController.brakedAxles == null)
            return;

        //  Finding braked axles. If not found, return.
        if (CarController.brakedAxles.Count < 1)
            return;

        //  Checking braked axles and their connected wheel's forward slip amount. If vehicle is applying brake, and wheels are skidding, engage ABS. Otherwise, disable it.
        for (int i = 0; i < CarController.brakedAxles.Count; i++) {

            if ((Mathf.Abs(CarController.brakedAxles[i].leftWheelCollider.wheelSlipAmountForward) * CarController.brakedAxles[i].brakeInput) >= engageABSThreshold) {

                CarController.brakedAxles[i].leftWheelCollider.CutBrakeABS(ABSIntensity);
                ABSEngaged = true;

            }

            if ((Mathf.Abs(CarController.brakedAxles[i].rightWheelCollider.wheelSlipAmountForward) * CarController.brakedAxles[i].brakeInput) >= engageABSThreshold) {

                CarController.brakedAxles[i].rightWheelCollider.CutBrakeABS(ABSIntensity);
                ABSEngaged = true;

            }

        }

    }

    /// <summary>
    /// ESP.
    /// </summary>
    private void UpdateESP() {

        ESPEngaged = false;

        //  If no axle found, return.
        if (CarController.AxleManager == null)
            return;

        //  If there are no front and rear axle found, return.
        if (!CarController.FrontAxle || !CarController.RearAxle)
            return;

        //  Getting front and rear wheels sideways slip.
        float frontSlip = CarController.FrontAxle.leftWheelCollider.wheelSlipAmountSideways + CarController.FrontAxle.rightWheelCollider.wheelSlipAmountSideways;
        float rearSlip = CarController.RearAxle.leftWheelCollider.wheelSlipAmountSideways + CarController.RearAxle.rightWheelCollider.wheelSlipAmountSideways;

        //  If front wheels are skidding, under steering. If rear wheels are skidding, over steering.
        bool underSteering, overSteering;

        if (Mathf.Abs(frontSlip) >= engageESPThreshold)
            underSteering = true;
        else
            underSteering = false;

        if (Mathf.Abs(rearSlip) >= engageESPThreshold)
            overSteering = true;
        else
            overSteering = false;

        //  If over steering or under steering, set esp engaged to true. Otherwise to false.
        if (overSteering || underSteering)
            ESPEngaged = true;

        //  If under steering, apply brake and cut traction of the corresponding wheel.
        if (underSteering) {

            if (CarController.FrontAxle.isBrake) {

                CarController.FrontAxle.leftWheelCollider.AddBrakeTorque((CarController.FrontAxle.maxBrakeTorque * (ESPIntensity * .25f)) * Mathf.Clamp(-rearSlip, 0f, Mathf.Infinity));
                CarController.FrontAxle.rightWheelCollider.AddBrakeTorque((CarController.FrontAxle.maxBrakeTorque * (ESPIntensity * .25f)) * Mathf.Clamp(rearSlip, 0f, Mathf.Infinity));

            }

        }

        //  If over steering, apply brake and cut traction of the corresponding wheel.
        if (overSteering) {

            if (CarController.RearAxle.isBrake) {

                CarController.RearAxle.leftWheelCollider.AddBrakeTorque((CarController.RearAxle.maxBrakeTorque * (ESPIntensity * .25f)) * Mathf.Clamp(-frontSlip, 0f, Mathf.Infinity));
                CarController.RearAxle.rightWheelCollider.AddBrakeTorque((CarController.RearAxle.maxBrakeTorque * (ESPIntensity * .25f)) * Mathf.Clamp(frontSlip, 0f, Mathf.Infinity));

            }

        }

    }

    /// <summary>
    /// TCS.
    /// </summary>
    private void UpdateTCS() {

        //  Setting TCS engaged to false before checking it.
        TCSEngaged = false;

        //  If no axle found, return.
        if (CarController.AxleManager == null)
            return;

        //  If there are no powered axle found, return.
        if (CarController.poweredAxles == null)
            return;

        // If there are no powered axle found, return.
        if (CarController.poweredAxles.Count < 1)
            return;

        //  If powered axles found, check their forward slips. If they breaks the point, engage TCS.
        for (int i = 0; i < CarController.poweredAxles.Count; i++) {

            if (CarController.direction == 1) {

                if (CarController.poweredAxles[i].leftWheelCollider.wheelSlipAmountForward >= engageTCSThreshold) {

                    CarController.poweredAxles[i].leftWheelCollider.CutTractionTCS(TCSIntensity);
                    TCSEngaged = true;

                }

                if (CarController.poweredAxles[i].rightWheelCollider.wheelSlipAmountForward >= engageTCSThreshold) {

                    CarController.poweredAxles[i].rightWheelCollider.CutTractionTCS(TCSIntensity);
                    TCSEngaged = true;

                }

            } else if (CarController.direction == -1) {

                if (CarController.poweredAxles[i].leftWheelCollider.wheelSlipAmountForward <= -engageTCSThreshold) {

                    CarController.poweredAxles[i].leftWheelCollider.CutTractionTCS(TCSIntensity);
                    TCSEngaged = true;

                }

                if (CarController.poweredAxles[i].rightWheelCollider.wheelSlipAmountForward <= -engageTCSThreshold) {

                    CarController.poweredAxles[i].rightWheelCollider.CutTractionTCS(TCSIntensity);
                    TCSEngaged = true;

                }

            }

        }

    }

    /// <summary>
    /// Steering helper.
    /// </summary>
    private void SteerHelper() {

        //  If vehicle is not grounded, return.
        if (!CarController.IsGrounded)
            return;

        //  Return if kinematic rigid is enabled.
        if (CarController.Rigid.isKinematic)
            return;

        //  Applying steering helper force based on rotation. 
        if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f) {

            float turnadjust = (transform.eulerAngles.y - oldRotation) * (steerHelperStrength / 2f);
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            CarController.Rigid.velocity = (velRotation * CarController.Rigid.velocity);

        }

        oldRotation = transform.eulerAngles.y;

    }

    /// <summary>
    /// Traction helper.
    /// </summary>
    private void TractionHelper() {

        // If vehicle is not grounded, return.
        if (!CarController.IsGrounded)
            return;

        //  Return if kinematic rigid is enabled.
        if (CarController.Rigid.isKinematic)
            return;

        //  Finding front axle.
        RCCP_Axle frontAxle = CarController.FrontAxle;

        //  If no front axle found, return.
        if (!frontAxle)
            return;

        //  Getting velocity of the vehicle and taking dot of the velocity with transform.up ref.
        Vector3 velocity = CarController.Rigid.velocity;
        velocity -= transform.up * Vector3.Dot(velocity, transform.up);
        velocity.Normalize();

        angle = -Mathf.Asin(Vector3.Dot(Vector3.Cross(transform.forward, velocity), transform.up));
        angularVelo = CarController.Rigid.angularVelocity.y;

        if (angle * frontAxle.steerAngle < 0)
            frontAxle.tractionHelpedSidewaysStiffness = (1f - Mathf.Clamp01(tractionHelperStrength * Mathf.Abs(angularVelo)));
        else
            frontAxle.tractionHelpedSidewaysStiffness = 1f;

    }

    /// <summary>
    /// Angular drag helper.
    /// </summary>
    private void AngularDragHelper() {

        CarController.Rigid.angularDrag = Mathf.Lerp(0f, 10f, (Mathf.Abs(CarController.speed) * angularDragHelperStrength) / 1000f);

    }

    /// <summary>
    /// Turn helper.
    /// </summary>
    private void TurnHelper() {

        //  If vehicle is not grounded, return.
        if (!CarController.IsGrounded)
            return;

        //  Return if kinematic rigid is enabled.
        if (CarController.Rigid.isKinematic)
            return;

        //  Creating direction angles for vehicle and steering.
        if (!steeringDirection || !velocityDirection) {

            if (!steeringDirection) {

                GameObject steeringDirectionGO = new GameObject("Steering Direction");
                steeringDirectionGO.transform.SetParent(transform, false);
                steeringDirection = steeringDirectionGO.transform;
                steeringDirectionGO.transform.localPosition = new Vector3(1f, 2f, 0f);
                steeringDirectionGO.transform.localScale = new Vector3(.1f, .1f, 3f);

            }

            if (!velocityDirection) {

                GameObject velocityDirectionGO = new GameObject("Velocity Direction");
                velocityDirectionGO.transform.SetParent(transform, false);
                velocityDirection = velocityDirectionGO.transform;
                velocityDirectionGO.transform.localPosition = new Vector3(-1f, 2f, 0f);
                velocityDirectionGO.transform.localScale = new Vector3(.1f, .1f, 3f);

            }

            return;

        }

        //  Getting angular velocity of the vehicle, and converting it to degree. And then assigning rotations of the velocity and steering directions. 
        Vector3 v = CarController.Rigid.angularVelocity;
        velocityAngle = (v.y * Mathf.Sign(transform.InverseTransformDirection(CarController.Rigid.velocity).z)) * Mathf.Rad2Deg;
        velocityDirection.localRotation = Quaternion.Lerp(velocityDirection.localRotation, Quaternion.AngleAxis(Mathf.Clamp(velocityAngle / 3f, -45f, 45f), Vector3.up), Time.fixedDeltaTime * 5f);
        steeringDirection.localRotation = Quaternion.Euler(0f, CarController.FrontAxle.steerAngle, 0f);

        int normalizer;

        if (steeringDirection.localRotation.y > velocityDirection.localRotation.y)
            normalizer = 1;
        else
            normalizer = -1;

        float angle2 = Quaternion.Angle(velocityDirection.localRotation, steeringDirection.localRotation) * (normalizer);

        //  Applies relative torque to the vehicle based on velocity - steering direction angles.
        CarController.Rigid.AddRelativeTorque(Vector3.up * ((angle2 * (Mathf.Clamp(transform.InverseTransformDirection(CarController.Rigid.velocity).z, -10f, 10f) / 5000f)) * turnHelperStrength), ForceMode.VelocityChange);

    }

    public void Reload() {

        ABSEngaged = false;
        ESPEngaged = false;
        TCSEngaged = false;
        velocityAngle = 0f;
        oldRotation = 0f;
        angle = 0f;
        angularVelo = 0f;

    }

}
