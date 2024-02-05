using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Wave data which applies to enemies with curve movers only.
    /// </summary>
    [CreateAssetMenu(menuName = "Shmup Boss/Waves/Curve Wave")]
    public class CurveWaveData : WaveData
    {
        /// <summary>
        /// The prefab or prefabs which will be spawned by this wave data. 
        /// These must have a curve mover and an enemy or boss component.
        /// </summary>
        [Header("Prefabs To Spawn")]
        [SerializeField]
        protected CurveMover[] enemyPrefabs;
        public CurveMover[] EnemyPrefabs
        {
            get
            {
                return enemyPrefabs;
            }
        }
    }
}