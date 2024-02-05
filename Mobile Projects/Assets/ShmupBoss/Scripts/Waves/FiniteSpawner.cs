using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A standard enemy spawner used in a level that can be completed by destroying all the enemies in the level.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Level/Finite Spawner")]
    public class FiniteSpawner : Spawner<FiniteSpawner>
    {
        [Tooltip("All the wave data that are going to be spawned and the time between each one.")]
        [SerializeField]
        private Wave[] waves;
        public Wave[] Waves
        {
            get
            {
                return waves;
            }
        }

        public int CoinsAtStart
        {
            get;
            private set;
        }

        protected override void Awake()
        {
            base.Awake();

            Level.IsUsingInfiniteSpawner = false;

            CoinsAtStart = GameManager.Instance.AccumlatedCoins;
        }

        protected override void StartLevel()
        {
            if (Level.Instance == null)
            {
                Debug.Log("FiniteSpawner.cs: fininte spawner needs a level instance to operate.");
                return;
            }

            Level.Instance.NotifyOfLevelStart(new LevelArgs(0));

            if (EnemyPool.Instance == null)
            {
                Debug.Log("FiniteSpawner.cs: if you want to spawn enemies from a spawner," +
                    " you will need to add an enemy pool to the scene.");

                return;
            }

            StartCoroutine(SpawnAllWaves());

            Level.Instance.SetPlayerToInvincible(false);
        }

        private IEnumerator SpawnAllWaves()
        {
            yield return new WaitForSeconds(wavesStartTime);

            int lastWaveInLevelIndex = Waves.Length - 1;

            for (int i = 0; i < Waves.Length; i++)
            {
                WaveData waveData = Waves[i].waveData;

                if (waveData == null)
                {
                    Debug.Log("FiniteSpawner.cs: Wave slot: " + i + " is empty in the finite spawner, " +
                        "please make sure you are not missing any wave data.");

                    continue;
                }

                if(i >= lastWaveInLevelIndex)
                {
                    hasStartedSpawningLastWaveInLevel = true;
                    StartCoroutine(SpawnWavesByType(waveData));

                    yield break;
                }
                else
                {
                    StartCoroutine(SpawnWavesByType(waveData));
                    yield return new WaitForSeconds(Waves[i].TimeForNextWave);
                }               
            }           
        }        

        /// <summary>
        /// The method is responsible for ending the level and saving the score.
        /// </summary>
        /// <param name="args">System event arguments which are null and are there for convention.</param>
        protected override void EndLevel(System.EventArgs args)
        {
            if (Level.Instance == null)
            {
                return;
            }

            if (Level.Instance.IsGameOver)
            {
                return;
            }

            Level.Instance.SetPlayerToInvincible(true);

            EnemyPool.Instance.OnAllEnemiesDespawned -= EndLevel;

            int currentPlayerScore = Level.Instance.CurrentScore;
            int currentCoins = CoinsAtStart + Level.Instance.CurrentCoins;

            GameManager.Instance.SaveLevelStats(currentPlayerScore, currentCoins, true);

            if (LevelUI.Instance == null)
            {
                return;
            }

            Level.Instance.RaiseOnLevelEnd();
        }
    }
}