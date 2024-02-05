using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Wave data which applies to enemies with waypoint movers only.
    /// </summary>
    [CreateAssetMenu(menuName = "Shmup Boss/Waves/Waypoint Wave")]
    public class WaypointWaveData : WaveData
    {
        /// <summary>
        /// The prefab or prefabs which will be spawned by this wave data. 
        /// These must have a waypoint mover and an enemy or boss component.
        /// </summary>
        [Header("Prefabs To Spawn")]
        [Tooltip("The prefab or prefabs which will be spawned by this wave data. " +
            "These must have a waypoint mover.")]
        [SerializeField]
        protected WaypointMover[] enemyPrefabs;
        public WaypointMover[] EnemyPrefabs
        {
            get
            {
                return enemyPrefabs;
            }
        }
    }
}