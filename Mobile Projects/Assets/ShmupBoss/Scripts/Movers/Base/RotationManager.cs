using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Used to rotate the game object in a particular axis order depending on the level type and to 
    /// make sure only one componenet is causing it to rotate it on an axis.
    /// </summary>
    public class RotationManager : MonoBehaviour
    {
        private Quaternion startRotation;

        /// <summary>
        /// The method which will rotate the game object on the X axis.
        /// </summary>
        private IRotatable rotatorOnX;

        /// <summary>
        /// The method which will rotate the game object on the Y axis.
        /// </summary>
        private IRotatable rotatorOnY;

        /// <summary>
        /// The method which will rotate the game object on the Z axis.
        /// </summary>
        private IRotatable rotatorOnZ;

        public float ChangeInXRotation 
        { 
            get; 
            private set; 
        }

        public float ChangeInYRotation 
        { 
            get; 
            private set; 
        }

        public float ChangeInZRotation 
        { 
            get; 
            private set; 
        }


        public float FacingAngle 
        { 
            get; 
            set; 
        }

        private void Awake()
        {
            startRotation = transform.rotation;
        }

        private void Update()
        {
            transform.rotation = startRotation;

            if (Level.IsVertical)
            {
                RotateOnX();
                RotateOnY();
            }
            else if (Level.IsHorizontal)
            {
                RotateOnY();
                RotateOnX();
            }

            RotateOnZ();
        }

        private void RotateOnX()
        {
            if (rotatorOnX == null)
            {
                return;
            }

            ChangeInXRotation = rotatorOnX.EditRotation(ChangeInXRotation);
            transform.RotateAround(transform.position, Vector3.right, ChangeInXRotation);
        }

        private void RotateOnY()
        {
            if (rotatorOnY == null)
            {
                return;
            }

            ChangeInYRotation = rotatorOnY.EditRotation(ChangeInYRotation);
            transform.RotateAround(transform.position, Vector3.up, ChangeInYRotation);
        }

        private void RotateOnZ()
        {
            if (rotatorOnZ == null)
            {
                return;
            }

            ChangeInZRotation = rotatorOnZ.EditRotation(ChangeInZRotation);

            // The Z rotation method takes into consideration the facing angle, which means it will preserve the 
            // original orientation. For example enemies used in a vertical level are created pointed downward
            // When this method rotates the enemy it will first assume and apply that direction, for enemies
            // used in a horizontal level, they are assumed to be rotated leftward, etc..
            transform.RotateAround(transform.position, Vector3.forward, ChangeInZRotation - FacingAngle);
        }

        /// <summary>
        /// Subscribes the method that will rotate the game object on one of the axis.
        /// </summary>
        /// <param name="rotator">The instance with the edit rotation method.</param>
        /// <param name="axis">The axis it will modify</param>
        public void Subscribe(IRotatable rotator, Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    rotatorOnX = rotator;
                    break;
                        
                case Axis.Y:
                    rotatorOnY = rotator;
                    break;

                case Axis.Z:
                    rotatorOnZ = rotator;
                    ChangeInZRotation = FacingAngle;
                    break;
            }

            return;
        }
    }
}