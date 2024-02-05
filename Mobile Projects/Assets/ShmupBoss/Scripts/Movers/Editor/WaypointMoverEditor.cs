using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Script for drawing and manipulating the points that make up a path of a waypoint mover.
    /// </summary>
    [CustomEditor(typeof(WaypointMover))]
    public class WaypointMoverEditor : EditorTools
    {
        private WaypointMover waypointMover;

        private SerializedProperty mode;
        private SerializedProperty waypoints;
        private SerializedProperty loopBackToPoint;

        private Vector3 prePosition;
             
        protected override void OnEnable()
        {
            waypointMover = (WaypointMover)target;
            go = waypointMover.gameObject;

            mode = serializedObject.FindProperty("mode");
            loopBackToPoint = serializedObject.FindProperty("loopBackToPoint");
            waypoints = serializedObject.FindProperty("waypoints");

            base.OnEnable();
        }

        private void OnSceneGUI()
        {
            serializedObject.Update();

            DrawWaypointPath();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawWaypointPath()
        {
            DrawClosedLoopLine();

            for (int i = 0; i < waypoints.arraySize; i++)
            {
                Vector3 currentPosition = GetPointPosition(i);

                Handles.color = GetPathColor(i);

                if(i != 0)
                {
                    Handles.DrawLine(prePosition, currentPosition);
                }

                prePosition = currentPosition;
                DrawLabel(i, currentPosition);

                currentPosition = DrawCircleHandle(currentPosition);
                EditPoint(i, currentPosition);
            }           
        }

        /// <summary>
        /// Draws the line that closes the loop.
        /// </summary>
        private void DrawClosedLoopLine()
        {
            Color prevHandleColor = Handles.color;

            if ((int)WaypointMoverMode.ContinueLoop == mode.enumValueIndex && waypoints.arraySize > 1)
            {
                Vector3 lastPos = GetPointPosition(waypoints.arraySize - 1);

                Vector3 firstLoopPos = GetPointPosition(loopBackToPoint.intValue);

                Handles.color = pathAlternativeColor;

                Handles.DrawDottedLine(firstLoopPos, lastPos, dottedLineSpace);
            }

            Handles.color = prevHandleColor;
        }

        /// <summary>
        /// Returns the waypoint path position at the indexed point.
        /// </summary>
        /// <param name="i">The index of the path point you wish to retrieve its location.</param>
        /// <returns></returns>
        private Vector3 GetPointPosition(int i)
        {
            SerializedProperty waypoint = waypoints.GetArrayElementAtIndex(i);
            SerializedProperty waypointPosition = waypoint.FindPropertyRelative("Position");

            return Math2D.Vector2ToVector3(waypointPosition.vector2Value, goTransform.position.z);
        }

        /// <summary>
        /// Will retrive the path color at a particular path point in order to draw the waypoint 
        /// point with the same color.
        /// </summary>
        /// <param name="index">The index of the path point you want to find the color of the path 
        /// segment near.</param>
        /// <returns></returns>
        private Color GetPathColor(int index)
        {
            if(mode.enumValueIndex == (int)WaypointMoverMode.Stop)
            {
                return pathColor;
            }
            else
            {
                if(index > loopBackToPoint.intValue)
                {
                    return pathAlternativeColor;
                }
                else
                {
                    return pathColor;
                }
            }
        }

        private Vector3 DrawCircleHandle(Vector3 currentPosition)
        {
            Color previousHandleColor = Handles.color;
            Handles.color = circleHandleColor;

            currentPosition = Handles.FreeMoveHandle(currentPosition, 
                Quaternion.identity, 
                handleSize.floatValue, 
                Vector3.one * handleSnap, 
                DoubleCircleHandleCap);

            Handles.color = previousHandleColor;

            return currentPosition;
        }

        /// <summary>
        /// Changes the position of a waypoint path point.
        /// </summary>
        /// <param name="i">The index of the path waypoint you wish to change its position.</param>
        /// <param name="currentPosition">The new position the point will have.</param>
        private void EditPoint(int i, Vector3 currentPosition)
        {
            SerializedProperty waypoint = waypoints.GetArrayElementAtIndex(i);
            SerializedProperty waypointPosition = waypoint.FindPropertyRelative("Position");

            waypointPosition.vector2Value = (Vector2)currentPosition;
        }
    }
}