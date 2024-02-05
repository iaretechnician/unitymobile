using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// System event arugments to store the properties of a particle hitting a target,
    /// this will store the hit angle, position and game object. This is used in the 
    /// particle hit fx script to spawn the hit effect using that information.
    /// </summary>
    public class ParticleHitArgs : System.EventArgs
    {
        public float HitAngle;

		public Vector3 HitPosition;

		public GameObject HitGO;

		public ParticleHitArgs(float hitAngle, Vector3 hitPosition, GameObject hitGO)
        {
            HitAngle = hitAngle;
            HitPosition = hitPosition;
            HitGO = hitGO;
        }
    }
}