using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Script for drawing and manipulating the points that make up a bezier path of a curve mover.
    /// </summary>
    [CustomEditor(typeof(CurveMover))]
    public class CurveMoverEditor : EditorTools
    {
        private CurveMover curveMover;
        private Path path;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Delete Last Segment"))
            {
                Undo.RecordObject(curveMover, "Delete Last Segment");
                path.RemoveLastSegment();
                SceneView.RepaintAll();
            }
        }

        protected override void OnEnable()
        {
            curveMover = (CurveMover)target;
            go = curveMover.gameObject;

            if (curveMover.CurvePath == null)
            {
                curveMover.CurvePath = new Path();
            }

            path = curveMover.CurvePath;

            base.OnEnable();
        }

        public void OnSceneGUI()
        {
            Draw();
            ProcessInput();
        }

        private void Draw()
        {
            DrawBezier();
            DrawControlPoints();            
        }

        private void DrawBezier()
        {
            for (int i = 0; i < path.NumOfSegments; i++)
            {
                Vector2[] segmentPoints = path.GetControlPointsInSegment(i);

                Handles.DrawBezier(segmentPoints[0], segmentPoints[3], segmentPoints[1], segmentPoints[2], pathColor, null, 3f);

                DrawTangents(segmentPoints);
            }
        }

        private void DrawTangents(Vector2[] segmentPoints)
        {
            Handles.color = pathAlternativeColor;
            Handles.DrawDottedLine(segmentPoints[0], segmentPoints[1], dottedLineSpace);
            Handles.DrawDottedLine(segmentPoints[2], segmentPoints[3], dottedLineSpace);
        }

        private void DrawControlPoints()
        {
            Handles.color = circleHandleColor;

            for (int i = 0; i < path.ControlPoints.Count; i++)
            {
                Vector2 pos = Handles.FreeMoveHandle(path.ControlPoints[i], Quaternion.identity, handleSize.floatValue, Vector2.zero, DoubleCircleHandleCap);

                DrawLabel(i, Math2D.Vector2ToVector3(pos));

                if (pos != path.ControlPoints[i])
                {
                    Undo.RecordObject(curveMover, "Move Control Point");
                    path.MovePoint(i, pos);
                }
            }
        }

        private void ProcessInput()
        {
            Event guiEvent = Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                Undo.RecordObject(curveMover, "Add Segment");
                path.AddSegment(mousePos);
            }
        }
    }
}   