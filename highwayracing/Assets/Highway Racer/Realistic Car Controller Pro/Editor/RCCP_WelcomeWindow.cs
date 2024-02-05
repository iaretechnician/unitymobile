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
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class RCCP_WelcomeWindow : EditorWindow {

    public class ToolBar {

        public string title;
        public UnityEngine.Events.UnityAction Draw;

        /// <summary>
        /// Create New Toolbar
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="onDraw">Method to draw when toolbar is selected</param>
        public ToolBar(string title, UnityEngine.Events.UnityAction onDraw) {

            this.title = title;
            this.Draw = onDraw;

        }

        public static implicit operator string(ToolBar tool) {
            return tool.title;
        }

    }

    /// <summary>
    /// Index of selected toolbar.
    /// </summary>
    public int toolBarIndex = 0;

    /// <summary>
    /// List of Toolbars
    /// </summary>
    public ToolBar[] toolBars = new ToolBar[]{

        new ToolBar("Welcome", WelcomePageContent),
        new ToolBar("Updates", UpdatePageContent),
        new ToolBar("Addons", Addons),
        new ToolBar("Keys", Keys),
        new ToolBar("DOCS", Documentations)

    };

    public static Texture2D bannerTexture = null;

    private GUISkin skin;

    private const int windowWidth = 600;
    private const int windowHeight = 720;

    [MenuItem("Tools/BoneCracker Games/Realistic Car Controller Pro/Welcome Window", false, 100)]
    public static void OpenWindow() {

        GetWindow<RCCP_WelcomeWindow>(true);

    }

    private void OnEnable() {

        titleContent = new GUIContent("Realistic Car Controller Pro");
        maxSize = new Vector2(windowWidth, windowHeight);
        minSize = maxSize;

        InitStyle();

    }

    private void InitStyle() {

        if (!skin)
            skin = Resources.Load("RCCP_Gui") as GUISkin;

        bannerTexture = (Texture2D)Resources.Load("Editor Icons/RCCP_Banner", typeof(Texture2D));

    }

    private void OnGUI() {

        GUI.skin = skin;

        DrawHeader();
        DrawMenuButtons();
        DrawToolBar();
        DrawFooter();

        if (!EditorApplication.isPlaying)
            Repaint();

    }

    private void DrawHeader() {

        GUILayout.Label(bannerTexture, GUILayout.Height(120));

    }

    private void DrawMenuButtons() {

        GUILayout.Space(-10);
        toolBarIndex = GUILayout.Toolbar(toolBarIndex, ToolbarNames());

    }

    #region ToolBars

    public static void WelcomePageContent() {

        GUILayout.Label("<size=18>Welcome!</size>");
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Thank you for purchasing and using Realistic Car Controller Pro. Please read the documentations before use. Also check out the online documentations for updated info. Have fun :)");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        GUILayout.FlexibleSpace();

    }

    public static void UpdatePageContent() {

        GUILayout.Label("<size=18>Updates</size>");

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>Installed Version: </b>" + RCCP_Version.version.ToString());
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>1</b>- Always backup your project before updating RCCP or any asset in your project!");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>2</b>- If you have own assets such as prefabs, audioclips, models, scripts in Realistic Car Controller Pro folder, keep your own asset outside from Realistic Car Controller Pro folder.");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("<b>3</b>- Delete Realistic Car Controller Pro folder, and import latest version to your project.");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        if (GUILayout.Button("Check Updates"))
            Application.OpenURL(RCCP_AssetPaths.assetStorePath);

        GUILayout.Space(6);

        GUILayout.FlexibleSpace();

    }

    public static void Addons() {

        GUILayout.Label("<size=18>Addons</size>");

        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("<b>ProFlares</b>");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Download and import ProFlares"))
            Application.OpenURL(RCCP_AssetPaths.proFlares);

        if (GUILayout.Button("Import ProFlares Integration"))
            AssetDatabase.ImportPackage(RCCP_AddonPackages.Instance.GetAssetPath(RCCP_AddonPackages.Instance.ProFlare), true);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

    }

    public static void Keys() {

        GUILayout.Label("<size=18>Scripting Define Symbols</size>");

        EditorGUILayout.Separator();

        EditorGUILayout.HelpBox("RCCP uses scripting define symbols to work with other addon packages. These packages are; \n\n BoneCracker Shared Assets (Enter / Exit)\nPhoton Integration\nDemo Content\n\nIf you attempt to import these addon packages, corresponding scripting symbol will be added to your build settings. But if you remove these addon packages, scripting symbol will still exists in the build settings and throw errors.", MessageType.None);
        EditorGUILayout.HelpBox("After removing any addon packages, please remove the corresponding scripting symbol in your build settings.\n\nPlease don't attempt to remove the key if package is still existing in the project. Remove the package first, after that you can remove the key.", MessageType.None);

        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("<b>Installed Scripting Symbols</b>");

        EditorGUILayout.BeginHorizontal();

        GUI.color = Color.red;

        if (EditorApplication.isCompiling)
            GUI.enabled = false;

#if !BCG_ENTEREXIT
        GUI.enabled = false;
#endif

        if (GUILayout.Button("Remove BCG_ENTEREXIT"))
            RCCP_SetScriptingSymbol.SetEnabled("BCG_ENTEREXIT", false);

        if (!EditorApplication.isCompiling)
            GUI.enabled = true;

#if !RCCP_DEMO
        GUI.enabled = false;
#endif

        if (GUILayout.Button("Remove RCCP_DEMO"))
            RCCP_SetScriptingSymbol.SetEnabled("RCCP_DEMO", false);

        if (!EditorApplication.isCompiling)
            GUI.enabled = true;

#if !RCCP_PHOTON
        GUI.enabled = false;
#endif

        if (GUILayout.Button("Remove RCCP_PHOTON"))
            RCCP_SetScriptingSymbol.SetEnabled("RCCP_PHOTON", false);

        GUI.enabled = true;
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        GUILayout.FlexibleSpace();

    }

    public static void Documentations() {

        GUILayout.Label("<size=18>Dcoumentation</size>");

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.HelpBox("Offline documentations can be found in the documentations folder.", MessageType.Info);

        if (GUILayout.Button("Online Documentations"))
            Application.OpenURL(RCCP_AssetPaths.documentations);

        if (GUILayout.Button("Youtube Tutorial Videos"))
            Application.OpenURL(RCCP_AssetPaths.YTVideos);

        if (GUILayout.Button("Other Assets"))
            Application.OpenURL(RCCP_AssetPaths.otherAssets);

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

    }

    #endregion

    private string[] ToolbarNames() {

        string[] names = new string[toolBars.Length];

        for (int i = 0; i < toolBars.Length; i++)
            names[i] = toolBars[i];

        return names;

    }

    private void DrawToolBar() {

        GUILayout.BeginArea(new Rect(4, 140, 592, 540));

        toolBars[toolBarIndex].Draw();

        GUILayout.EndArea();

        GUILayout.FlexibleSpace();

    }

    private void DrawFooter() {

        EditorGUILayout.BeginHorizontal("box");

        EditorGUILayout.LabelField("BoneCracker Games", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Realistic Car Controller Pro " + RCCP_Version.version, EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField("Ekrem Bugra Ozdoganlar", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndHorizontal();

    }

    private static void ImportPackage(string package) {

        try {
            AssetDatabase.ImportPackage(package, true);
        }
        catch (Exception) {
            Debug.LogError("Failed to import package: " + package);
            throw;
        }

    }

    private static void DeleteDemoContent() {

        Debug.LogWarning("Deleting demo contents...");

        foreach (var item in RCCP_DemoContent.Instance.contents) {

            if (item != null)
                FileUtil.DeleteFileOrDirectory(RCCP_GetAssetPath.GetAssetPath(item));

        }

        RCCP_DemoVehicles.Instance.vehicles = new RCCP_CarController[1];
        RCCP_DemoVehicles.Instance.vehicles[0] = RCCP_PrototypeContent.Instance.vehicles[0];
        RCCP_DemoMaterials.Instance.demoMaterials = new RCCP_DemoMaterials.MaterialStructure[0];
        RCCP_DemoMaterials.Instance.vehicleBodyMaterials = new Material[0];
        RCCP_DemoScenes.Instance.Clean();

        EditorUtility.SetDirty(RCCP_DemoVehicles.Instance);
        EditorUtility.SetDirty(RCCP_DemoMaterials.Instance);
        EditorUtility.SetDirty(RCCP_DemoScenes.Instance);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RCCP_SetScriptingSymbol.SetEnabled("RCCP_DEMO", false);

        Debug.LogWarning("Deleted demo contents!");
        EditorUtility.DisplayDialog("Deleted Demo Contents", "All demo contents have been deleted!", "Ok");

    }

}
