﻿//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RCCP_VehicleUpgrade_SirenManager))]
public class RCCP_VehicleUpgrade_SirenEditor : Editor {

    RCCP_VehicleUpgrade_SirenManager prop;

    public override void OnInspectorGUI() {

        prop = (RCCP_VehicleUpgrade_SirenManager)target;
        serializedObject.Update();

        EditorGUILayout.HelpBox("All sirens can be used under this manager. Each siren has target index. Click 'Get All Sirens' after editing sirens.", MessageType.None);

        DrawDefaultInspector();

        if (GUILayout.Button("Get All Sirens"))
            prop.sirens = prop.GetComponentsInChildren<RCCP_VehicleUpgrade_Siren>(true);

        serializedObject.ApplyModifiedProperties();

    }

}
