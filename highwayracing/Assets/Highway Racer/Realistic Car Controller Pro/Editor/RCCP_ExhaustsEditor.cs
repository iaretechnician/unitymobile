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

[CustomEditor(typeof(RCCP_Exhausts))]
public class RCCP_ExhaustsEditor : Editor {

    RCCP_Exhausts prop;
    GUISkin skin;
    Color guiColor;

    private void OnEnable() {

        skin = Resources.Load<GUISkin>("RCCP_Gui");
        guiColor = GUI.color;

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_Exhausts)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Exhausts.", MessageType.Info, true);

        if (prop.Exhaust != null) {

            for (int i = 0; i < prop.Exhaust.Length; i++) {

                EditorGUILayout.BeginHorizontal();

                GameObject exhaust = prop.Exhaust[i].gameObject;
                EditorGUILayout.ObjectField(exhaust, typeof(GameObject), true);

                if (GUILayout.Button("Edit"))
                    Selection.activeObject = exhaust;

                EditorGUILayout.EndHorizontal();

            }

        }

        GUILayout.Space(10f);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.EndVertical();

        if (!EditorUtility.IsPersistent(prop)) {

            EditorGUILayout.BeginVertical(GUI.skin.box);

            GUI.color = Color.green;

            if (GUILayout.Button("Create Exhaust"))
                CreateExhaust();

            GUI.color = guiColor;

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

    private void CreateExhaust() {

        GameObject exhaust = (GameObject)Instantiate(RCCP_Settings.Instance.exhaustGas, prop.transform.position, prop.transform.rotation * Quaternion.Euler(0f, 180f, 0f));
        exhaust.name = RCCP_Settings.Instance.exhaustGas.name;
        exhaust.transform.SetParent(prop.transform, true);
        exhaust.transform.localPosition = new Vector3(0f, 0f, -2f);
        Selection.activeGameObject = exhaust;

    }

}
