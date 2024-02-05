using UnityEngine;

namespace ShmupBossEditor
{
    /// <summary>
    /// The object that represents the assets in the PickByIcon window
    /// </summary>
    public class AssetToggle
    {
        /// <summary>
        /// the assets path relative to the assets folder
        /// </summary>
        public string Path;

        /// <summary>
        /// the assets Object.
        /// </summary>
        public Object Pref;

        /// <summary>
        /// the assets GUI
        /// </summary>
        public GUIContent Preview;

        /// <summary>
        /// AssetToggle constructor
        /// </summary>
        /// <param name="path">the assets path relative to the assets folder</param>
        /// <param name="pref">the assets Object.</param>
        /// <param name="preview">he assets GUI.</param>
        public AssetToggle(string path, Object pref, GUIContent preview)
        {
            Path = path;
            Pref = pref;
            Preview = preview;
        }
    }
}