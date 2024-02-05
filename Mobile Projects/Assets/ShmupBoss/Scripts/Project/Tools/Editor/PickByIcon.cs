using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ShmupBossEditor
{
    /// <summary>
    /// Provides an easy way to select prefabs by their icon.
    /// </summary>
    public class PickByIcon : EditorWindow
    {
        /// <summary>
        /// The margin between the box and the icons.
        /// </summary>
        private const float BoxSpace = 5;

        /// <summary>
        /// The space margin between asset icons.
        /// </summary>
        private const float IconSpace = 5;

        /// <summary>
        /// Scrolling slider width.
        /// </summary>
        private const float SliderSpace = 15;

        /// <summary>
        /// Assets directory.
        /// </summary>
        private static string dir;

        private float windowWidth;
        private float windowHight;
        private float iconWidth;

        /// <summary>
        /// The editor which creates this window.
        /// </summary>
        private Editor inspectorWindow;

        /// <summary>
        /// Container for the selected prefab.
        /// </summary>
        private ObjectContainer targetContainer;

        /// <summary>
        /// List of prefabs icons.
        /// </summary>
        private List<AssetToggle> icons = new List<AssetToggle>();

        private Vector2 currentScrollPosition = new Vector2(BoxSpace, BoxSpace);

        private int iconID = -1;
        int IconID
        {
            get
            {
                return iconID;
            }
            set
            {
                if (value != -1)
                {
                    targetContainer.ContainedObject = icons[value].Pref;

                    if (inspectorWindow != null)
                    {
                        inspectorWindow.Repaint();
                    }

                    this.Close();
                }

                iconID = -1;
            }
        }

        /// <summary>
        /// Initializes the Windows.
        /// </summary>
        /// <param name="field">The container which will be filled with the selected prefab.</param>
        /// <param name="directory">The directory name.</param>
        /// <param name="extension">The file extension type.</param>
        /// <param name="yourWindow">Reference of the editor that creates a call for this constructor.</param>
        /// <param name="width">The width of the window</param>
        /// <param name="height">The height of the window.</param>
        public void WindowsInitialize(ObjectContainer field, string directory, string extension, Editor yourWindow, float width, float height)
        {
            if (directory != null)
            {
                Refresh(directory, extension);
            }

            GetFilesUnderDirectory("Bullets", "mat");

            // Sets the window to the mouse position and sets its size
            Vector2 mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            this.position = new Rect(mousePosition.x, mousePosition.y, width, height);
            this.maxSize = new Vector2(width, height);
            this.minSize = new Vector2(width, height);
            windowWidth = width;
            windowHight = height;

            // Calculates the width of the icons
            iconWidth = windowWidth - IconSpace * 2 - BoxSpace * 2 - SliderSpace;

            targetContainer = field;

            inspectorWindow = yourWindow;
        }

        /// <summary>
        /// Initializes the Windows.
        /// </summary>
        /// <param name="field">The container which will be filled with the selected prefab.</param>
        /// <param name="directory">The directory name.</param>
        /// <param name="extension">The file extension type.</param>
        /// <param name="yourWindow">Reference of the editor that creates a call for this constructor.</param>
        public void WindowsInitialize(ObjectContainer field, string directory, string extension, Editor yourWindow)
        {
            WindowsInitialize(field, directory, extension, yourWindow, 100, 300);
        }

        private void OnGUI()
        {
            RefreshIcon();
            DrawIconScroll();
        }

        /// <summary>
        /// Draws asset icons in a scroll view.
        /// </summary>
        private void DrawIconScroll()
        {
            // Background box.
            GUI.Box(new Rect(BoxSpace, BoxSpace, windowWidth - BoxSpace * 2, windowHight - BoxSpace * 2), 
                GUIContent.none, 
                EditorStyles.textArea);

            currentScrollPosition = GUI.BeginScrollView(new Rect(BoxSpace + IconSpace, 
                BoxSpace + IconSpace, 
                iconWidth + SliderSpace, 
                windowHight - (BoxSpace + IconSpace) * 2), 
                currentScrollPosition, 
                new Rect(0, 0, iconWidth, icons.Count * iconWidth));

            for (int i = 0; i < icons.Count; i++)
            {
                // Changes the icon id if it's selected.
                if (i == IconID)
                {
                    GUI.Toggle(new Rect(0, i * iconWidth, iconWidth, iconWidth), true, icons[i].Preview, GUI.skin.button);
                }
                else if (GUI.Toggle(new Rect(0, i * iconWidth, iconWidth, iconWidth), false, icons[i].Preview, GUI.skin.button))
                {
                    IconID = i;
                }
            }

            GUI.EndScrollView();
        }

        void Refresh(string directory, string extension)
        {
            icons.Clear();

            IEnumerable<string> targetPaths = GetFilesUnderDirectory(directory, extension);

            foreach (string path in targetPaths)
            {

                string pathFromAssets = RemoveAssetsFromPath(path);

                Object prefab = AssetDatabase.LoadAssetAtPath<Object>(path);

                EditorUtility.SetDirty(prefab);

                Texture2D previewImage = AssetPreview.GetAssetPreview(prefab);

                icons.Add(new AssetToggle(pathFromAssets, prefab, new GUIContent(previewImage)));
            }
        }

        private void RefreshIcon()
        {
            foreach (AssetToggle assetToggle in icons)
            {
                assetToggle.Preview.image = AssetPreview.GetAssetPreview(assetToggle.Pref);
            }
        }

        /// <summary>
        /// get the paths for all files that are lying under the same directory folder.
        /// </summary>
        /// <param name="directory">the name of the common folder</param>
        /// <param name="extension">the extension required to filter the result</param>
        /// <returns></returns>
        private static string[] GetFilesUnderDirectory(string directory, string extension)
        {
            string[] targetDirectory = Directory.GetDirectories(@"Assets/", directory, SearchOption.AllDirectories);

            if (targetDirectory.Length == 0)
            {
                Debug.Log("PickByIcon.cs: Please move your prefab under folder named ( " + directory + " )");

                return targetDirectory;
            }

            List<string> targetFiles = new List<string>();

            foreach (string s in targetDirectory)
            {
                targetFiles.AddRange(Directory.GetFiles(s, "*." + extension, SearchOption.TopDirectoryOnly));
            }

            if (targetFiles.Count == 0)
            {
                Debug.Log("PickByIcon.cs: No prefabs where found under ( " + directory + " ), " +
                    "make sure that they have the following extension ( ." + extension + " )");
            }

            return targetFiles.ToArray();

        }

        /// <summary>
        /// Removes the assets directory from a given path,
        /// making it local to the assets folder.
        /// </summary>
        /// <param name="path">The path that needs to change.</param>
        /// <returns>The path after editing</returns>
        static string RemoveAssetsFromPath(string path)
        {
            path = path.Substring("Assets".Length).Replace("\\", "/");
            return path;
        }
    }
}