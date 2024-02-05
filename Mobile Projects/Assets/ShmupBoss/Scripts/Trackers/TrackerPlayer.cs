using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Uses the player as its target and finds the vector to it, 
    /// direction, distance and when it has reached it or not.<br></br>
    /// This is typically used with enemy detonators (mines), magnet mover pickups 
    /// and missiles following player.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Trackers/Tracker Player")]
    public class TrackerPlayer : Tracker
    {
        public override bool IsTargetFound
        {
            get
            {
                if(Level.Instance == null)
                {
                    return false;
                }

                return Level.Instance.IsPlayerAlive;
            }
        }

        public override Transform TargetTransform 
        {
            get
            {
                if (Level.Instance == null)
                {
                    return null;
                }

                if (Level.Instance.PlayerComp == null)
                {
                    return null;
                }

                return Level.Instance.PlayerComp.transform;
            }
        }
    }
}