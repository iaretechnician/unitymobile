using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// <para>A bezier curve that is made up of control points and is divided into segments.<br></br>
    /// Each segment includes a starting and ending point and a starting and ending tangent, these segments 
    /// added up together constitute a bezier curve.</para><br></br>
    /// You can think of this bezier curve as one of the following three:<br></br>
    /// -A curve that is made up of control points (These control points include tangent points.)<br></br>
    /// -Or a curve made up of segments, each segment is made up of 4 control points.<br></br>
    /// -Or a curve made up of bezier control points, these bezier control points contain tangent 
    /// and distance information and always lie on the curve itself.
    /// </summary>
    [System.Serializable]
    public class Path
    {
        /// <summary>
        /// Control points which make up the path.
        /// </summary>
        /// <param name="i">The index of the control point in the control points array.</param>
        /// <returns>The indexed control point of this path.</returns>
        public Vector2 this[int i]
        {
            get
            {
                return ControlPoints[i];
            }
            set
            {
                ControlPoints[i] = value;
            }
        }

        /// <summary>
        /// All the control points that make up this path.<br></br>
        /// This includes both the acnhor (start/end) control points and their corresponding tangent or intermediate 
        /// points which may or may not lie on the curve.
        /// </summary>
        [Tooltip("All the control points that make up this path. This includes both the acnhor (start/end) control " +
            "points and their corresponding tangent or intermediate points which may or may not lie on the curve.")]
        public List<Vector2> ControlPoints;

        public int LastControlPoint
        {
            get
            {
                return ControlPoints.Count - 1;
            }
        }

        public int NumOfSegments
        {
            get
            {
                return ControlPoints.Count / 3;
            }
        }

        /// <summary>
        /// The curve points that actually lie on the curve. This excludes any tanget points.
        /// </summary>
        [HideInInspector]
        public List<BezierCurvePoint> CurvePoints;

        public int LastCurvePoint
        {
            get
            {
                return CurvePoints.Count - 1;
            }
        }

        #region Constructor
        public Path()
        {
            if (Level.IsVertical)
            {
                CreatePathForVerticalLevel();
            }
            else if (Level.IsHorizontal)
            {
                CreatePathForHorizontalLevel();
            }
            else
            {
                CreatePath();
            }
        }

        private void CreatePathForVerticalLevel()
        {
            ControlPoints = new List<Vector2>
                    {
                    Vector2.up * ProjectConstants.PathConstructorScale,
                    (Vector2.left + Vector2.up) * ProjectConstants.PathConstructorScale * 0.5f,
                    (Vector2.right + Vector2.down) * ProjectConstants.PathConstructorScale * 0.5f,
                    Vector2.down * ProjectConstants.PathConstructorScale
                    };
        }

        private void CreatePathForHorizontalLevel()
        {
            ControlPoints = new List<Vector2>
                    {
                    Vector2.left * ProjectConstants.PathConstructorScale,
                    (Vector2.left + Vector2.up) * ProjectConstants.PathConstructorScale * 0.5f,
                    (Vector2.right + Vector2.down) * ProjectConstants.PathConstructorScale * 0.5f,
                    Vector2.right * ProjectConstants.PathConstructorScale
                    };
        }

        private void CreatePath()
        {
            ControlPoints = new List<Vector2>
                    {
                    Vector2.left,
                    (Vector2.left + Vector2.up) * 0.5f,
                    (Vector2.right + Vector2.down) * 0.5f,
                    Vector2.right
                    };
        }
        #endregion

        #region Methods for manipulating the control points on the path.
        /// <summary>
        /// Returns the 4 segment control points which include a starting anchor point, a starting 
        /// tangent point, an ending tangent point and an ending anchor point.
        /// </summary>
        /// <param name="segmentIndex">The index of the segment you wish to extract its control points.</param>
        /// <returns>The control points which make up this segment.</returns>
        public Vector2[] GetControlPointsInSegment(int segmentIndex)
        {
            return new Vector2[] { ControlPoints[segmentIndex * 3],
                ControlPoints[segmentIndex * 3 + 1],
                ControlPoints[segmentIndex * 3 + 2],
                ControlPoints[segmentIndex * 3 + 3]};
        }

        /// <summary>
        /// Adds an anchor point and all needed tangents which are interpolated from the anchor points.
        /// </summary>
        /// <param name="pos">The position of the ending anchor control point.</param>
        public void AddSegment(Vector2 pos)
        {
            // Adds a tangent point after the ending control point of the previous segment.
            ControlPoints.Add((ControlPoints[LastControlPoint] * 2f) - ControlPoints[LastControlPoint - 1]);

            // Adds the tangent point before the ending control point of the new segment.
            ControlPoints.Add((ControlPoints[LastControlPoint] + pos) * 0.5f);

            // Adds the ending control point of the new segment.
            ControlPoints.Add(pos);
        }

        /// <summary>
        /// Deletes the last segment in the curve by removing the last 3 control points.
        /// </summary>
        public void RemoveLastSegment()
        {
            int controlPointsListLength = ControlPoints.Count;

            if(controlPointsListLength < 7)
            {
                return;
            }

            ControlPoints.RemoveAt(controlPointsListLength - 1);
            ControlPoints.RemoveAt(controlPointsListLength - 2);
            ControlPoints.RemoveAt(controlPointsListLength - 3);          
        }

        /// <summary>
        /// Moves a control point on a curve. Works for both acnhor control points and tangent control points.
        /// </summary>
        /// <param name="i">The control point to move.</param>
        /// <param name="pos">The new position the control point will be moved to.</param>
        public void MovePoint(int i, Vector2 pos)
        {
            // How much the control point will be moved, this will also be used to move the points after and before it.
            Vector2 controlPointDisplacement = pos - ControlPoints[i];

            // Move the targeted control point.
            ControlPoints[i] = pos;

            // If the moved control point index is an acnhor control point.
            if (i % 3 == 0)
            {
                int tangentAfter = i + 1;
                int tangentBefore = i - 1;

                // If this is the first path point only change the tangent after.
                if (i == 0)
                {
                    ControlPoints[tangentAfter] += controlPointDisplacement;
                }
                // If this is the last path point only change the tangent before.
                else if (i == LastControlPoint)
                {
                    ControlPoints[tangentBefore] += controlPointDisplacement;
                }
                // If this a middle curve point then you can change beoth tangents.
                else
                {
                    ControlPoints[tangentAfter] += controlPointDisplacement;
                    ControlPoints[tangentBefore] += controlPointDisplacement;
                }
            }
            // If the moved control point index is a tanget control point.
            else
            {
                bool isNextPointAnchor = (i + 1) % 3 == 0;

                // If the next point is an anchor it will find the corresponding tangent in the point right after 
                // the anchor, but if the next point is a tangent, then it will find the tangent two points before.
                // Simply put, after a tangent point is selected, it will determine which end of the tangent is selected 
                // by seeing if the next point is an anchor or a tangent. Same goes to find the related anchor point.
                int correspondingTangent = isNextPointAnchor ? i + 2 : i - 2;
                int anchor = isNextPointAnchor ? i + 1 : i - 1;

                // The following lines ensure that when the corresponding tangent is moved, it keeps 
                // its length.
                if (correspondingTangent > 0 && correspondingTangent < LastControlPoint)
                {
                    float correspondingTangentLength = Vector2.Distance(
                        ControlPoints[anchor],
                        ControlPoints[correspondingTangent]);

                    Vector2 movementDirection = (ControlPoints[anchor] - pos).normalized;

                    ControlPoints[correspondingTangent] = ControlPoints[anchor] + (movementDirection * correspondingTangentLength);
                }
            }
        }
        #endregion

        #region Methods for preparing and setting up curve points.
        public void PrepareCurvePoints()
        {
            CurvePoints.Clear();
            CreateBezierCurvePoints();
            CalculateCurvePointDistances();
            CreateTangents();
        }

        /// <summary>
        /// Creates the bezier curve points based on the control points list.
        /// </summary>
        public void CreateBezierCurvePoints()
        {
            for (int i = 0; i < NumOfSegments; i++)
            {
                Vector2[] segmentPoints = GetControlPointsInSegment(i);

                for (float t = 0.0f; t < 1.0f; t += ProjectConstants.PathPointsSampleTolerance)
                {
                    Vector2 curvePointPos = BezierTools.EvaluateCubic(segmentPoints[0],
                        segmentPoints[1],
                        segmentPoints[2],
                        segmentPoints[3],
                        t
                        );

                    BezierCurvePoint curvePoint = new BezierCurvePoint(curvePointPos);
                    CurvePoints.Add(curvePoint);
                }
            }
        }

        /// <summary>
        /// Assigns the path distance for each curve point on it.
        /// </summary>
        public void CalculateCurvePointDistances()
        {
            // The starting curve point distance at the beginning of the curve is zero.
            CurvePoints[0].Dist = 0f;

            float DistanceFromPathStart = 0f;

            for (int i = 1; i < CurvePoints.Count; i++)
            {
                // Adds up all the distances between each curve point and the one before it.
                // The distance of the last curve point on the curve is approximately the entire curve length.
                DistanceFromPathStart += Vector2.Distance(CurvePoints[i - 1].Pos, CurvePoints[i].Pos);
                CurvePoints[i].Dist = DistanceFromPathStart;
            }
        }

        /// <summary>
        /// Assigns tangents to each curve point on the path.
        /// </summary>
        public void CreateTangents()
        {
            CreateStartAndEndTangents();

            // Calculates tangents by using the vector between the points before and after each curve point.
            for (int i = 1; i < CurvePoints.Count - 1; i++)
            {
                CurvePoints[i].Tangent = (CurvePoints[i + 1].Pos - CurvePoints[i - 1].Pos).normalized;
            }
        }

        /// <summary>
        /// These had to be created in a special method because there is either
        /// no curve points before or after them to find the average like the other 
        /// intermediate curve points.
        /// </summary>
        private void CreateStartAndEndTangents()
        {
            CurvePoints[0].Tangent = CurvePoints[1].Pos - CurvePoints[0].Pos;
            CurvePoints[CurvePoints.Count - 1].Tangent = CurvePoints[CurvePoints.Count - 1].Pos - CurvePoints[CurvePoints.Count - 2].Pos;
        }
        #endregion

        #region Methods for interpolating and finding the position and tangents on path.
        public Vector2 FindPosition(float distanceTraveled)
        {
            //Goes through all the curve points trying to find a matching distance to the distance traveled.
            for (int i = 0; i < LastCurvePoint; i++)
            {
                if (distanceTraveled >= CurvePoints[i].Dist && distanceTraveled < CurvePoints[i + 1].Dist)
                {
                    float t = FindPercentage(distanceTraveled, i);

                    return Vector2.Lerp(CurvePoints[i].Pos, CurvePoints[i + 1].Pos, t);
                }
            }

            Debug.Log("Path.cs: Could not find position on path.");
            return Vector2.zero;
        }

        public Vector2 FindTangent(float distanceTraveled)
        {
            //Goes through all the curve points trying to find a matching distance to the distance traveled.
            for (int i = 0; i < LastCurvePoint; i++)
            {
                if (distanceTraveled >= CurvePoints[i].Dist && distanceTraveled < CurvePoints[i + 1].Dist)
                {
                    float t = FindPercentage(distanceTraveled, i);

                    return Vector2.Lerp(CurvePoints[i].Tangent, CurvePoints[i + 1].Tangent, t);
                }
            }

            Debug.Log("Path.cs: Could not find tangent on path.");

            return Vector2.zero;
        }

        /// <summary>
        /// Finds the percentage of where a point is located between 2 curve points.
        /// </summary>
        /// <param name="distanceTraveled">Where the point is located on the curve.</param>
        /// <param name="i">The index of the curve point the point is located at or after.</param>
        /// <returns>The percentage between 2 curve points.</returns>
        private float FindPercentage(float distanceTraveled, int i)
        {
            int first = i;
            int second = i + 1;

            float distanceFromFirstCurvePoint = distanceTraveled - CurvePoints[first].Dist;
            float distanceBetweenFirstAndSecondCurvePoint = CurvePoints[second].Dist - CurvePoints[first].Dist;
            float percentageBetweenFirstAndSecondCurvePoint = distanceFromFirstCurvePoint / distanceBetweenFirstAndSecondCurvePoint;
            return percentageBetweenFirstAndSecondCurvePoint;
        }
        #endregion
    }
}