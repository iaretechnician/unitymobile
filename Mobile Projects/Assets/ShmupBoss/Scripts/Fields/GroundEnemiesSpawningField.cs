using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This field will give the user additional control of only spawning the ground enemies 
    /// when they enter this field instead of combining it with the playfield since no offset option
    /// is available for the ground enemies unlike the standard enemies spawned by the finite/infinite
    /// spawner.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Fields/Ground Enemies Spawning Field")]
    public class GroundEnemiesSpawningField : GameField
    {
        protected override Color gizmosColor
        {
            get
            {
                return Color.cyan;
            }
        }

        /// <summary>
        /// Makes sure that the any spawned ground enemies will find its proper
        /// hierarchy to be parented to at the treadmill.
        /// </summary>
        private bool canFindGroundEnemiesOnTreadmillHierarchy
        {
            get
            {
                if (Treadmill.Instance == null)
                {
                    return false;
                }

                if (Treadmill.Instance.GroundEnemiesOnTreadmillHierarchy == null)
                {
                    return false;
                }

                return true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ProcessGroundEnemiesPlaceHolders(collision);
        }

        protected override void Initialize()
        {
            AddRigidBody2D();
            FindFieldBoundries();
            AddBoxCollider2D();
        }

        /// <summary>
        /// Take the ground enemy placeholder, and spawns the enemy according to the name of the 
        /// placeholder, parents it under the treadmill then destroys the treamill.
        /// </summary>
        /// <param name="collision"></param>
        private void ProcessGroundEnemiesPlaceHolders(Collider2D collision)
        {
            if (collision.gameObject.layer == ProjectLayers.EnemyPlaceholders)
            {
                if (!canFindGroundEnemiesOnTreadmillHierarchy)
                {
                    return;
                }

                GameObject PulledEnemy = EnemyPool.Instance.Spawn(collision.name);
                PulledEnemy.transform.position = collision.transform.position;

                PulledEnemy.transform.parent = Treadmill.Instance.GroundEnemiesOnTreadmillHierarchy;

                Destroy(collision.gameObject);
            }
        }

#if UNITY_EDITOR
        protected override Vector2 GetGizmoLabelPosition()
        {
            return new Vector2(((boundries.xMin + boundries.xMax) * 0.5f), boundries.yMax);
        }
#endif
    }
}