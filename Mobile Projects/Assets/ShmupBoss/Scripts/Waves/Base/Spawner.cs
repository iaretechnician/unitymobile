using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for the finite and infinite spawner.
    /// </summary>
    /// <typeparam name="T">The finite or infinite spawner types.</typeparam>
    public abstract class Spawner<T> : MonoBehaviour where T: MonoBehaviour
    {
        public static T Instance;

        /// <summary>
        /// The delay time for spawning the first enemy wave.
        /// </summary>
        [Tooltip("The delay time for spawning the first enemy wave.")]
        [SerializeField]
        protected float wavesStartTime;

        private int numberOfWavesCurrentlyInLevel;

        /// <summary>
        /// This number is needed to help end the level. When there are no waves left to spawn and there are no 
        /// enemies left active as well. This means that all enemies have been eliminated and the level can finish.
        /// </summary>
        public int NumberOfWavesCurrentlyInLevel
        {
            get
            {
                return numberOfWavesCurrentlyInLevel;
            }
            set
            {
                numberOfWavesCurrentlyInLevel = value;

                if (hasStartedSpawningLastWaveInLevel && value <= 0)
                {
                    if(EnemyPool.Instance == null)
                    {
                        return;
                    }

                    EnemyPool.Instance.OnAllEnemiesDespawned += EndLevel;
                }
            }
        }       

        protected bool hasStartedSpawningLastWaveInLevel;

        /// <summary>
        /// Caches the static instance, resets the level and adds the start level to the 
        /// 4th initializer stage.
        /// </summary>
        protected virtual void Awake()
        {
            Instance = this as T;

            ResetLevel();

            Initializer.Instance.SubscribeToStage(StartLevel, 4);
        }

        protected abstract void StartLevel();

        protected virtual void ResetLevel()
        {
            hasStartedSpawningLastWaveInLevel = false;
            NumberOfWavesCurrentlyInLevel = 0;
        }

        protected abstract void EndLevel(System.EventArgs args);

        /// <summary>
        /// Will check the type of the wave data and spawn the wave according to its type.
        /// </summary>
        /// <param name="waveData">The wave data to be spawned.</param>
        /// <returns></returns>
        protected IEnumerator SpawnWavesByType(WaveData waveData)
        {
            if(waveData == null)
            {
                Debug.Log("Spawner.cs: an empty wave data has somehow been passed to the spawner.");

                yield break;
            }
            
            NumberOfWavesCurrentlyInLevel++;

            int indexOfEnemyToSpawn = 0;
            int enemyPrefabsLength = 0;
            bool isEnemySuccessfullySpawned = false;

            for (int i = 0; i < waveData.NumberOfEnemiesToSpawn; i++)
            {
                if (waveData is SideSpawnableWaveData sideSpawnableWaveData)
                {
                    enemyPrefabsLength = sideSpawnableWaveData.EnemyPrefabs.Length;
                    isEnemySuccessfullySpawned = 
                        WaveDataTools.SpawnSideSpawnableWaveEnemy(sideSpawnableWaveData, indexOfEnemyToSpawn);
                }
                else if (waveData is CurveWaveData curveWaveData)
                {
                    enemyPrefabsLength = curveWaveData.EnemyPrefabs.Length;
                    isEnemySuccessfullySpawned = 
                        WaveDataTools.SpawnCurveWaveEnemy(curveWaveData, indexOfEnemyToSpawn);
                }
                else if (waveData is WaypointWaveData waypointWaveData)
                {
                    enemyPrefabsLength = waypointWaveData.EnemyPrefabs.Length;
                    isEnemySuccessfullySpawned = 
                        WaveDataTools.SpawnWaypointWaveEnemy(waypointWaveData, indexOfEnemyToSpawn);
                }

                if (!isEnemySuccessfullySpawned)
                {
                    continue;
                }

                if (!waveData.IsSpawningEnemiesInRandomOrder)
                {
                    if (indexOfEnemyToSpawn >= enemyPrefabsLength - 1)
                    {
                        indexOfEnemyToSpawn = 0;
                    }
                    else
                    {
                        indexOfEnemyToSpawn++;
                    }
                }

                if (waveData.IsRandomTimeBetweenEnemies)
                {
                    float timeBetweenEnemies = UnityEngine.Random.Range(waveData.MinTimeBetweenEnemies, waveData.MaxTimeBetweenEnemies);

                    if (i < waveData.NumberOfEnemiesToSpawn - 1)
                    {
                        yield return new WaitForSeconds(timeBetweenEnemies);
                    }
                }
                else
                {
                    if (i < waveData.NumberOfEnemiesToSpawn - 1)
                    {
                        yield return new WaitForSeconds(waveData.TimeBetweenEnemies);
                    }
                }
            }

            NumberOfWavesCurrentlyInLevel--;

            yield break;
        }
    }
}