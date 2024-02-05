using System;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for all the different agents in this pack: player, enemy, enemy detonator and boss sub phase.<br></br>
    /// This contains things like vitals, list of weapons, taking hits, invincibility and all the different events.<br></br>
    /// Please note that the agent collidable class inherits from this class and that they have been separated into
    /// two different classes instead of being combined to make it easier to understand the classes and debug or 
    /// add any future features, all agent collision information are stored in the agent collidable class.
    /// </summary>
    public abstract class Agent : MonoBehaviour
    {
        public event CoreDelegate OnActivation;
        public event CoreDelegate OnElimination;

        public event CoreDelegate OnTakeHit;
        public event CoreDelegate OnHealthDamage;
        public event CoreDelegate OnShieldDamage;

        public event CoreDelegate OnInvincibilityStart;
        public event CoreDelegate OnInvincibilityEnd;

        /// <summary>
        /// Full health of the agent at the start of the level.
        /// </summary>
        [Header("Vitals")]
        [Space]
        [Tooltip("Full health of the agent at the start of the level.")]
        [SerializeField]
        protected float health;

        /// <summary>
        /// Full shield of the agent at the start of the level.
        /// </summary>
        [Tooltip("Full shield of the agent at the start of the level.")]
        [SerializeField]
        protected float shield;

        /// <summary>
        /// Backing field for the current health, shield, full and max values for the agent.
        /// </summary>
        protected Vitals currentVitals;

        /// <summary>
        /// Exposed current health, shield, full and max values for the agent.
        /// </summary>
        public Vitals CurrentVitals
        {
            get
            {
                return currentVitals;
            }
        }

        /// <summary>
        /// This multiplier is controlled by the multiplier data set at the multiplier
        /// instance and control the health, shield and their full values to increase or decrease
        /// the difficulty of the level.
        /// </summary>
        public abstract float VitalsMultiplier
        {
            get;
        }

        /// <summary>
        /// For how long does the agent remain invincible after it has been spawned.
        /// </summary>
        [Header("Invincibility Settings")]
        [Space]
        [Tooltip("For how long does the agent remain invincible after it has been spawned.")]
        [SerializeField]
        protected float invincibilityTimeAfterSpawn;

        /// <summary>
        /// When set to true it will make bullets and missiles pass through the agent when the agent is 
        /// invincible. When false, bullets and missiles will hit the agent but without causing damage.
        /// </summary>
        [Tooltip("When set to true it will make bullets and missiles pass through the agent when the agent" +
            " is invincible. When false, bullets and missiles will hit the agent but without causing damage.")]
        [SerializeField]
        protected bool isPassthroughWhenInvincible;

        /// <summary>
        /// Stores the value of the "IsInvincible" bool property, which means whenever the value of 
        /// "IsInvincible" is changed an event would be invoked.<br></br>
        /// You can add methods to the events of "OnChangeToTrue" or "OnChangeToFalse" which are 
        /// stored in this changing bool and have them executed when the events are onvoked.
        /// </summary>
        private ChangingBool changingIsInvincibleBool = new ChangingBool();

        /// <summary>
        /// This value is set to true when either "IsInvincibleTrigger1" or "IsInvincibleTrigger2"
        /// is set to true.<br></br> 
        /// The reason 2 bools are used to possibly trigger this value is to give 
        /// added control to the invincibility.<br></br> 
        /// This is needed because we have multiple ways to control 
        /// invincibility such as spawn invinciblity, pickups or boss system invinciblity which are all
        /// controlled by co-routine and we do not wish that they cancel each other out.
        /// </summary>
        public bool IsInvincible
        {
            get
            {
                return changingIsInvincibleBool.CurrentValue;
            }
            private set
            {
                changingIsInvincibleBool.CurrentValue = value;
            }
        }

        private bool isInvincibleTrigger1;

        /// <summary>
        /// The first trigger for the invinciblity, this is typically used with the player or enemy
        /// spawn invinciblity and will set the "IsInvincible" bool.
        /// </summary>
        protected bool IsInvincibleTrigger1
        {
            get
            {
                return isInvincibleTrigger1;
            }
            set
            {
                isInvincibleTrigger1 = value;
                SetIsInvincibleValue();
            }
        }


        private bool isInvincibleTrigger2;

        /// <summary>
        /// The second trigger for the invinciblity, this is typically used with the boss system
        /// inviciblity settings and with the player when the level ends and will set the 
        /// "IsInvincible" bool.
        /// </summary>
        public bool IsInvincibleTrigger2
        {
            get
            {
                return isInvincibleTrigger2;
            }
            set
            {
                isInvincibleTrigger2 = value;
                SetIsInvincibleValue();
            }
        }

        // Note: Code style convecntion had been broken and this method has been placed here 
        // amongest the fields and properties because it is closely related to the above properties.

        /// <summary>
        /// Controls the "IsInvincible" bool by the 2 invinciblity triggers, if only one
        /// the triggers is true; "IsInvincible" will be set to true.
        /// </summary>
        private void SetIsInvincibleValue()
        {
            if(isInvincibleTrigger1 || isInvincibleTrigger2)
            {
                IsInvincible = true;
            }
            else
            {
                IsInvincible = false;
            }
        }
         
        [Header("Weapons")]
        [Space]
        [Tooltip("This pack uses 2 types of weapons, weapon munition and weapon particle," +
            " any munition weapons used by the agent need to be listed in this array.")]
        [SerializeField]
        protected WeaponMunition[] munitionWeapons;
        public WeaponMunition[] MunitionWeapons
        {
            get
            {
                return munitionWeapons;
            }
        }

        [Space]
        [Tooltip("This pack uses 2 types of weapons, weapon munition and weapon particle," +
            " any particle weapons used by the agent need to be listed in this array.")]
        [SerializeField]
        protected WeaponParticle[] particleWeapons;
        public WeaponParticle[] ParticleWeapons
        {
            get
            {
                return particleWeapons;
            }
        }

        /// <summary>
        /// Bool needed to know if it was required to replace particle weapons after an agent
        /// is despawned or not. this replacement process is essential so that the bullets 
        /// that have been already fired by a particle weapon remain active in the scene and
        /// do not suddenly disappear after an agent has been dispawned.
        /// </summary>
        protected bool isUsingParticleWeapons;

        /// <summary>
        /// When set to false, any munition (bullets or missiles) hitting this agent will not 
        /// spawn a hit fx. This will give you a slight performance imporvement since every 
        /// munition hitting this agent undergoes a get component  operation to see if it is 
        /// spawning a hit fx or not.<br></br>
        /// Warning: When this is set to false, The munition hit fx component hitting this
        /// agent will not be active (i.e. you will not see the explosion of a missile hitting this agent.)
        /// </summary>
        [Header("Advanced Settings ")]
        [Space]
        [SB_HelpBox("Warning: When this is set to false, The munition hit fx component hitting this agent" +
            " will not be active! (i.e. you will not see the explosion of a missile hitting this agent.)")]
        [Tooltip("When set to false, any munition (bullets or missiles) hitting this agent will not spawn " +
            "a hit fx. This will give you a slight performance improvement  since every munition hitting " +
            "this agent undergoes a get component  operation to see if it is spawning a hit fx or not.")]
        [SerializeField]
        protected bool isTriggeringMunitionFXWhenHit = true;

        public abstract int OpposingFactionMunitionLayer
        {
            get;
        }
        
        protected virtual void OnValidate()
        {
            Validate();
        }

        /// <summary>
        /// Adds invincibilty events and finds if is using particle weapons.
        /// </summary>
        protected virtual void Awake()
        {
            changingIsInvincibleBool.OnChangeToTrue += RaiseOnInvincibilityStart;
            changingIsInvincibleBool.OnChangeToFalse += RaiseOnInvincibilityEnd;

            if (particleWeapons.Length > 0)
            {
                isUsingParticleWeapons = true;
            }
        }

        /// <summary>
        /// Resets the settings.
        /// </summary>
        protected virtual void OnEnable()
        {
            ResetSettings();
        }

        /// <summary>
        /// Stops all coroutines.
        /// </summary>
        protected virtual void OnDisable()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// processes collisions with munition and checks if is passthrough when invincible.
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            // The following if statement is to protect against very rare cases where 
            // two bullets hit  at the exact same instant causing the despawn method 
            // to be activated twice before giving the object a chance to properly 
            // deactivate and avoid accessing the OnTriggerEnter after the enemy 
            // has despawned.
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (IsInvincible && isPassthroughWhenInvincible)
            {
                return;
            }

            if (collision.gameObject.layer == OpposingFactionMunitionLayer)
            {
                HitByOpposingFactionMunition(collision);
            }
        }

        protected virtual void Validate()
        {
            ValidateHealth();
        }

        private void ValidateHealth()
        {
            if(health <= 0)
            {
                health = 10.0f;
            }
        }

        protected abstract void HitByOpposingFactionMunition(Collider2D collision);

        /// <summary>
        /// Resets the vitals. Due to current health/shield properties protection, please always 
        /// make sure to assign the max then full health/shield before assigning any values to 
        /// currentVitals health/shield.
        /// </summary>
        protected virtual void ResetSettings()
        {
            currentVitals.MaxHealth = health * VitalsMultiplier;
            currentVitals.MaxShield = shield * VitalsMultiplier;

            currentVitals.FullHealth = health * VitalsMultiplier;
            currentVitals.FullShield = shield * VitalsMultiplier;

            currentVitals.Health = health * VitalsMultiplier;
            currentVitals.Shield = shield * VitalsMultiplier;
        }

        /// <summary>
        /// Raises the on activation event.
        /// </summary>
        protected virtual void Activate()
        {
            RaiseOnActivation(new VitalsBarChangeArgs(CurrentVitals));
        }

        /// <summary>
        /// Checks for invinciblity, subtracts the damage value, raises the proper
        /// events and eliminates the agent if necessary.
        /// </summary>
        /// <param name="damage">The value to subtract from the vitals.</param>
        public virtual void TakeHit(float damage)
        {
            if (!IsInvincible)
            {
                DamageWithShield(damage);
                RaiseOnTakeHit();

                if (currentVitals.Health <= 0.0f)
                {
                    Eliminate();
                }
            }
        }

        /// <summary>
        /// Subtracts the damage value while taking into consideration
        /// both the shield and health values.
        /// </summary>
        /// <param name="damage">The value to subtract from the vitals.</param>
        protected virtual void DamageWithShield(float damage)
        {
            if (damage > currentVitals.Shield && currentVitals.Shield > 0.0f)
            {
                float shieldDamageLeftover = damage - CurrentVitals.Shield;

                DamageShield(damage);
                DamageHealth(shieldDamageLeftover);
            }
            else if(currentVitals.Shield <= 0.0f)
            {
                DamageHealth(damage);
            }
            else
            {
                DamageShield(damage);
            }
        }

        private void DamageHealth(float damage)
        {
            currentVitals.Health -= damage;
            RaiseOnHealthDamage(new VitalsBarChangeArgs(CurrentVitals));
        }

        private void DamageShield(float damage)
        {
            currentVitals.Shield -= damage;
            RaiseOnShieldDamage(new VitalsBarChangeArgs(CurrentVitals));
        }

        /// <summary>
        /// Eliminate is called when the agent (A player or enemy) has been eliminated by the opposing faction.
        /// It is different from disable. Disable can be called when the enemy for example is despawned when going
        /// out of the game bounds, while eliminate is only called when the agent has been eliminated by taking hits.
        /// In the case of the player it will also allow for the decrease of lives numbers and in enemies it will
        /// allow for spawning drops, adding score etc... It will also activate the elimination event and any methods
        /// associated with it by the master control or the Sfx.
        /// </summary>
        protected virtual void Eliminate()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (isUsingParticleWeapons)
            {
                ReplaceParticleWeapons();
            }

            RaiseOnElimination();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Subscribes methods to their respective agent event, this enables the execution of 
        /// those methods when the events are invoked.
        /// </summary>
        /// <param name="method">The method you wish to execute when the event is invoked, it has to have the core
        /// delegate signature and use a system argument for its parameter.</param>
        /// <param name="agentEvent">The agent event which will invoke its execution.</param>
        public virtual void Subscribe(CoreDelegate method, AgentEvent agentEvent)
        {
            switch (agentEvent)
            {
                case AgentEvent.Activation:
                    {
                        OnActivation += method;
                        return;
                    }

                case AgentEvent.Elimination:
                    {
                        OnElimination += method;
                        return;
                    }

                case AgentEvent.TakeHit:
                    {
                        OnTakeHit += method;
                        return;
                    }

                case AgentEvent.HealthDamage:
                    {
                        OnHealthDamage += method;
                        return;
                    }

                case AgentEvent.ShieldDamage:
                    {
                        OnShieldDamage += method;
                        return;
                    }

                case AgentEvent.InvincibilityStart:
                    {
                        OnInvincibilityStart += method;
                        return;
                    }

                case AgentEvent.InvincibilityEnd:
                    {
                        OnInvincibilityEnd += method;
                        return;
                    }

                case AgentEvent.TranslationError:
                    {
                        Debug.Log("Agent.cs: agent: " + name + ". Event : " + agentEvent + " failed to subscribe. " +
                            "Have you modified one of the agents (Enemy, Player, etc..) subscribe method " +
                            "or one of the agents enum events? The agent is getting a translation error " +
                            "which is a result of not being able to translate into a proper agent event. " +
                            "Please note this error can come from anything that subscribes to an agent. " +
                            "It can be for example a SFX, an FX Spawner or anything that uses this sub method.");

                        return;
                    }

                default:
                    {
                        Debug.Log("Agent.cs: " + name + ". failed to subscribe the method to the agent event of: " + agentEvent);

                        return;
                    }
            }
        }

        /// <summary>
        /// Decommisions the particle weapons after having duplicated and renested them.
        /// this replacement process is essential so that the bullets 
        /// that have been already fired by a particle weapon remain active in the scene and
        /// do not suddenly disappear after an agent has been dispawned.
        /// </summary>
        protected void ReplaceParticleWeapons()
        {
            for (int i = 0; i < ParticleWeapons.Length; i++)
            {
                if(particleWeapons[i] == null)
                {
                    continue;
                }
                
                WeaponParticle originalParticleWeapon = ParticleWeapons[i];

                WeaponParticle replacementParticleWeapon = Instantiate(
                    originalParticleWeapon,
                    originalParticleWeapon.transform.position,
                    originalParticleWeapon.transform.rotation,
                    originalParticleWeapon.transform.parent);

                replacementParticleWeapon.name = originalParticleWeapon.name;

                originalParticleWeapon.gameObject.transform.parent = null;
                originalParticleWeapon.Decommision();

                ParticleWeapons[i] = replacementParticleWeapon;
            }
        }

        /// <summary>
        /// Invokes the on activation event, you will need to pass a vitals bar change 
        /// arguments so that the UI can be updated with the new vitals value.
        /// </summary>
        /// <param name="args">Arguments that contain the vitals information which will 
        /// be passed in the UI.</param>
        protected void RaiseOnActivation(VitalsBarChangeArgs args)
        {
            OnActivation?.Invoke(args);
        }

        protected void RaiseOnElimination()
        {
            OnElimination?.Invoke(null);
        }

        protected void RaiseOnTakeHit()
        {
            OnTakeHit?.Invoke(null);
        }

        /// <summary>
        /// Invokes the on health damage event, you will need to pass a vitals bar change 
        /// arguments so that the UI can be updated with the new vitals value.
        /// </summary>
        /// <param name="args">Arguments that contain the vitals information which will 
        /// be passed in the UI.</param>
        protected void RaiseOnHealthDamage(VitalsBarChangeArgs args)
        {
            OnHealthDamage?.Invoke(args);
        }

        /// <summary>
        /// Invokes the on shiled damage event, you will need to pass a vitals bar change 
        /// arguments so that the UI can be updated with the new vitals value.
        /// </summary>
        /// <param name="args">Arguments that contain the vitals information which will 
        /// be passed in the UI.</param>
        protected void RaiseOnShieldDamage(VitalsBarChangeArgs args)
        {
            OnShieldDamage?.Invoke(args);
        }

        protected void RaiseOnInvincibilityStart()
        {
            OnInvincibilityStart?.Invoke(null);
        }

        protected void RaiseOnInvincibilityEnd()
        {
            OnInvincibilityEnd?.Invoke(null);
        }
    }
}