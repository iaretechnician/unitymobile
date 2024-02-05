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
/// Connecter between engine and the gearbox. Transmits the received power from the engine to the gearbox or not.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Drivetrain/RCCP Clutch")]
public class RCCP_Clutch : RCCP_Component {

    /// <summary>
    /// Overrides clutch rpm with given rpm value. All calculations will be ignored.
    /// </summary>
    public bool overrideClutch = false;

    /// <summary>
    /// Current clutch input. Clamped to 0 - 1. 0 means, clutch is not pressed, and 1 means clutch is pressed. 
    /// </summary>
    [Range(0f, 1f)] public float clutchInput = 1f;

    /// <summary>
    /// Raw input used to smooth lerp the actual input.
    /// </summary>
    [Range(0f, 1f)] public float clutchInputRaw = 1f;

    /// <summary>
    /// Clutch inertia. Faster reactions on lower values, slower reactions on higher values. Only used with auto.
    /// </summary>
    [Range(.1f, .4f)] public float clutchInertia = .15f;

    /// <summary>
    /// Adjusts clutch input automatically based on the engine rpm, and speed of the vehicle.
    /// </summary>
    public bool automaticClutch = true;

    /// <summary>
    /// Applies clutch input if engine rpm drops on idle. 
    /// </summary>
    public float engageRPM = 800f;

    /// <summary>
    /// Received torque from the component. Usually it would be the engine in this case.
    /// </summary>
    public float receivedTorqueAsNM = 0f;

    /// <summary>
    /// Delivered torque to the next component. Usually it would be the gearbox in this case.
    /// </summary>
    public float producedTorqueAsNM = 0f;

    /// <summary>
    /// Output event with custom class.
    /// </summary>
    public RCCP_Event_Output outputEvent = new RCCP_Event_Output();
    public RCCP_Output output = new RCCP_Output();

    float velocity;

    private void Update() {

        //  Calculating clutch input.
        Input();

    }

    private void FixedUpdate() {

        //  Delivering torque.
        Output();

    }

    /// <summary>
    /// Calculating clutch input.
    /// </summary>
    private void Input() {

        //  Early out if override clutch is enabled. This means an external class will adjust input value.
        if (overrideClutch)
            return;

        //  If automatic clutch is not enabled, receive the input directly from the player. Otherwise, calculate it.
        if (!automaticClutch) {

            clutchInputRaw = CarController.clutchInput_P;

        } else {

            //  Looks like automatic clutch is enabled. Apply input 1 if engine rpm drops below the engage rpm.
            if (CarController.engineRPM <= engageRPM) {

                clutchInputRaw = 1f;

                //  If engine rpm is above the engage rpm, but vehicle is on low speeds, calculate the estimated input based on vehicle speed and throttle input.
            } else if (Mathf.Abs(CarController.speed) < 20f) {

                clutchInputRaw = Mathf.Lerp(clutchInputRaw, (Mathf.Lerp(1f, (Mathf.Lerp(.5f, 0f, (Mathf.Abs(CarController.speed)) / 20f)), Mathf.Abs(CarController.throttleInput_V * CarController.gearInput_V))), Time.fixedDeltaTime * 20f);

                //  If vehicle speed is above the limit, and engine rpm is above the engage rpm, don't apply input.
            } else {

                clutchInputRaw = 0f;

            }

        }

        //  If gearbox is shifting now, apply input.
        if (CarController.shiftingNow)
            clutchInputRaw = 1f;

        //  If handbrake is in action, apply input.
        if (CarController.handbrakeInput_V >= .75f)
            clutchInputRaw = 1f;

        //  Smoothing the clutch input with inertia.
        clutchInput = Mathf.SmoothDamp(clutchInput, clutchInputRaw, ref velocity, clutchInertia);

    }

    /// <summary>
    /// Overrides the clutch input with given value.
    /// </summary>
    /// <param name="targetRPM"></param>
    public void OverrideInput(float targetInput) {

        clutchInput = targetInput;

    }

    /// <summary>
    /// Received torque from the component.
    /// </summary>
    /// <param name="output"></param>
    public void ReceiveOutput(RCCP_Output output) {

        receivedTorqueAsNM = output.NM;

    }

    /// <summary>
    /// Output.
    /// </summary>
    private void Output() {

        if (output == null)
            output = new RCCP_Output();

        producedTorqueAsNM = receivedTorqueAsNM * (1f - clutchInputRaw);

        output.NM = producedTorqueAsNM;
        outputEvent.Invoke(output);

    }

    public void Reload() {

        clutchInput = 1f;
        clutchInputRaw = 1f;
        producedTorqueAsNM = 0f;

    }

}
