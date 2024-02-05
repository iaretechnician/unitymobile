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
using System.IO;

[CustomEditor(typeof(RCCP_WheelBlur))]
public class RCCP_WheelBlurEditor : Editor {

    RCCP_WheelBlur prop;
    GUISkin skin;
    bool previewing;
    List<GameObject> previews = new List<GameObject>();

    private void OnEnable() {

        skin = Resources.Load<GUISkin>("RCCP_Gui");
        previewing = false;

    }

    private void OnDisable() {

        previewing = false;

        for (int i = 0; i < previews.Count; i++)
            DestroyImmediate(previews[i]);

    }

    public override void OnInspectorGUI() {

        prop = (RCCP_WheelBlur)target;
        serializedObject.Update();
        GUI.skin = skin;

        EditorGUILayout.HelpBox("Fake wheel blur textures will be created and aligned. Offset and scale can be changed.", MessageType.Info, true);
        EditorGUILayout.HelpBox("Use unique material per vehicle.", MessageType.Info);

        if (Application.isPlaying)
            GUI.enabled = false;

        if (GUILayout.Button("Create New Material"))
            CreateNewMaterial();

        if (previewing) {

            GUI.color = Color.green;

            previews.Clear();

            foreach (Transform item in prop.transform) {

                if (item != prop.transform)
                    previews.Add(item.gameObject);

            }

        }

        if (GUILayout.Button("Toggle")) {

            if (!previewing) {

                previewing = true;
                prop.CreateRenderers();

            } else {

                previewing = false;
                prop.DestroyRenderersEditor();

            }

        }

        GUI.color = Color.white;

        if (!Application.isPlaying) {

            if (previewing)
                prop.Toggle();
            else
                prop.DestroyRenderersEditor();

        }

        GUI.enabled = true;

        DrawDefaultInspector();

        if (!EditorUtility.IsPersistent(prop)) {

            EditorGUILayout.BeginVertical(GUI.skin.box);

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

    private void CreateNewMaterial() {

        prop.targetMaterial = null;
        Material material;

#if !BCG_URP
        material = new Material(Shader.Find("RCCP_WheelBlur"));
#else
        material = new Material(Shader.Find("RCCP_WheelBlur_URP"));
#endif

        string newAssetName = "Assets/" + prop.transform.root.name + "_WheelBlur" + ".mat";

        if (File.Exists(newAssetName)) {

            AssetDatabase.DeleteAsset(newAssetName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

        AssetDatabase.CreateAsset(material, newAssetName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        prop.targetMaterial = material;

    }

}
