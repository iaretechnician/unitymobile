//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RCCP_Exterior_Cameras))]
public class RCCP_ExteriorCamerasEditor : Editor {

    RCCP_Exterior_Cameras prop;
    GUISkin skin;

    private void OnEnable() {

        skin = Resources.Load<GUISkin>("RCCP_Gui");

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Exterior_Cameras)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Exterior cameras attached to the vehicle as hood camera and wheel camera.", MessageType.Info, true);

        //DrawDefaultInspector();

        GUILayout.Space(10f);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.EndVertical();

        if (!EditorUtility.IsPersistent(prop)) {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            RCCP_HoodCamera hoodCamera = prop.GetComponentInChildren<RCCP_HoodCamera>(true);
            RCCP_WheelCamera wheelCamera = prop.GetComponentInChildren<RCCP_WheelCamera>(true);

            if (hoodCamera == null) {

                if (GUILayout.Button("Add Hood / Cockpit Camera"))
                    CreateHoodCamera();

            } else {

                if (GUILayout.Button("Select Hood / Cockpit Camera"))
                    Selection.activeGameObject = hoodCamera.gameObject;

            }

            if (wheelCamera == null) {

                if (GUILayout.Button("Add Wheel Camera"))
                    CreateWheelCamera();

            } else {

                if (GUILayout.Button("Select Wheel Camera"))
                    Selection.activeGameObject = wheelCamera.gameObject;

            }

            if (GUILayout.Button("Back"))
                Selection.activeObject = prop.GetComponentInParent<RCCP_OtherAddons>(true).gameObject;

            EditorGUILayout.EndVertical();

        }

        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CreateHoodCamera() {

        GameObject hoodCam = Instantiate(RCCP_Settings.Instance.RCCPHoodCamera, prop.transform.position, prop.transform.rotation);
        hoodCam.name = RCCP_Settings.Instance.RCCPHoodCamera.name;
        hoodCam.transform.SetParent(prop.transform, true);
        hoodCam.GetComponent<RCCP_HoodCamera>().Reset();
        Selection.activeGameObject = hoodCam;

    }

    private void CreateWheelCamera() {

        GameObject wheelCam = Instantiate(RCCP_Settings.Instance.RCCPWheelCamera, prop.transform.position, prop.transform.rotation);
        wheelCam.name = RCCP_Settings.Instance.RCCPWheelCamera.name;
        wheelCam.transform.SetParent(prop.transform, true);
        Selection.activeGameObject = wheelCam;

    }

}
