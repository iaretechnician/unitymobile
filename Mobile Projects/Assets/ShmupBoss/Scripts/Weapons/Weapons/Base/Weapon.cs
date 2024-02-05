using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Basic class for all weapons used in this pack, this includes both munition and particle weapons.
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {
        public event CoreDelegate OnFire;

        /// <summary>
        /// How many bullets/missiles are fired per second.
        /// </summary>
        protected float rate;
        public abstract float Rate
        {
            get;
        }

        /// <summary>
        /// Multiplier which is used by the weapon rate control component to change the rate of 
        /// firing in a timed manner.<br></br>
        /// </summary>
        private float rateControlMultiplier;

        /// <summary>
        /// The weapon final rate is calculated using the input rate, this rate control and the level multiplier rate.
        /// </summary>
        public float RateControlMultiplier
        {
            get
            {
                return rateControlMultiplier;
            }
            set
            {
                rateControlMultiplier = value;

                // This is to reset the wait time counter if a new rate control multiplier has been set.
                if (value > 0.0f && Level.IsFinishedLoading)
                {
                    FiringTime = Time.time;
                }
            }
        }

        /// <summary>
        /// The time at which the weapon can fire again.
        /// </summary>
        public float FiringTime
        {
            get;
            set;
        }

        /// <summary>
        /// The time period until a weapon can fire again.
        /// </summary>
        private float waitTime;
        public float WaitTime
        {
            get
            {
                if (Rate > 0.0f)
                {
                    waitTime = (1 / Rate);
                }
                else
                {
                    waitTime = Mathf.Infinity;
                }

                return waitTime;
            }
        }

        /// <summary>
        /// Number of bullets/missiles that will be emitted when the weapon fires, only use this in conjunction 
        /// with the arc angle spread to fire in a radial pattern.
        /// </summary>
        protected int BulletsNumber;

        /// <summary>
        /// Determines how spread out the radial bullets are if the bullets number is bigger than 1.
        /// </summary>
        protected float ArcAngleSpread;

        /// <summary>
        /// This time forces a wait period between making firing sound effects, this is usually needed when 
        /// you have a weapon with a very high rate of fire which would make the sound effects overlap and become 
        /// incomprehensible.
        /// </summary>
        protected float SfxCoolDownTime;

        /// <summary>
        /// Returns true if the weapon is in SFX cooldown and cannot play any new sound effects.
        /// </summary>
        protected bool isInFiringSfxCooldown;

        /// <summary>
        /// Resets the firing time and the rate control multiplier.
        /// </summary>
        protected virtual void Awake()
        {
            RateControlMultiplier = 1.0f;
            FiringTime = Time.time;
        }

        /// <summary>
        /// If the current time is more than the firing time; the weapon will emit its projectiles. 
        /// Otherwise this method will not execute anything.
        /// </summary>
        /// <param name="agentDirection">The current direction of the agent firing the weapon.</param>
        public void FireWhenPossible(Vector2 agentDirection)
        {          
            if(Rate <= 0.0f)
            {
                return;
            }
            
            if (Time.time > FiringTime)
            {
                FiringTime = Time.time + WaitTime;

                Fire(agentDirection);
                StartFiringSoundCoroutine();
                RaiseOnFire();
            }
        }

        public abstract void Fire(Vector2 agentDirection);

        protected abstract void StartFiringSoundCoroutine();

        protected void RaiseOnFire()
        {
            OnFire?.Invoke(null);
        }
    }
}