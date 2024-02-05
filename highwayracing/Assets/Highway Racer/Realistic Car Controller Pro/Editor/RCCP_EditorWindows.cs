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

public class RCCP_EditorWindows : Editor {

    #region Edit Settings
#if RCCP_SHORTCUTS
    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Edit RCCP Settings #s", false, -100)]
    public static void OpenRCCSettings() {
        Selection.activeObject = RCCP_Settings.Instance;
    }
#else
    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Edit RCCP Settings", false, -100)]
    public static void OpenRCCSettings() {
        Selection.activeObject = RCCP_Settings.Instance;
    }
#endif
    #endregion

    #region Configure

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Configure/Ground Materials", false, -65)]
    public static void OpenGroundMaterialsSettings() {
        Selection.activeObject = RCCP_GroundMaterials.Instance;
    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Configure/Changable Wheels", false, -65)]
    public static void OpenChangableWheelSettings() {
        Selection.activeObject = RCCP_ChangableWheels.Instance;
    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Configure/Recorded Clips", false, -65)]
    public static void OpenRecordSettings() {
        Selection.activeObject = RCCP_Records.Instance;
    }

    #endregion

    #region Managers

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Create/Managers/Add RCCP Scene Manager To Scene", false, -50)]
    public static void CreateRCCPSceneManager() {
        Selection.activeObject = RCCP_SceneManager.Instance;
    }

    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Create/Managers/Add RCCP Scene Manager To Scene", false, -50)]
    public static void CreateRCCPSceneManager2() {
        Selection.activeObject = RCCP_SceneManager.Instance;
    }

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Create/Managers/Add RCCP Skidmarks Manager To Scene", false, -50)]
    public static void CreateRCCPSkidmarksManager() {
        Selection.activeObject = RCCP_SkidmarksManager.Instance;
    }

    [MenuItem("GameObject/BoneCracker Games/Realistic Car Controller Pro/Create/Managers/Add RCCP Skidmarks Manager To Scene", false, -50)]
    public static void CreateRCCPSkidmarksManager2() {
        Selection.activeObject = RCCP_SkidmarksManager.Instance;
    }

    #endregion

    #region Help
    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Help", false, 0)]
    public static void Help() {

        EditorUtility.DisplayDialog("Contact", "Please include your invoice number while sending a contact form. I usually respond within a business day.", "Close");

        string url = "http://www.bonecrackergames.com/contact/";
        Application.OpenURL(url);

    }
    #endregion Help

    //    #region Logitech
    //#if RCC_LOGITECH
    //	[MenuItem("Tools/BoneCracker Games/Realistic Car Controller/Create/Logitech/Logitech Manager", false, -50)]
    //	public static void CreateLogitech() {

    //		RCC_LogitechSteeringWheel logi = RCC_LogitechSteeringWheel.Instance;
    //		Selection.activeGameObject = logi.gameObject;

    //	}
    //#endif
    //    #endregion

    //[MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Export Project Settings", false, 10)]
    //public static void ExportProjectSettings() {

    //    string[] projectContent = new string[] { "ProjectSettings/InputManager.asset" };
    //    AssetDatabase.ExportPackage(projectContent, "RCCP_ProjectSettings.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
    //    Debug.Log("Project Exported");

    //}

}
