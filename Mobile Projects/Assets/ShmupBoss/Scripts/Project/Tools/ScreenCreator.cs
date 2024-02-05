using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// Responsible for creating a color/image screen that should hide any empty spaces in the canvas if 
    /// you are using a vertical level in desktop and are forcing the aspect ratio.
    /// </summary>
    public static class ScreenCreator
    {
        /// <summary>
        /// Creates a black screen canvas that fills the view.
        /// </summary>
        public static void CreateScreen()
        {
            GameObject blackScreen = new GameObject("----- Black Screen -----");

            Canvas blackScreenCanavas = blackScreen.AddComponent<Canvas>();
            blackScreenCanavas.renderMode = RenderMode.ScreenSpaceOverlay;

            RectTransform blackScreenRect = blackScreen.GetComponent<RectTransform>();
            blackScreenRect.offsetMin = Vector2.zero;
            blackScreenRect.offsetMax = Vector2.one;

            blackScreen.AddComponent<Image>().color = Color.black;
        }

        /// <summary>
        /// Creates a color screen canvas that fills the view.
        /// </summary>
        /// <param name="color">The color you want the screen to have.</param>
        public static void CreateScreen(Color color)
        {
            GameObject colorScreen = new GameObject("----- Color Screen -----");

            Canvas blackScreenCanavas = colorScreen.AddComponent<Canvas>();
            blackScreenCanavas.renderMode = RenderMode.ScreenSpaceOverlay;

            RectTransform blackScreenRect = colorScreen.GetComponent<RectTransform>();
            blackScreenRect.offsetMin = Vector2.zero;
            blackScreenRect.offsetMax = Vector2.one;

            colorScreen.AddComponent<Image>().color = color;
        }

        /// <summary>
        /// Creates an image screen canvas that fills the view.
        /// </summary>
        /// <param name="sprite">The image you would like to be used as a background for the canvas.</param>
        public static void CreateScreen(Sprite sprite)
        {
            GameObject imageScreen = new GameObject("----- Image Screen -----");

            Canvas blackScreenCanavas = imageScreen.AddComponent<Canvas>();
            blackScreenCanavas.renderMode = RenderMode.ScreenSpaceOverlay;

            RectTransform blackScreenRect = imageScreen.GetComponent<RectTransform>();
            blackScreenRect.offsetMin = Vector2.zero;
            blackScreenRect.offsetMax = Vector2.one;

            imageScreen.AddComponent<Image>().sprite = sprite;
        }
    }
}