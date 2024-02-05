using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Simple rotator for rotating an object on the Z-axis.<br></br> 
    /// This rotator does not use the rotation manager component.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Rotators/Circular Rotator")]
    public class CircularRotator : MonoBehaviour
    {
        /// <summary>
        /// Rotation change speed on the Z-axis.
        /// </summary>
        [Tooltip("Rotation change speed on the Z-axis.")]
        [SerializeField]
        private float turnSpeed;

        private void Update()
        {
            float circularRotation = (turnSpeed * Time.deltaTime);

            transform.RotateAround(transform.position, Vector3.forward, circularRotation);
        }
    }
}