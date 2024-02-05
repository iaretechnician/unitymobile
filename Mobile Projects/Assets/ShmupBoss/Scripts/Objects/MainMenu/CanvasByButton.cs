using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// A canvas which will be activated when a button is pressed.
    /// </summary>
    [System.Serializable]
    public class CanvasByButton
    {
        [Tooltip("This canvas name is only used for organizational purposes in the editor.")]
        public string CanvasName;

        /// <summary>
        /// The canvas which will be activated when the button is pressed.
        /// </summary>
        [Tooltip("The canvas which will be activated when the button is pressed.")]
        public RectTransform Canvas;

        /// <summary>
        /// The button which will activate the canvas when pressed.
        /// </summary>
        [Tooltip("The button which will activate the canvas when pressed.")]
        public Button ActivatingButton;

        /// <summary>
        /// The button which will be first selected when the cavnas is activated.
        /// </summary>
        [Tooltip("The button which will be first selected when the canvas is activated.")]
        public Button FirstSelectedButton;
    }
}