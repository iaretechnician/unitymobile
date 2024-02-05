using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Draws the help box atribute which shows a box in the inspector
    /// containing a message above any field which implement the help box atribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(SB_HelpBox))]
    public class HelpBoxAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Returns the selected attribute.
        /// </summary>
        private SB_HelpBox SelectedHelpBox
        {
            get
            {
                return (SB_HelpBox)attribute;
            }
        }

        /// <summary>
        /// Draws the help box and the field in the inspector.
        /// </summary>
        /// <param name="rect">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Rect helpBoxRect = rect;

            // Shifts the help box down by the margins.
            helpBoxRect.y += ProjectConstants.HelpBoxMargins;

            helpBoxRect.height = GetHelpBoXHeight(SelectedHelpBox.Message);

            // Draws the help box.
            EditorGUI.HelpBox(helpBoxRect, SelectedHelpBox.Message, UnityEditor.MessageType.Info);

            // Shifts the field beneath the help box to create a margin.
            rect.y += helpBoxRect.height + ProjectConstants.HelpBoxMargins * 2;

            // Removes the help box height from the field height.
            rect.height -= (helpBoxRect.height + ProjectConstants.HelpBoxMargins * 2);

            // Draws the field.
            EditorGUI.PropertyField(rect, property, new GUIContent(property.name));
        }

        /// <summary>
        /// return the height for the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>the height for this property in pixel.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + GetHelpBoXHeight(SelectedHelpBox.Message) + ProjectConstants.HelpBoxMargins * 2;
        }

        /// <summary>
        /// Calculates the height for the EditorGUI.HelpBox based on the message contained within.
        /// </summary>
        /// <param name="message">The help box message.</param>
        /// <returns>The height of the help box.</returns>
        protected virtual float GetHelpBoXHeight(string message)
        {
            GUIStyle style = new GUIStyle(EditorStyles.helpBox);
            return style.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth - ProjectConstants.HelpBoxIconWidth);
        }
    }
}