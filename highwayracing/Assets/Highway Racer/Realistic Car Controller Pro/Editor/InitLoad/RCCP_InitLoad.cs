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

public class RCCP_InitLoad : EditorWindow {

    [InitializeOnLoadMethod]
    public static void InitOnLoad() {

        EditorApplication.delayCall += EditorDelayedUpdate;

    }

    public static void EditorDelayedUpdate() {

        CheckSymbols();

        RCCP_DemoScenes.Instance.GetPaths();

#if RCCP_PHOTON
        RCCP_DemoScenes_Photon.Instance.GetPaths();
#endif

#if BCG_ENTEREXIT
        BCG_DemoScenes.Instance.GetPaths();
#endif

    }

    public static void CheckSymbols() {

        bool newInputSystemKey = RCCP_Settings.Instance.useNewInputSystem;

        if (newInputSystemKey) {

#if !BCG_NEWINPUTSYSTEM

            RCCP_SetScriptingSymbol.SetEnabled("BCG_NEWINPUTSYSTEM", true);

#endif

        } else {

#if BCG_NEWINPUTSYSTEM

            RCCP_SetScriptingSymbol.SetEnabled("BCG_NEWINPUTSYSTEM", false);

#endif

        }

        RCCP_SetScriptingSymbol.SetEnabled("BCG_RCCP", true);
        RCCP_SetScriptingSymbol.SetEnabled("BCG_URP", true);
        RCCP_Installation.Check();

    }

}
