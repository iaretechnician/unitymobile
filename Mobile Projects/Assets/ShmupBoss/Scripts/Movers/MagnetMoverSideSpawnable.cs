using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This mover can be used with enemies which are typically mines, and are spawned by a side spawnable
    /// wave data in a finite/infinite spawner.<br></br> 
    /// It will simulate the effect of being attracted to the player when it comes closer.<br></br>
    /// This uses the player tracker to determine how to move.<br></br>
    /// The direction of movement of the agent when idle (target is not within magnet radius) is determined 
    /// by the spawn sides (initial direction).
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Magnet Mover Side Spawnable")]
    public class MagnetMoverSideSpawnable : MagnetMover, ISideSpawnable
    {
        /// <summary>
        /// This initial direction is determined by the spawn side.
        /// </summary>
        public FourDirection InitialDirection
        {
            get;
            set;
        }

        protected override void FindCurrentDirection()
        {
            if (tracker.IsTargetFound && isWithinMagnetRadius)
            {
                CurrentDirection = tracker.DirectionToTarget;
            }
            else
            {
                CurrentDirection = Directions.FourDirectionToVector2(InitialDirection);
            }
        }
    }
}