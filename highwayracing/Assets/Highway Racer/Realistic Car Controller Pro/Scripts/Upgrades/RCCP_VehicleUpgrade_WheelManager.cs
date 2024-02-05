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
/// Manager for upgradable wheels.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Customization/RCCP Vehicle Upgrade Wheel Manager")]
public class RCCP_VehicleUpgrade_WheelManager : RCCP_UpgradeComponent, IRCCP_UpgradeComponent {

    /// <summary>
    /// Current wheel index.
    /// </summary>
    public int wheelIndex = -1;

    /// <summary>
    /// Initializing.
    /// </summary>
    public void Initialize() {

        // If last selected wheel found, change the wheel.
        wheelIndex = Loadout.wheel;

        if (wheelIndex != -1)
            ChangeWheels(RCCP_ChangableWheels.Instance.wheels[wheelIndex].wheel, true);

    }

    /// <summary>
    /// Changes the wheel with the target wheel index.
    /// </summary>
    /// <param name="wheelIndex"></param>
    public void UpdateWheel(int index) {

        //  Setting wheel index.
        wheelIndex = index;

        //  Return if wheel index is not set.
        if (wheelIndex == -1)
            return;

        //  Checking the RCCP_ChangableWheels for selected wheel index.
        if (RCCP_ChangableWheels.Instance.wheels[wheelIndex] == null) {

            Debug.LogError("RCCP_ChangableWheels doesn't have that wheelIndex numbered " + wheelIndex.ToString());
            return;

        }

        //  Changing the wheels.
        ChangeWheels(RCCP_ChangableWheels.Instance.wheels[wheelIndex].wheel, true);

        //  Refreshing the loadout.
        Refresh(this);

        //  Saving the loadout.
        if (CarController.Customizer.autoSave)
            Save();

    }

    /// <summary>
    /// Changes the wheel with the target wheel index.
    /// </summary>
    /// <param name="wheelIndex"></param>
    public void UpdateWheelWithoutSave(int index) {

        //  Setting wheel index.
        wheelIndex = index;

        //  Return if wheel index is not set.
        if (wheelIndex == -1)
            return;

        //  Checking the RCCP_ChangableWheels for selected wheel index.
        if (RCCP_ChangableWheels.Instance.wheels[wheelIndex] == null) {

            Debug.LogError("RCCP_ChangableWheels doesn't have that wheelIndex numbered " + wheelIndex.ToString());
            return;

        }

        //  Changing the wheels.
        ChangeWheels(RCCP_ChangableWheels.Instance.wheels[wheelIndex].wheel, true);

    }

    /// <summary>
    /// Change wheel models. You can find your wheel models array in Tools --> BCG --> RCCP --> Configure Changable Wheels.
    /// </summary>
    public void ChangeWheels(GameObject wheel, bool applyRadius) {

        //  Return if no wheel or wheel is deactivated.
        if (!wheel || (wheel && !wheel.activeSelf))
            return;

        //  Return if no any wheelcolliders found.
        if (CarController.AllWheelColliders == null)
            return;

        //  Return if no any wheelcolliders found.
        if (CarController.AllWheelColliders.Length < 1)
            return;

        //  Looping all wheelcolliders.
        for (int i = 0; i < CarController.AllWheelColliders.Length; i++) {

            //  Disabling renderer of the wheelmodel.
            if (CarController.AllWheelColliders[i].wheelModel.GetComponent<MeshRenderer>())
                CarController.AllWheelColliders[i].wheelModel.GetComponent<MeshRenderer>().enabled = false;

            //  Disabling all child models of the wheel.
            foreach (Transform t in CarController.AllWheelColliders[i].wheelModel.GetComponentInChildren<Transform>())
                t.gameObject.SetActive(false);

            //  Instantiating new wheel model.
            GameObject newWheel = Instantiate(wheel, CarController.AllWheelColliders[i].transform.position, CarController.AllWheelColliders[i].transform.rotation, CarController.AllWheelColliders[i].wheelModel);
            newWheel.transform.localPosition = Vector3.zero;
            newWheel.transform.localRotation = Quaternion.identity;

            //  If wheel is at right side, multiply scale X by -1 for symetry.
            if (CarController.AllWheelColliders[i].transform.localPosition.x > 0f)
                newWheel.transform.localScale = new Vector3(newWheel.transform.localScale.x * -1f, newWheel.transform.localScale.y, newWheel.transform.localScale.z);

            //  If apply radius is set to true, calculate the radius.
            if (applyRadius)
                CarController.AllWheelColliders[i].WheelCollider.radius = RCCP_GetBounds.MaxBoundsExtent(wheel.transform);

        }

    }

    /// <summary>
    /// Restores the settings to default.
    /// </summary>
    public void Restore() {

        wheelIndex = 0;

        //  Changing the wheels.
        ChangeWheels(RCCP_ChangableWheels.Instance.wheels[wheelIndex].wheel, true);

    }

}
