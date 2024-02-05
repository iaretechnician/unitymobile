using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Munition based which will be pooled using the proper agent munition pool.<br></br>
    /// Missiles require the proper type of tracker (player/random enemy) and a missile mover 
    /// (player/random enemy).<br></br>
    /// This missile is a munition which cannot be affected by the opposing faction munition.<br></br>
    /// When the missiles hits an opposing faction, it damages it by its collision, the explosion of the 
    /// missile has no damage radius.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Munition/Missile")]
    public class Missile : Munition
    {
        protected MissileMover missileMover;
        
        protected virtual void Awake()
        {
            if (!isIndependentOfWeaponRotation)
            {
                missileMover = GetComponent<MissileMover>();

                if (missileMover == null)
                {
                    Debug.Log("Missile.cs: missile named: " + name + " requires a missile mover of the proper " +
                        "type to operate. If it's fired from an enemy it needs a missile mover following player, " +
                        "if fired from a player it needs one following a random mover.");

                    return;
                }
            }

            Tracker tracker = GetComponent<Tracker>();

            if(tracker == null)
            {
                Debug.Log("Missile.cs: missile named: " + name + "  needs to have a " +
                    "tracker added to it to make things work.");

                return;
            }

            tracker.OnReachingTarget += SelfDestruct;
        }
        
        /// <summary>
        /// This will make the missile despawn if it ever reaches the pivot point of its target.<br></br>
        /// Please note that while this causes activating the on hit event it does not actually 
        /// damage the target. The target is only damaged if the missiles collides with it. 
        /// </summary>
        /// <param name="args">Null system event arguments for keeping convention.</param>
        private void SelfDestruct(System.EventArgs args)
        {
            Despawn();
        }

        public override void Initialize(Vector3 pos, Quaternion firingRotation, Vector2 emitDirection)
        {
            transform.position = pos;

            if (isIndependentOfWeaponRotation)
            {
                return;
            }

            if (missileMover == null)
            {
                return;
            }

            missileMover.EmitDirection = emitDirection;
            missileMover.FiringRotation = firingRotation;

            return;
        }
    }
}