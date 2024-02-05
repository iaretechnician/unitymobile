using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Moves the game object in uniform direction based on the spawn side, it will also rotate it in 
    /// accordance to the rotation speed values.<br></br>
    /// The speed of this object is also affected by the treadmill speed multiplier.<br></br>
    /// This script is used with the background objects spawner to provide simple movement and rotation 
    /// for the spawned objects and is not intended to be used separately for enemies, for that you could 
    /// use the simple mover and the simple rotator.
    /// </summary>
    public class MoverRotator : MonoBehaviour
    {
        /// <summary>
        /// The rect side the game object connected to the component is spawned from.
        /// </summary>
        public RectSide SpawnSide
        {
            get;
            set;
        }
        
        public float Speed
        {
            get;
            set;
        }

        public float XRotationSpeed
        {
            get;
            set;
        }

        public float YRotationSpeed
        {
            get;
            set;
        }

        public float ZRotationSpeed
        {
            get;
            set;
        }

        private void Update()
        {
            RotateOnAllAxis();
            transform.position += FindDeltaDisplacement();
        }

        private void RotateOnAllAxis()
        {
            transform.Rotate(Vector3.right, XRotationSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up, YRotationSpeed * Time.deltaTime);
            transform.Rotate(Vector3.forward, ZRotationSpeed * Time.deltaTime);
        }

        private Vector3 FindDeltaDisplacement()
        {
            float multipliedSpeed = FindSpeedMultipliedByTreadmillSpeed();

            switch (SpawnSide)
            {
                case RectSide.Top:
                    return Vector3.down * multipliedSpeed * Time.deltaTime;

                case RectSide.Bottom:
                    return Vector3.up * multipliedSpeed * Time.deltaTime;

                case RectSide.Right:
                    return Vector3.left * multipliedSpeed * Time.deltaTime;

                case RectSide.Left:
                    return Vector3.right * multipliedSpeed * Time.deltaTime;

                default:
                    return Vector3.zero;
            }          
        }

        private float FindSpeedMultipliedByTreadmillSpeed()
        {
            float treadmillSpeedMultiplier = 1.0f;

            if (Treadmill.Instance != null)
            {
                treadmillSpeedMultiplier = Treadmill.Instance.SpeedMultiplier;
            }

            return (Speed * treadmillSpeedMultiplier);
        }
    }
}