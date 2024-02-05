using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Any munition, pickups, backgrounds, background objects and enemies which exit this 
    /// field will be despawned using their appropriate pools.<br></br> 
    /// Please note that the enemies collider trigger for despawning is activated from inside the enemy
    /// script and not this one in order to activate more features that needs access to the enemy script.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Fields/Despawning Field")]
    public class DespawningField : GameField
    {
        protected override Color gizmosColor
        {
            get
            {
                return Color.magenta;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            gameObject.layer = ProjectLayers.DespawningField;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.gameObject.activeSelf)
            {
                return;
            }
            
            int collisionLayer = collision.gameObject.layer;

            switch (collisionLayer)
            {
                case ProjectLayers.PlayerMunition:
                    DespawnPlayerMunition(collision);
                    break;

                case ProjectLayers.EnemyMunition:
                    DespawnEnemyMunition(collision);
                    break;

                case ProjectLayers.Pickups:
                    DespawnPickups(collision);
                    break;

                case ProjectLayers.Backgrounds:
                    DespawnBackgrounds(collision);
                    break;

                case ProjectLayers.BackgroundObjects:
                    DespawnBackgroundObjects(collision);
                    break;
            }
        }

        protected override void Initialize()
        {
            AddRigidBody2D();
            FindFieldBoundries();
            AddBoxCollider2D();
        }

        private static void DespawnPlayerMunition(Collider2D collision)
        {
            if (MunitionPoolPlayer.Instance != null)
            {
                MunitionPoolPlayer.Instance.Despawn(collision.gameObject);
            }
        }

        private static void DespawnEnemyMunition(Collider2D collision)
        {
            if (MunitionPoolEnemy.Instance != null)
            {
                MunitionPoolEnemy.Instance.Despawn(collision.gameObject);
            }
        }

        private static void DespawnPickups(Collider2D collision)
        {
            if (PickupPool.Instance != null)
            {
                PickupPool.Instance.Despawn(collision.gameObject);
            }
        }

        private static void DespawnBackgrounds(Collider2D collision)
        {
            if (ScrollingBackground.Instance != null)
            {
                ScrollingBackground.Instance.ReplaceBackground(collision.gameObject);
            }
        }

        private static void DespawnBackgroundObjects(Collider2D collision)
        {
            if (BackgroundObjectsSpawner.Instance != null)
            {
                BackgroundObjectsSpawner.Instance.Despawn(collision.gameObject);
            }
        }

#if UNITY_EDITOR
        protected override Vector2 GetGizmoLabelPosition()
        {
            return new Vector2(boundries.xMin + ProjectConstants.LabelMarginDistance, boundries.yMin);
        }
#endif
    }
}