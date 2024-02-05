using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Attaches a game object to a target transform. Can be useful if you want for example to link a 
    /// spawned FX to the object that spawned it.
    /// </summary>
    public class ConstrainedPoint : MonoBehaviour
    {
        /// <summary>
        /// The target transform you wish to link the object with this component to.
        /// </summary>
        [Tooltip("The target transform you wish to link the object with this component to.")]
        public Transform Target;

        /// <summary>
        /// Will sanp the position of the object with this component to that of its target, otherwise it will 
        /// keep it's initial position and be linked to its target. This will only be used if the displacement
        /// from target is not set later on by anohther instance.
        /// </summary>
        [Header("Additional Settings")]
        [Tooltip("Will sanp the position of the object with this component to that of its target, otherwise it " +
            "will keep it's initial position and be linked to its target. This will only be used if the displacement " +
            "from target is not set later on by anohther instance.")]
        public bool SnapToTargetPosition = true;

        [HideInInspector]
        public Vector3 DisplacementFromTarget;

        void Awake()
        {
            if(Target != null)
            {
                if (SnapToTargetPosition)
                {
                    DisplacementFromTarget = Vector3.zero;
                }
                else
                {
                    DisplacementFromTarget = transform.position - Target.position;
                }
            }      
        }

        void Update()
        {
            if (Target != null)
            {
                transform.position = DisplacementFromTarget + Target.position;
            }                
        }
    }
}