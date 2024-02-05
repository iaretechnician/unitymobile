using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Any extra tools that are related to munition, for now it only contains finding the munition speed.
    /// </summary>
    public static class MunitionTools
    {      
        /// <summary>
        /// Using a munition instance, it will find its type and then workout its speed value.
        /// </summary>
        /// <param name="munition">The munition which you wish to find its speed.</param>
        /// <returns>The speed value of the munuition.</returns>
        public static float FindMunitionSpeed(Munition munition)
        {
            if (munition is Bullet bullet)
            {
                return bullet.Speed;
            }
            else if (munition is Missile missile)
            {
                MissileMover missileMover = munition.gameObject.GetComponent<MissileMover>();

                if (missileMover == null)
                {
                    Debug.Log("WeaponMunitionPlayer.cs: You have used a missile munition but have forgotten to " +
                        "add a proper missile mover to it, or have created your own. " +
                        "Unable to find the speed of the missile to create the preview.");

                    return 1.0f;
                }

                return missileMover.Speed;
            }
            else
            {
                Debug.Log("WeaponMunitionPlayer.cs: The munition you have in the munition prefab uses a type  " +
                    "that can't be recognized, unable to find the speed of the munition to create the review.");

                return 1.0f;
            }
        }
    }
}