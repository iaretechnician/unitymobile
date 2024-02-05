using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Holds methods for translating player and enemy events into agent events.<br></br>
    /// This is needed, because inside the agent script. The subscribe method uses a unified agent event.
    /// </summary>
    public static class AgentEventTranslator
    {
        /// <summary>
        /// Translates a player event into an agent event so that it can be used in the agent subscribe method.
        /// </summary>
        /// <param name="playerEvent">The player event you wish to transalte.</param>
        /// <returns>The translated agent event.</returns>
        public static AgentEvent FromPlayerEvent(PlayerEvent playerEvent)
        {
            switch (playerEvent)
            {
                case PlayerEvent.Activation:
                    return AgentEvent.Activation;

                case PlayerEvent.Elimination:
                    return AgentEvent.Elimination;

                case PlayerEvent.TakeHit:
                    return AgentEvent.TakeHit;

                case PlayerEvent.HealthDamage:
                    return AgentEvent.HealthDamage;

                case PlayerEvent.ShieldDamage:
                    return AgentEvent.ShieldDamage;

                case PlayerEvent.TakeCollision:
                    return AgentEvent.TakeCollision;

                case PlayerEvent.DealCollision:
                    return AgentEvent.DealCollision;

                case PlayerEvent.InvincibilityStart:
                    return AgentEvent.InvincibilityStart;

                case PlayerEvent.InvincibilityEnd:
                    return AgentEvent.InvincibilityEnd;

                case PlayerEvent.PickUp:
                    return AgentEvent.PickUp;

                case PlayerEvent.Heal:
                    return AgentEvent.Heal;

                case PlayerEvent.HealHealth:
                    return AgentEvent.HealHealth;

                case PlayerEvent.HealShield:
                    return AgentEvent.HealShield;

                case PlayerEvent.HealthUpgrade:
                    return AgentEvent.HealthUpgrade;

                case PlayerEvent.ShieldUpgrade:
                    return AgentEvent.ShieldUpgrade;

                case PlayerEvent.InvincibilityPickup:
                    return AgentEvent.InvincibilityPickup;

                case PlayerEvent.LivesPickup:
                    return AgentEvent.LivesPickup;

                case PlayerEvent.WeaponsUpgrade:
                    return AgentEvent.WeaponsUpgrade;

                case PlayerEvent.WeaponsDowngrade:
                    return AgentEvent.WeaponsDowngrade;

                case PlayerEvent.VisualUpgrade:
                    return AgentEvent.VisualUpgrade;

                case PlayerEvent.VisualDowngrade:
                    return AgentEvent.VisualDowngrade;

                case PlayerEvent.SpeedChange:
                    return AgentEvent.SpeedChange;

                case PlayerEvent.PointPickup:
                    return AgentEvent.PointPickup;

                case PlayerEvent.CoinPickup:
                    return AgentEvent.CoinPickup;

                case PlayerEvent.BombsPickup:
                    return AgentEvent.BombsPickup;

                case PlayerEvent.FiringBomb:
                    return AgentEvent.FiringBomb;

                default:
                    Debug.Log("AgentEventTranslator.cs: Failed to translate player event: " + playerEvent);
                    return AgentEvent.TranslationError;
            }
        }

        /// <summary>
        /// Translates an enemy event into an agent event so that it can be used in the agent subscribe method.
        /// </summary>
        /// <param name="enemyEvent">The enemy event you wish to transalte.</param>
        /// <returns>The translated agent event.</returns>
        public static AgentEvent FromEnemyEvent(EnemyEvent enemyEvent)
        {
            switch (enemyEvent)
            {
                case EnemyEvent.Activation:
                    return AgentEvent.Activation;

                case EnemyEvent.Elimination:
                    return AgentEvent.Elimination;

                case EnemyEvent.TakeHit:
                    return AgentEvent.TakeHit;

                case EnemyEvent.HealthDamage:
                    return AgentEvent.HealthDamage;

                case EnemyEvent.ShieldDamage:
                    return AgentEvent.ShieldDamage;

                case EnemyEvent.TakeCollision:
                    return AgentEvent.TakeCollision;

                case EnemyEvent.DealCollision:
                    return AgentEvent.DealCollision;

                case EnemyEvent.InvincibilityStart:
                    return AgentEvent.InvincibilityStart;

                case EnemyEvent.InvincibilityEnd:
                    return AgentEvent.InvincibilityEnd;

                case EnemyEvent.Drop:
                    return AgentEvent.Drop;

                case EnemyEvent.DetonationStart:
                    return AgentEvent.DetonationStart;

                default:
                    Debug.Log("AgentEventTranslator.cs: Failed to translate enemy event: " + enemyEvent);
                    return AgentEvent.TranslationError;
            }
        }
    }
}