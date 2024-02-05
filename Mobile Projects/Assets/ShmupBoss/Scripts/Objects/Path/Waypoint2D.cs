using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Position in 2D space with a time value associated with it.
    /// </summary>
    [System.Serializable]
    public class Waypoint2D
    {
        public float WaitTime;

        public Vector2 Position;

        public Vector3 Position3D
        {
            get
            {
                return Math2D.Vector2ToVector3(Position);
            }
        }

        public Waypoint2D(Vector2 position, float waitTime)
        {
            Position = position;
            WaitTime = waitTime;
        }
    }
}