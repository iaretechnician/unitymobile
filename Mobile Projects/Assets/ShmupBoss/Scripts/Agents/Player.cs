using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// In addition to all the information inherited from the agent and agentCollidable class
    /// The player class holds information for player specific events and upgrade infromation.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Agents/Player")]
    public class Player : AgentCollidable
    {
        public event CoreDelegate OnPickUp;
        public event CoreDelegate OnHeal;
        public event CoreDelegate OnHealHealth;
        public event CoreDelegate OnHealShield;
        public event CoreDelegate OnHealthUpgrade;
        public event CoreDelegate OnShieldUpgrade;
        public event CoreDelegate OnInvinciblityPickup;

        public event CoreDelegate OnLivesPickup;

        public event CoreDelegate OnWeaponsUpgrade;
        public event CoreDelegate OnWeaponsDowngrade;

        public event CoreDelegate OnVisualUpgrade;
        public event CoreDelegate OnVisualDowngrade;

        public event CoreDelegate OnSpeedChange;

        public event CoreDelegate OnPointsPickup;
        public event CoreDelegate OnCoinsPickup;
        public event CoreDelegate OnBombsPickup;
        public event CoreDelegate OnFiringBomb;

        /// <summary>
        /// The max health a player can achieve after taking health upgrades.
        /// </summary>
        [Header("Vitals Upgrade Limits")]
        [Space]
        [Tooltip("The max health a player can achieve after taking health upgrades.")]
        [SerializeField]
        private float maxHealth;

        /// <summary>
        /// The max shield a player can achieve after taking health upgrades.
        /// </summary>
        [Tooltip("The max shield a player can achieve after taking health upgrades.")]
        [SerializeField]
        private float maxShield;

        /// <summary>
        /// The weapons start upgrade stage for the player in the level.
        /// </summary>
        [Header("Weapons Upgrade Settings")]
        [Space]
        [Tooltip("The start weapons upgrade stage for the player in the level.")]
        [SerializeField]
        private int weaponsUpgradeStage;
        private int currentWeaponsUpgradeStage;
        public int CurrentWeaponsUpgradeStage
        {
            get
            {
                return currentWeaponsUpgradeStage;
            }
            private set
            {
                if(value > maxWeaponsUpgradeStage)
                {
                    currentWeaponsUpgradeStage = maxWeaponsUpgradeStage;
                }
                else if(value < 0)
                {
                    currentWeaponsUpgradeStage = 0;
                }
                else
                {
                    currentWeaponsUpgradeStage = value;
                }

                UpdateWeaponsUpgradeStage();
            }
        }

        /// <summary>
        /// The max weapons upgrade stage for the player after picking up upgrades.
        /// </summary>
        [Tooltip("The max weapons upgrade stage for the player after picking up upgrades.")]
        [SerializeField]
        private int maxWeaponsUpgradeStage;

        /// <summary>
        /// The start visual upgrade stage for the player in the level.
        /// </summary>
        [Header("Visual Upgrade Settings")]
        [Space]
        [Tooltip("The start visual upgrade stage for the player in the level.")]
        [SerializeField]
        private int visualUpgradeStage;
        private int currentVisualUpgradeStage;
        public int CurrentVisualUpgradeStage
        {
            get
            {
                return currentVisualUpgradeStage;
            }
            private set
            {
                if (value > maxVisualUpgradeStage)
                {
                    currentVisualUpgradeStage = maxVisualUpgradeStage;
                }
                else if (value < 0)
                {
                    currentVisualUpgradeStage = 0;
                }
                else
                {
                    currentVisualUpgradeStage = value;
                }
            }
        }

        /// <summary>
        /// The max visual upgrade stage for the player after picking up upgrades.
        /// </summary>
        [Tooltip("The max visual upgrade stage for the player after picking up upgrades.")]
        [SerializeField]
        private int maxVisualUpgradeStage;

        /// <summary>
        /// If true, during the level, when the player dies it will keep its upgrades after respawn.
        /// </summary>
        [Tooltip("If true, during the level, when the player dies it will keep its upgrades after respawn.")]
        [SerializeField]
        private bool isKeepingUpgradesOnRespawn;

        /// <summary>
        /// The time counter in which during the player is invinncible.
        /// This counter is used with only with the player and not with the enemy as well because 
        /// the player picks up invinicbility time pickups which has the possiblity of being additive.
        /// </summary>
        private float invincibilityTimeCounter;

        /// <summary>
        /// This is set to true when the agent starts executing the Unity update hook to make sure the on enable hook 
        /// has passed, it acts as a second or late on enable hook because certain events and methods need to be first 
        /// processed in on enabled then others in on late enabled.
        /// </summary>
        private bool isLateEnabled;

        public override float CollisionDamage
        {
            get
            {
                return collisionDamage * CurrentMultiplier.PlayerCollisionDamageMultiplier;
            }
        }

        public PlayerMover PlayerMoverComp
        {
            get;
            private set;
        }

        public override int OpposingFactionMunitionLayer
        {
            get
            {
                return ProjectLayers.EnemyMunition;
            }
        }

        public override int OpposingFactionLayer
        {
            get
            {
                return ProjectLayers.Enemies;
            }
        }

        /// <summary>
        /// This multiplier is extracted from the multiplier data set at the multiplier instance.
        /// </summary>
        public override float VitalsMultiplier
        {
            get
            {
                return CurrentMultiplier.PlayerVitalsMultiplier;
            }
        }

        /// <summary>
        /// In adition to invinciblity events, finding if is using particles; 
        /// this awake hook validates the input and caches the player mover comp.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Validate();
            PlayerMoverComp = GetComponent<PlayerMover>();
        }

        protected void Update()
        {
            if (!isLateEnabled)
            {
                Activate();
                isLateEnabled = true;
            }
            
            ApplyImmunity();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            isLateEnabled = false;
        }

        protected override void Validate()
        {
            base.Validate();

            ValidateMaxVitals();
            ValidateWeaponsUpgradeStage();
            ValidateVisualUpgradeStage();
        }

        private void ValidateMaxVitals()
        {
            if(maxHealth < health)
            {
                maxHealth = health;
            }

            if(maxShield < shield)
            {
                maxShield = shield;
            }
        }

        private void ValidateWeaponsUpgradeStage()
        {
            if (weaponsUpgradeStage < 0)
            {
                weaponsUpgradeStage = 0;
            }

            if (maxWeaponsUpgradeStage < weaponsUpgradeStage)
            {
                maxWeaponsUpgradeStage = weaponsUpgradeStage;
            }
        }

        private void ValidateVisualUpgradeStage()
        {
            if (visualUpgradeStage < 0)
            {
                visualUpgradeStage = 0;
            }

            if (maxVisualUpgradeStage < visualUpgradeStage)
            {
                maxVisualUpgradeStage = visualUpgradeStage;
            }
        }

        /// <summary>
        /// Resets the vitals (health/shield), the "CanTakeCollisions", is invincible triggers 
        /// and counter and the upgrade stages.<br></br>
        /// Due to current health/shield properties protection, please always make sure to assign the max then full 
        /// health/shield before assigning any values to current vitals health/shield.
        /// </summary>
        protected override void ResetSettings()
        {
            currentVitals.MaxHealth = maxHealth * VitalsMultiplier;
            currentVitals.MaxShield = maxShield * VitalsMultiplier;

            currentVitals.FullHealth = health * VitalsMultiplier;
            currentVitals.FullShield = shield * VitalsMultiplier;

            currentVitals.Health = health * VitalsMultiplier;
            currentVitals.Shield = shield * VitalsMultiplier;

            IsInvincibleTrigger1 = true;
            IsInvincibleTrigger2 = false;
            invincibilityTimeCounter = invincibilityTimeAfterSpawn;
            CanTakeCollisions = canTakeCollisions;

            if (!isKeepingUpgradesOnRespawn)
            {
                CurrentWeaponsUpgradeStage = weaponsUpgradeStage;
                CurrentVisualUpgradeStage = visualUpgradeStage;
            }
        }

        protected virtual void ApplyImmunity()
        {
            if (invincibilityTimeCounter > 0.0f)
            {
                IsInvincibleTrigger1 = true;
            }
            else
            {
                IsInvincibleTrigger1 = false;
            }

            if (invincibilityTimeCounter >= 0.0f)
            {
                invincibilityTimeCounter -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Spawns any munition hit fx, deactivates the munition which has hit the player and takes any hit damage.
        /// </summary>
        /// <param name="collider2D">The collision object with 2D collider which just hit the player</param>
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

            if (MunitionPoolEnemy.Instance == null)
            {
                Debug.Log("The player is trying to despawn munition but is unable " +
                    "to because it can't find the munitionPoolEnemy. Munition was " +
                    "simply deactivated without being able to access it's damage value.");

                collider2D.gameObject.SetActive(false);
                return;
            }
                        
            MunitionPoolEnemy.Instance.Despawn(collider2D.gameObject);

            if (IsInvincible)
            {
                return;
            }

            string munitionName = collider2D.name;
            float munitionDamage;

            try
            {
                munitionDamage = MunitionPoolEnemy.Instance.MunitionDamageByName[munitionName];
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
        /// This needs to override the base method to add the adjust player lives call before the elmination.
        /// </summary>
        protected override void Eliminate()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (isUsingParticleWeapons)
            {
                ReplaceParticleWeapons();
            }

            if (Level.Instance != null)
            {
                Level.Instance.AdjustPlayerLives(-1);
            }

            RaiseOnElimination();
            gameObject.SetActive(false);
        }

        public override void Subscribe(CoreDelegate method, AgentEvent agentEvent)
        {
            switch (agentEvent)
            {
                case AgentEvent.PickUp:
                    {
                        OnPickUp += method;
                        return;
                    }

                case AgentEvent.Heal:
                    {
                        OnHeal += method;
                        return;
                    }

                case AgentEvent.HealHealth:
                    {
                        OnHealHealth += method;
                        return;
                    }

                case AgentEvent.HealShield:
                    {
                        OnHealShield += method;
                        return;
                    }

                case AgentEvent.HealthUpgrade:
                    {
                        OnHealthUpgrade += method;
                        return;
                    }

                case AgentEvent.ShieldUpgrade:
                    {
                        OnShieldUpgrade += method;
                        return;
                    }

                case AgentEvent.InvincibilityPickup:
                    {
                        OnInvinciblityPickup += method;
                        return;
                    }

                case AgentEvent.LivesPickup:
                    {
                        OnLivesPickup += method;
                        return;
                    }

                case AgentEvent.WeaponsUpgrade:
                    {
                        OnWeaponsUpgrade += method;
                        return;
                    }

                case AgentEvent.WeaponsDowngrade:
                    {
                        OnWeaponsDowngrade += method;
                        return;
                    }

                case AgentEvent.VisualUpgrade:
                    {
                        OnVisualUpgrade += method;
                        return;
                    }

                case AgentEvent.VisualDowngrade:
                    {
                        OnVisualDowngrade += method;
                        return;
                    }

                case AgentEvent.SpeedChange:
                    {
                        OnSpeedChange += method;
                        return;
                    }

                case AgentEvent.PointPickup:
                    {
                        OnPointsPickup += method;
                        return;
                    }

                case AgentEvent.CoinPickup:
                    {
                        OnCoinsPickup += method;
                        return;
                    }

                case AgentEvent.BombsPickup:
                    {
                        OnBombsPickup += method;
                        return;
                    }

                case AgentEvent.FiringBomb:
                    {
                        OnFiringBomb += method;
                        return;
                    }
            }

            base.Subscribe(method, agentEvent);
        }

        public void Heal(float healAmount)
        {
            bool isUsingShield = currentVitals.FullShield > 0.0f;
            bool isHealthFull = currentVitals.Health >= currentVitals.FullHealth;
            bool isShieldFull = currentVitals.Shield >= currentVitals.FullShield;

            if (!isHealthFull)
            {
                if (isUsingShield)
                {
                    ApplyHealLeftover(healAmount);
                }

                currentVitals.Health += healAmount;
                RaiseOnHeal();
            }
            else if (isUsingShield && !isShieldFull)
            {
                currentVitals.Shield += healAmount;
                RaiseOnHeal();
            }
        }

        private void ApplyHealLeftover(float healAmount)
        {
            float emptyHealthAmount = currentVitals.FullHealth - currentVitals.Health;

            if (healAmount > emptyHealthAmount)
            {
                float healAmountLeftover = healAmount - emptyHealthAmount;
                currentVitals.Shield += healAmountLeftover;
            }
        }

        public void FillHealth(float healthAmount)
        {
            if (currentVitals.Health < currentVitals.FullHealth)
            {
                currentVitals.Health += healthAmount;
                RaiseOnHealHealth();
            }
        }

        public void FillShield(float ShieldAmount)
        {
            if (currentVitals.Shield < currentVitals.FullShield)
            {
                currentVitals.Shield += ShieldAmount;
                RaiseOnHealShield();
            }
        }

        public void UpgradeHealth(float upgradeAmount)
        {
            if (currentVitals.FullHealth < maxHealth)
            {
                currentVitals.FullHealth += upgradeAmount;
                RaiseOnHealthUpgrade();
            }
        }

        public void UpgradeShield(float upgradeAmount)
        {
            if (currentVitals.FullShield < maxShield)
            {
                currentVitals.FullShield += upgradeAmount;
                RaiseOnShieldUpgrade();
            }
        }

        public void AddImmunityTime(float time)
        {
            invincibilityTimeCounter += time;
            RaiseOnInvincibiltyPickup();
        }

        public void NotifyOfPlayerLivesPickup()
        {
            RaiseOnExtraLifePickup();
        } 
        
        /// <summary>
        /// This method is executed when the weapons upgrade stage has changed. 
        /// </summary>
        private void UpdateWeaponsUpgradeStage()
        {
            foreach (WeaponMunitionPlayer weapon in MunitionWeapons)
            {
                if (weapon == null)
                {
                    continue;
                }

                weapon.StageIndex = CurrentWeaponsUpgradeStage;
            }

            foreach (WeaponParticlePlayer weapon in ParticleWeapons)
            {
                if(weapon == null)
                {
                    continue;
                }
                
                weapon.StageIndex = CurrentWeaponsUpgradeStage;
            }
        }

        public void UpgradeWeapons(int amount)
        {
            CurrentWeaponsUpgradeStage += amount;

            if(amount > 0)
            {
                RaiseOnWeaponsUpgrade();
            }
            else if(amount < 0)
            {
                RaiseOnWeaponsDowngrade();
            }
        }

        public void UpgradeVisual(int amount)
        {
            CurrentVisualUpgradeStage += amount;

            if (amount > 0)
            {
                RaiseOnVisualUpgrade();
            }
            else if(amount < 0)
            {
                RaiseOnVisualDowngrade();
            }
        }

        public void AdjustSpeed(float amount)
        {
            if(PlayerMoverComp == null)
            {
                return;
            }

            if(amount == 0.0f)
            {
                return;
            }

            PlayerMoverComp.AdjustSpeed(amount);

            RaiseOnSpeedChange();
        }

        public void NotifiyOfPickup(PickupType pickupType)
        {
            RaiseOnPickup(new PickupArgs(pickupType));
        }

        public void NotifiyOfPointsPickup()
        {
            RaiseOnPointsPickup();
        }

        public void NotifiyOfCoinsPickup()
        {
            RaiseOnCoinsPickup();
        }

        public void NotifiyOfBombsPickup()
        {
            RaiseOnBombsPickup();
        }

        public void NotifiyOfFiringBomb()
        {
            RaiseOnFiringBomb();
        }

        /// <summary>
        /// Invokes the on pickup event. It requires passing the pickup args which store the
        /// pickup type, to know if the UI for example needs to be updated if it's a health or 
        /// shield pickup.
        /// </summary>
        /// <param name="args">System event arguments which contain the type of item picked up.</param>
        protected void RaiseOnPickup(PickupArgs args)
        {
            OnPickUp?.Invoke(args);
        }

        protected void RaiseOnHeal()
        {
            OnHeal?.Invoke(null);
        }

        protected void RaiseOnHealHealth()
        {
            OnHealHealth?.Invoke(null);
        }

        protected void RaiseOnHealShield()
        {
            OnHealShield?.Invoke(null);
        }

        protected void RaiseOnHealthUpgrade()
        {
            OnHealthUpgrade?.Invoke(null);
        }

        protected void RaiseOnShieldUpgrade()
        {
            OnShieldUpgrade?.Invoke(null);
        }

        protected void RaiseOnInvincibiltyPickup()
        {
            OnInvinciblityPickup?.Invoke(null);
        }

        protected void RaiseOnExtraLifePickup()
        {
            OnLivesPickup?.Invoke(null);
        }

        protected void RaiseOnWeaponsUpgrade()
        {
            OnWeaponsUpgrade?.Invoke(null);
        }

        protected void RaiseOnWeaponsDowngrade()
        {
            OnWeaponsDowngrade?.Invoke(null);
        }

        protected void RaiseOnVisualUpgrade()
        {
            OnVisualUpgrade?.Invoke(null);
        }

        protected void RaiseOnVisualDowngrade()
        {
            OnVisualDowngrade?.Invoke(null);
        }

        protected void RaiseOnSpeedChange()
        {
            OnSpeedChange?.Invoke(null);
        }

        protected void RaiseOnPointsPickup()
        {
            OnPointsPickup?.Invoke(null);
        }

        protected void RaiseOnCoinsPickup()
        {
            OnCoinsPickup?.Invoke(null);
        }

        protected void RaiseOnBombsPickup()
        {
            OnBombsPickup?.Invoke(null);
        }

        protected void RaiseOnFiringBomb()
        {
            OnFiringBomb?.Invoke(null);
        }       
    }
}