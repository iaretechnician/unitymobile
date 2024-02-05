using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for all the different agents in this pack: player, enemy, enemy detonator and boss sub phase.<br></br>
    /// This (including the agent base class) contain things like vitals, collisions, list of weapons, taking hits, 
    /// invincibility and all the different events.<br></br>
    /// Please note that the agent collidable class inherits from the agent class and that they have been separated into
    /// two different classes instead of being combined to make it easier to understand the classes and debug or 
    /// add any future features. The agent collidable class in itself holds only the collision information, all the vitals
    /// , weapons, hits etc.. are stored in the agent base class.
    /// </summary>
    public abstract class AgentCollidable: Agent
    {
        public event CoreDelegate OnTakeCollision;
        public event CoreDelegate OnDealCollision;

        /// <summary>
        /// The damage caused to the other agent which collides with this agent.
        /// </summary>
        [Header("Collision Settings")]
        [Space]
        [Tooltip("The damage caused to the other agent which collides with this agent.")]
        [SerializeField]
        protected float collisionDamage;
        public virtual float CollisionDamage
        {
            get
            {
                return collisionDamage;
            }
        }

        [SerializeField]
        protected bool canTakeCollisions;

        public bool CanTakeCollisions
        {
            get;
            protected set;
        }

        /// <summary>
        /// The time after taking a collision in which the agent will be immune from further collisions.
        /// </summary>
        [Tooltip("The time after taking a collision in which the agent will be immune from further collisions.")]
        [SerializeField]
        protected float collisionCoolDownTime;

        public abstract int OpposingFactionLayer
        {
            get;
        }

        /// <summary>
        /// This hook is used to process collisions received from the opposing Faction.
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (!CanTakeCollisions)
            {
                return;
            }

            if (IsInvincible)
            {
                return;
            }

            if (collision.gameObject.layer == OpposingFactionLayer)
            {
                HitByOpposingFaction(collision);
            }
        }

        protected virtual void HitByOpposingFaction(Collision2D collision)
        {
            AgentCollidable opposingFactionAgentCollidable = collision.gameObject.GetComponent<AgentCollidable>();

            if (opposingFactionAgentCollidable == null)
            {
                Debug.Log("AgentCollidable.cs: Did you assign an enemy/player layer to something which isn't an enemy/player, " +
                    "or did you forget to add an enemy or player component somewhere? Was unable to get the" +
                    "component of an agentCollidable when was hit by what was supposed to be an agentCollidable.");

                return;
            }

            if (opposingFactionAgentCollidable.CollisionDamage > 0.0f)
            {
                ProcessCollisionHit(opposingFactionAgentCollidable);
            }            
        }
        /// <summary>
        /// Invokes the proper events, initiates the collision cool down and any take hit damage.
        /// </summary>
        /// <param name="opposingFactionAgentCollidable">The other agent which has hit this agent.</param>
        protected virtual void ProcessCollisionHit(AgentCollidable opposingFactionAgentCollidable)
        {
            RaiseOnTakeCollision();
            opposingFactionAgentCollidable.RaiseOnDealCollision();
            
            if (gameObject.activeSelf)
            {
                StartCoroutine(CoolDownAfterCollision(opposingFactionAgentCollidable));
            }
            
            TakeHit(opposingFactionAgentCollidable.CollisionDamage);
        }

        /// <summary>
        /// In addition to resetting the vitals (health/shield), The "CanTakeCollisions" value is also reset.<br></br>
        /// Due to current health/shield properties protection, please always make sure to assign the max then full 
        /// health/shield before assigning any values to current vitals health/shield.
        /// </summary>
        protected override void ResetSettings()
        {
            base.ResetSettings();

            CanTakeCollisions = canTakeCollisions;
        }

        protected virtual IEnumerator CoolDownAfterCollision(AgentCollidable collisionDealingAgent)
        {
            CanTakeCollisions = false;

            yield return new WaitForSeconds(collisionCoolDownTime);

            if (canTakeCollisions)
            {
                CanTakeCollisions = true;

                yield break;
            }
            else
            {
                yield break;
            }
        }

        public override void Subscribe(CoreDelegate method, AgentEvent agentEvent)
        {
            switch (agentEvent)
            {
                case AgentEvent.TakeCollision:
                    {
                        OnTakeCollision += method;
                        return;
                    }

                case AgentEvent.DealCollision:
                    {
                        OnDealCollision += method;
                        return;
                    }
            }

            base.Subscribe(method, agentEvent);
        }

        protected void RaiseOnTakeCollision()
        {
            OnTakeCollision?.Invoke(null);
        }

        protected void RaiseOnDealCollision()
        {
            OnDealCollision?.Invoke(null);
        }
    }
}