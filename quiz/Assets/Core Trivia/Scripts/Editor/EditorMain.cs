using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using UnityEditor.Compilation;

namespace CoreTrivia.Editor
{
    public class CustomStyles
    {
        public static GUISkin inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

        public static GUIStyle standardBtn = new GUIStyle(inspectorSkin.button)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,

            fixedHeight = 30
        };
    }
}

[InitializeOnLoad]
public class EditorMain : MonoBehaviour
{
    private static string InstallPrefKey = "CoreTrivia.Installed";
    private static string DontAskAgainPrefKey = "CoreTrivia.LeaveMeAlone";

    private static DateTime LastCheckTime;

    static EditorMain()
    {
        if (EditorPrefs.HasKey(DontAskAgainPrefKey))
            return;

        if (!EditorPrefs.HasKey(InstallPrefKey))
        {
            EditorPrefs.SetString(InstallPrefKey, DateTime.UtcNow.ToString());
        }

        EditorApplication.update -= CheckRatingStatus;
        EditorApplication.update += CheckRatingStatus;
    }

    private static void CheckRatingStatus()
    {
        DateTime InstallDateTime = DateTime.Parse(EditorPrefs.GetString(InstallPrefKey));

        TimeSpan timeDiff = DateTime.UtcNow.Subtract(InstallDateTime);

        if (timeDiff.TotalDays < 7)
        {
            EditorApplication.update -= CheckRatingStatus;
            return;
        }

        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPaused || EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            LastCheckTime = DateTime.UtcNow;
            return;
        }

        if (EditorApplication.timeSinceStartup < 300)
            return;

        if (DateTime.UtcNow.Subtract(LastCheckTime).TotalSeconds < 30)
            return;

        ShowRatingDialog();
    }

    private static void ShowRatingDialog()
    {
        bool CanRate = EditorUtility.DisplayDialog("Rate Core Trivia", "If you enjoy using Core Trivia, kindly take a moment to rate it. Thank you for the support.\n\nGo to Tools > Core Trivia to disable this dialog.", "Rate", "Later");

        if (CanRate)
        {
            RateAsset();
        }
        else
        {
            EditorApplication.update -= CheckRatingStatus;
            EditorPrefs.SetString(InstallPrefKey, DateTime.UtcNow.ToString());
        }
    }

    [MenuItem("Tools/Core Trivia/Official Website", false, 1)]
    internal static void OpenWebsite()
    {
        Application.OpenURL("http://www.coretrivia.com");
    }

    [MenuItem("Tools/Core Trivia/Documentation", false, 2)]
    internal static void OpenDocumentation()
    {
        Application.OpenURL("http://docs.coretrivia.com");
    }

    [MenuItem("Tools/Core Trivia/Rate Core Trivia", false, 2)]
    internal static void RateAsset()
    {
        Application.OpenURL("https://assetstore.unity.com/packages/slug/203504?aid=1101lHeK");
        DisableRatingPopup();
    }

    [MenuItem("Tools/Core Trivia/Helpers/Disable Rating Popup", false, 15)]
    internal static void LeaveMeAlone()
    {
        if (EditorUtility.DisplayDialog("Disable Rating Popup", "Would you like to disable the rating popup?", "Yes", "Cancel"))
        {
            DisableRatingPopup(true);
        }
    }

    private static void DisableRatingPopup(bool silent = true)
    {
        EditorPrefs.SetBool(DontAskAgainPrefKey, true);

        if (!silent)
        {
            EditorUtility.DisplayDialog("Success", "The rating popup has been disabled.", "OK");
        }
    }

    [MenuItem("Tools/Core Trivia/Helpers/Update Globals", false, 18)]
    internal static void SelectGlobals()
    {
        GameObject GlobalsObj = GameObject.Find("Globals");

        Selection.activeObject = GlobalsObj.gameObject;
    }
}
