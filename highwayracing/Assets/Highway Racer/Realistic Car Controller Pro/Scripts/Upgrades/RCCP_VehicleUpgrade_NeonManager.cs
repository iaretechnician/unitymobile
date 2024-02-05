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
/// Manager for upgradable neons.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Customization/RCCP Vehicle Upgrade Neon Manager")]
public class RCCP_VehicleUpgrade_NeonManager : RCCP_UpgradeComponent, IRCCP_UpgradeComponent {

    /// <summary>
    /// Neon painters.
    /// </summary>
    public RCCP_VehicleUpgrade_Neon neon;

    /// <summary>
    /// Indexes of neons.
    /// </summary>
    public int index = -1;

    /// <summary>
    /// Neon materials.
    /// </summary>
    public Material[] neons;

    /// <summary>
    /// Empty material.
    /// </summary>
    public Material neon_Null;

    public void Initialize() {

        //  If neon is null, return.
        if (neon == null)
            return;

        //  Setting neon material to null.
        neon.SetNeon(neon_Null);

        //  And then getting index values from the loadout. -1 means it's empty.
        index = Loadout.neonIndex;

        //  If index is not -1, set material of the neon by the loadout.
        if (index != -1) {

            neon.gameObject.SetActive(true);
            neon.SetNeon(neons[index]);

        }

    }

    public void DisableAll() {

        //  If neon is null, return.
        if (neon == null)
            return;

        //  If index is not -1, set material of the decal by the loadout.
        neon.SetNeon(neon_Null);

        neon.gameObject.SetActive(false);

    }

    public void EnableAll() {

        //  If neon is null, return.
        if (neon == null)
            return;

        //  If index is not -1, set material of the decal by the loadout.
        neon.SetNeon(neon_Null);

        neon.gameObject.SetActive(true);

    }

    /// <summary>
    /// Upgrades target neon index and saves it.
    /// </summary>
    /// <param name="index"></param>
    public void Upgrade(Material material) {

        //  If neon is null, return.
        if (neon == null)
            return;

        neon.gameObject.SetActive(true);

        neon.SetNeon(material);
        index = FindMaterialIndex(material);

        //  Refreshing the loadout.
        Refresh(this);

        //  Saving the loadout.
        if (CarController.Customizer.autoSave)
            Save();

    }

    /// <summary>
    /// Upgrades target neon index and saves it.
    /// </summary>
    /// <param name="index"></param>
    public void UpgradeWithoutSave(Material material) {

        //  If neon is null, return.
        if (neon == null)
            return;

        neon.gameObject.SetActive(true);

        neon.SetNeon(material);
        index = FindMaterialIndex(material);

    }

    /// <summary>
    /// Restores the settings to default.
    /// </summary>
    public void Restore() {

        //  If empty decal is null, return.
        if (neon_Null == null)
            return;

        neon.SetNeon(neon_Null);
        neon.gameObject.SetActive(false);

    }

    /// <summary>
    /// Finds index of the material.
    /// </summary>
    /// <param name="_material"></param>
    /// <returns></returns>
    private int FindMaterialIndex(Material _material) {

        int index = -1;

        if (neons != null) {

            for (int i = 0; i < neons.Length; i++) {

                if (neons[i] == _material)
                    index = i;

            }

        }

        return index;

    }

}
