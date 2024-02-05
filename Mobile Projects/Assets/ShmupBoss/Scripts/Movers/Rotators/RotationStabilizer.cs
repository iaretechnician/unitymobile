using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A very simple component to make sure that the rotation of the game object it is attached to is 
    /// unchanged by any hierarchy.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Rotators/Rotation Stabilizer")]
    public class RotationStabilizer : MonoBehaviour
    {
        private Quaternion startRotation;
        
        private void Awake()
        {
            startRotation = transform.rotation;
        }

        void Update()
        {
            transform.rotation = startRotation;
        }
    }
}