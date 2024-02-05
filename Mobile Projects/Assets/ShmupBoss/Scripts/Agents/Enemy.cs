using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// In addition to all the information inherited from the agent and agentCollidable class
    /// The enemy class holds information for enemy specific events and drop infromation.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Agents/Enemy")]
    public class Enemy : AgentCollidable, IInGamePool
    {
        /// <summary>
        /// This is related to the "OnAllEnemyDestroyed" destroyed event and not to the actual pooling
        /// system, this event is related to the number of active enemies in level and is raised when
        /// a new enemy is activated in the level.
        /// </summary>
        public event CoreDelegate OnAddToInGamePool;

        /// <summary>
        /// This is related to the "OnAllEnemyDestroyed" destroyed event and not to the actual pooling
        /// system, this event is related to the number of active enemies in level and is raised when
        /// an enemy is deactivated in the level.
        /// </summary>
        public event CoreDelegate OnRemoveFromInGamePool;

        public event CoreDelegate OnDrop;

        /// <summary>
        /// The radius in which dropped items will possibly be spawned in.
        /// </summary>
        [Header("Drop Settings")]
        [Space]
        [Tooltip("The radius in which dropped items will possibly be spawned in.")]
        [SerializeField]
        protected float dropRadius;

        /// <summary>
        /// Array of pickup items with their possibility of being dropped.
        /// </summary>
        [Tooltip("Array of pickup items with their possibility of being dropped.")]
        [SerializeField]
        protected Drop[] drops;
        public Drop[] Drops
        {
            get
            {
                return drops;
            }
        }

        [Header("Elimination Rewards")]
        [Space]
        [Tooltip("Score gained when eliminating this enemy.")]
        [SerializeField] 
        protected int score;
        public int Score
        {
            get
            {
                return Mathf.CeilToInt(score * CurrentMultiplier.ScoreMultiplier);
            }
        }

        [Tooltip("Coins gained when eliminating this enemy.")]
        [SerializeField]
        protected int coins;
        public int Coins
        {
            get
            {
                return Mathf.CeilToInt(coins * CurrentMultiplier.CoinsMultiplier);
            }
        }

        /// <summary>
        /// Despawns the enemy after this time has passed.
        /// </summary>
        [Header("Disable Settings")]
        [Space]
        [Tooltip("Despawns the enemy after this time has passed.")]
        [SerializeField]
        protected float disableAfterTime;

        /// <summary>
        /// This gives elimination rewards and drops items when the enemy is disabled after time.
        /// </summary>
        [SerializeField]
        [Tooltip("This gives elimination rewards and drops items when the enemy is disabled after time.")]
        protected bool isEliminatedWhenDisabled;

        public override float CollisionDamage
        {
            get
            {
                return collisionDamage * CurrentMultiplier.EnemiesCollisionDamageMultiplier;
            }
        }

        public override int OpposingFactionMunitionLayer
        {
            get
            {
                return ProjectLayers.PlayerMunition;
            }
        }

        public override int OpposingFactionLayer
        {
            get
            {
                return ProjectLayers.Player;
            }
        }

        public override float VitalsMultiplier
        {
            get
            {
                return CurrentMultiplier.EnemiesVitalsMultiplier;
            }
        }

        /// <summary>
        /// Used by the enemy shooter and the boss system to enable/disable shooting.
        /// </summary>
        [HideInInspector]
        public bool CanShoot = true;

        /// <summary>
        /// In addition to reseting the settings, this starts the spawn invincibility and disabling routine, 
        /// adds the enemy to the number active enemies in the scene and raises the proper events
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(ActivateSpawnInvincibility(invincibilityTimeAfterSpawn));

            if (Level.IsFinishedLoading)
            {
                this.AddToPool<Enemy>();
                RaiseOnAddToInGamePool();
                Activate();
            }           

            if(disableAfterTime > 0.0f)
            {
                StartCoroutine(DisableAfterTime(disableAfterTime));
            }          
        }

        /// <summary>
        /// Removes the enemy from the number of active enemies from the scene, this list is used for the tracker
        /// to pick a random enemy.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (!Level.IsFinishedLoading)
            {
                return;
            }

            this.RemoveFromPool<Enemy>();

            RaiseOnRemoveFromInGamePool();
        }

        /// <summary>
        /// Despawns the enemy if it gets out of the despawning field.
        /// </summary>
        /// <param name="collision">The other collision object the enemy just exited its trigger.</param>
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == ProjectLayers.DespawningField)
            {
                Despawn();
            }
        }

        protected virtual IEnumerator ActivateSpawnInvincibility(float time)
        {
            IsInvincibleTrigger1 = true;

            yield return new WaitForSeconds(time);

            IsInvincibleTrigger1 = false;

            yield break;
        }

        protected IEnumerator DisableAfterTime(float time)
        {
            yield return new WaitForSeconds(time);

            if (isEliminatedWhenDisabled)
            {
                Eliminate();
            }
            else
            {
                Despawn();
            }
        }

        /// <summary>
        /// Spawns any munition hit fx, deactivates the munition which has hit the enemy and takes any hit damage.
        /// </summary>
        /// <param name="collider2D">The collision object with 2D collider which just hit the enemy</param>
        protected override void HitByOpposingFactionMunition(Collider2D collider2D)
        {
            if (isTriggeringMunitionFXWhenHit)
            {
                Munition munition = collider2D.gameObject.GetComponent<Munition>();

                if (munition != null)
                {
                    munition.NotifyOfOnHit();
                }
            }

            if (MunitionPoolPlayer.Instance == null)
            {
                Debug.Log("An enemy is trying to despawn munition but is unable " +
                    "to because it can't find the munitionPoolPlayer. Munition was " +
                    "simply deactivated without being able to access it's damage value.");

                collider2D.gameObject.SetActive(false);
                return;
            }

            MunitionPoolPlayer.Instance.Despawn(collider2D.gameObject);

            if (IsInvincible)
            {
                return;
            }

            string munitionName = collider2D.name;
            float munitionDamage;

            try
            {
                munitionDamage = MunitionPoolPlayer.Instance.MunitionDamageByName[munitionName];
            }
            catch (KeyNotFoundException)
            {
                Debug.Log("Can't find the player munition data that just hit an enemy. " +
                    "Did you assign a munition layer to a non munition object?" +
                    "Or perhaps your munition was not pooled correctly.");

                return;
            }

            TakeHit(munitionDamage);
        }

        /// <summary>
        /// Gives Eliminiation rewards, drops pickups, raises proper events and despawns the enemy.
        /// </summary>
        protected override void Eliminate()
        {
            AddEliminationRewards();
            ScatterDrops();
            RaiseOnElimination();
            Despawn();
        }

        /// <summary>
        /// Despawns the enemy from the enemy pool after replacing particle weapons.
        /// </summary>
        protected virtual void Despawn()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            if (isUsingParticleWeapons)
            {
                ReplaceParticleWeapons();
            }

            if (EnemyPool.Instance != null)
            {
                EnemyPool.Instance.Despawn(gameObject);
            }
            else
            {
                Debug.Log("Was unable to find the enemy in the EnemyPool" +
                    " to despawn it, it was simply deactivated instead. " +
                    "It's possible that you don't have an enemy pool in your scene, " +
                    "or you used an enemy in the scene without spawning it from a wave" +
                    " in a spawner or somehow the enemy wasn't pooled correctly.");

                gameObject.SetActive(false);
            }
        }

        protected virtual void AddEliminationRewards()
        {
            if (Level.Instance == null)
            {
                return;
            }

            Level.Instance.AddScore(Score);
            Level.Instance.AddCoins(Coins);
        }

        protected void ScatterDrops()
        {           
            foreach(Drop drop in drops)
            {
                if(drop.DropChance < Random.Range(0, 100))
                {
                    return;
                }

                if(drop.PickupPrefab == null)
                {
                    Debug.Log("Enemy.cs Enemy named: " + name + " is missing one of the pickup prefabs in its drops list.");

                    continue;
                }

                Vector3 dropPosition = transform.position + Math2D.Vector2ToVector3(Random.insideUnitCircle * dropRadius);

                GameObject pickupGO = PickupPool.Instance.Spawn(drop.PickupPrefab.name);
                pickupGO.transform.position = dropPosition;

                RaiseOnDrop();
            }
        }

        public override void Subscribe(CoreDelegate method, AgentEvent eventType)
        {
            switch (eventType)
            {
                case AgentEvent.Drop:
                    {
                        OnDrop += method;
                        return;
                    }
            }

            base.Subscribe(method, eventType);
        }

        protected void RaiseOnDrop()
        {
            OnDrop?.Invoke(null);
        }

        protected void RaiseOnAddToInGamePool()
        {
            OnAddToInGamePool?.Invoke(null);
        }

        protected void RaiseOnRemoveFromInGamePool()
        {
            OnRemoveFromInGamePool?.Invoke(null);
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            GizmosExtension.DrawCircle(transform.position, dropRadius);
        }
    }
}