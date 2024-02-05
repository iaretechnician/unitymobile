using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This mover can be used with enemies which are spawned by a side spawnable wave data in 
    /// a finite/infinite spawner.<br></br> 
    /// It will simulate the effect of a missile being fired towards the player, but in this case it will have
    /// enemy vitals, fx and weapons, it is best used with an enemy detonator (mine) since it will constantly seek 
    /// the player.<br></br>
    /// This uses the player tracker to determine how to move.<br></br>
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Missile Mover Side Spawnable")]
    public class MissileMoverSideSpawnable : MissileMover, ISideSpawnable
    {
        private FourDirection initialDirection;
        public FourDirection InitialDirection
        {
            get
            {
                return initialDirection; 
            }
            set
            {
                initialDirection = value;
                currentVector = Directions.FourDirectionToVector2(InitialDirection);
            }
        }
    }
}