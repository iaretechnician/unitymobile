using UnityEngine;
using UnityEditor;

namespace ShmupBossEditor
{
    /// <summary>
    /// Base class for editor scripts which draw gizmos for points.
    /// </summary>
    public class EditorTools : Editor
    {
        protected SerializedProperty handleSize;

        protected const float handleSnap = 0.5f;
        protected const float labelPositionOffset = 1.0f;

        protected const float dottedLineSpace = 5.0f;

        protected readonly Color circleHandleColor = Color.red;
        protected readonly Color pathColor = Color.yellow;
        protected readonly Color pathAlternativeColor = Color.cyan;

        protected GameObject go;
        protected Transform goTransform;

        protected virtual void OnEnable()
        {
            goTransform = go.transform;
            handleSize = serializedObject.FindProperty("HandleSize");
        }

        /// <summary>
        /// Draws an index number near a point (position in 3D space).
        /// </summary>
        /// <param name="index">The index number to draw.</param>
        /// <param name="currentPosition">The position which the index will be drawn next to.</param>
        protected void DrawLabel(int index, Vector3 currentPosition)
        {
            Handles.Label(currentPosition - Vector3.one * handleSize.floatValue * labelPositionOffset, (index).ToString());
        }

        /// <summary>
        /// Draws a double circle handle.
        /// </summary>
        /// <param name="controlID">The control ID for the handle.</param>
        /// <param name="position">The position of the handle.</param>
        /// <param name="rotation">The rotation of the handle.</param>
        /// <param name="size">The size of the handle.</param>
        /// <param name="eventType">Event type for the handle to act upon.</param>
        public static void DoubleCircleHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
            Handles.CircleHandleCap(controlID, position, rotation, size * 0.5f, eventType);
        }
    }
}