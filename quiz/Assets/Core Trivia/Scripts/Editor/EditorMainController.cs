using UnityEngine;
using UnityEditor;
using CoreTrivia;
using CoreTrivia.Editor;

[CustomEditor(typeof(MainController))]
public class EditorMainController : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        GUI.color = Color.cyan;

        if (GUILayout.Button("Official Website", CustomStyles.standardBtn))
            Application.OpenURL("http://www.coretrivia.com");

        GUI.color = Color.yellow;

        if (GUILayout.Button("Documentation", CustomStyles.standardBtn))
            Application.OpenURL("http://docs.coretrivia.com");

        GUI.color = Color.white;

        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUI.color = Color.magenta;

        if (GUILayout.Button("Rate Core Trivia Template", CustomStyles.standardBtn))
            Application.OpenURL("https://assetstore.unity.com/packages/slug/203504?aid=1101lHeK");

        GUI.color = Color.white;

        GUILayout.Space(10);

        DrawDefaultInspector();

        GUILayout.Space(10);
    }
}
