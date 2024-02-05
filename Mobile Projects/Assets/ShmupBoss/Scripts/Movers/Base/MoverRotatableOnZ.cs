using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for any movers that have an edit rotation method on the Z axis.<br></br>
    /// (This includes: curve mover, any mover tracking class and simple movers.)
    /// </summary>
    public abstract class MoverRotatableOnZ : Mover, IRotatable
    {
        /// <summary>
        /// If set to true, this mover will rotate the game object it is moving, this is applied to the Z axis.
        /// </summary>
        [Tooltip("If set to true, this mover will rotate the game object it is moving, this is applied to the Z axis.")]
        [SerializeField]
        protected bool isFollowingDirection;

        protected Quaternion startRotation;

        /// <summary>
        /// The default facing angle of an enemy agent using any mover which inherits from "MoverRotatableOnZ"
        /// </summary>
        public virtual float FacingAngle
        {
            get
            {
                return FacingAngles.NonPlayer;
            }
        }

        /// <summary>
        /// Caches speed, adds on mover state change event and prepares the mover for following direction.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (isFollowingDirection)
            {
                startRotation = transform.localRotation;
                SubscribeToRotationManagerOnZ();
            }           
        }

        /// <summary>
        /// Resets the local rotation after it has been changed by edit rotation.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (isFollowingDirection)
            {
                transform.localRotation = startRotation;
            }
        }

        /// <summary>
        /// Adds a rotation manager componenet which will control the rotation of the mover on
        /// the Z axis by the "EditRotation" method which can be overridden and customized.
        /// </summary>
        protected virtual void SubscribeToRotationManagerOnZ()
        {
            RotationManager rotationManager = GetComponent<RotationManager>();

            if (rotationManager == null)
            {
                rotationManager = gameObject.AddComponent<RotationManager>();
            }

            rotationManager.FacingAngle = FacingAngle;
            rotationManager.Subscribe(this, Axis.Z);
        }

        /// <summary>
        /// This method will control the rotation on the Z axis.
        /// </summary>
        /// <param name="angle">The current rotation angle on the Z axis which will be processed by this method</param>
        /// <returns>The angle rotation on the Z axis after it has been processed.</returns>
        public virtual float EditRotation(float angle)
        {
            return Math2D.VectorToDegree(CurrentDirection);
        }
    }
}