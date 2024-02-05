using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This adds the option for the enemy to detonate in a proximity mine like manner.<br></br>
    /// if the player comes within this agent' detonation start radius; it will explode after the set time, 
    /// damaging the player if it's within its explosion radius.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Agents/Enemy Detonator")]
    public class EnemyDetonator : Enemy
    {
        public event CoreDelegate OnDetonationStart;
        public event CoreDelegate OnExplosionHitTarget;

        /// <summary>
        /// The damage dealt to the player if it's within the explosion radius at the time of the explosion.
        /// </summary>
        [Header("Detonation Settings")]
        [Space]
        [Tooltip("The damage dealt to the player if it's within the explosion radius at the time of the explosion.")]
        [SerializeField]
        private float explosionDamage;

        /// <summary>
        /// The radius in which the player will be damaged in if it's located within at the time of the explosion.
        /// </summary>
        [Tooltip("The radius in which the player will be damaged in if it's located within at the time of the explosion.")]
        [SerializeField]
        private float explosionRadius;

        /// <summary>
        /// If the player enters this radius, then the detonation process will start and after
        /// the "timeToDetonate" the explosion will occur.
        /// </summary>
        [Tooltip("If the player enters this radius, then the detonation process will start and " +
            "after the timeToDetonate the explosion will occur.")]
        [SerializeField]
        private float detonationStartRadius;

        /// <summary>
        /// The time between the detonation start and the explosion.
        /// </summary>
        [Tooltip("The time between the detonation start and the explosion.")]
        [SerializeField]
        private float timeToDetonate;

        /// <summary>
        /// Is true after the detonation has been initiated and the player has entered the detonation start radius.
        /// </summary>
        private bool isDetenationActivated;

        /// <summary>
        /// The tracker which holds the information for the target to know if the target has entered the detonation 
        /// start radius. The target is usually the player.
        /// </summary>
        private Tracker tracker;

        /// <summary>
        /// In addition to invincibilty events and finding if the enemy is using particle weapons. 
        /// This will also cache or add a tracker.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            tracker = GetComponent<Tracker>();

            if (tracker == null) 
            {
                tracker = gameObject.AddComponent<TrackerPlayer>();
            }
        }

        protected void Update()
        {
            CheckForDetonation();
        }

        /// <summary>
        /// Resets the vitals (health/shield), the "CanTakeCollisions" value and the 
        /// "isDetenationActivated" value.<br></br>
        /// Due to current health/shield properties protection, please always make sure to assign the max then 
        /// full health/shield before assigning any values to current vitals health/shield.
        /// </summary>
        protected override void ResetSettings()
        {
            base.ResetSettings();

            isDetenationActivated = false;
        }

        /// <summary>
        /// When the tracker target have entered the detonation start radius, this method starts the detonation process.
        /// </summary>
        private void CheckForDetonation()
        {
            if (tracker.DistanceToTarget <= detonationStartRadius && !isDetenationActivated)
            {
                if (gameObject.activeSelf)
                {
                    StartCoroutine(Detonate());

                    isDetenationActivated = true;
                }
            }
        }

        /// <summary>
        /// Creates the explosion that will damage the player if it's within it's radius and it will also eliminate this enemy.
        /// </summary>
        /// <returns></returns>
        IEnumerator Detonate()
        {
            RaiseOnDetonationStart();

            yield return new WaitForSeconds(timeToDetonate);

            if(Level.Instance == null)
            {
                yield break;
            }

            if(tracker.DistanceToTarget <= explosionRadius)
            {
                if(Level.Instance.PlayerComp != null && Level.Instance.IsPlayerAlive)
                {
                    RaiseOnExplosionHitTarget();
                    Level.Instance.PlayerComp.TakeHit(explosionDamage);
                }
            }

            Eliminate();
        }

        public override void Subscribe(CoreDelegate method, AgentEvent eventType)
        {
            switch (eventType)
            {
                case AgentEvent.DetonationStart:
                    {
                        OnDetonationStart += method;
                        return;
                    }
            }

            base.Subscribe(method, eventType);
        }

        protected void RaiseOnExplosionHitTarget()
        {
            OnExplosionHitTarget?.Invoke(null);
        }

        protected void RaiseOnDetonationStart()
        {
            OnDetonationStart?.Invoke(null);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            Gizmos.color = Color.red;
            GizmosExtension.DrawCircle(transform.position, explosionRadius);

            Gizmos.color = Color.cyan;
            GizmosExtension.DrawCircle(transform.position, detonationStartRadius);
        }
    }
}