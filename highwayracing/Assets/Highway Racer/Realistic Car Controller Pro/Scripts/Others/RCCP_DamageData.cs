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

public class RCCP_DamageData {

    public static void SaveDamage(RCCP_CarController carController, string saveName) {

        RCCP_Damage damageComponent = carController.Damage;

        if (damageComponent == null) {

            Debug.LogError("Damage component couldn't found on the vehicle named: " + carController.transform.name + "!");
            return;

        }

        if (damageComponent.damageData == null)
            damageComponent.damageData = new RCCP_Damage.DamageData();

        damageComponent.damageData.Initialize(damageComponent);

        PlayerPrefs.SetString(saveName + "_DamageData", JsonUtility.ToJson(damageComponent.damageData));

        Debug.Log("Damage Saved For " + damageComponent.transform.root.name);

    }

    public static void LoadDamage(RCCP_CarController carController, string saveName) {

        RCCP_Damage damageComponent = carController.Damage;

        if (damageComponent == null) {

            Debug.LogError("Damage component couldn't found on the vehicle named: " + carController.transform.name + "!");
            return;

        }

        RCCP_Damage.DamageData damageData = JsonUtility.FromJson<RCCP_Damage.DamageData>(PlayerPrefs.GetString(saveName + "_DamageData"));

        if (damageData == null) {

            Debug.LogError("Damage data couldn't found on the vehicle named: " + carController.transform.name + "!");
            return;

        }

        if (damageComponent.damageData == null)
            damageComponent.damageData = new RCCP_Damage.DamageData();

        damageComponent.originalMeshData = damageData.originalMeshData;
        damageComponent.originalWheelData = damageData.originalWheelData;
        damageComponent.damagedMeshData = damageData.damagedMeshData;
        damageComponent.damagedWheelData = damageData.damagedWheelData;

        if (damageComponent.lights != null && damageComponent.lights.Length >= 1) {

            for (int i = 0; i < damageData.lightData.Length; i++)
                damageComponent.lights[i].broken = damageData.lightData[i];

        }

        damageComponent.repaired = false;
        damageComponent.repairNow = false;
        damageComponent.deformingNow = true;
        damageComponent.deformed = false;
        damageComponent.deformationTime = 0f;

        damageComponent.CheckDamage();

        Debug.Log("Damage data loaded on vehicle named: " + carController.transform.name);

    }

}
