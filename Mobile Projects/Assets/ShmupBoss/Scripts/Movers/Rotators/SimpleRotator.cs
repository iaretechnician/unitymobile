using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Rotates the object by user input values.<br></br>
    /// This component does not use the rotation manager.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Rotators/Simple Rotator")]
    public class SimpleRotator : MonoBehaviour
    {
        [Tooltip("Rotation speed on the X axis")]
        [SerializeField]
        private float xRotationSpeed;

        [Tooltip("Rotation speed on the Y axis")]
        [SerializeField]
        private float yRotationSpeed;

        [Tooltip("Rotation speed on the Z axis")]
        [SerializeField]
        private float zRotationSpeed;

        private void Update()
        {
            transform.Rotate(Vector3.right, xRotationSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up, yRotationSpeed * Time.deltaTime);
            transform.Rotate(Vector3.forward, zRotationSpeed * Time.deltaTime);
        }
    }
}