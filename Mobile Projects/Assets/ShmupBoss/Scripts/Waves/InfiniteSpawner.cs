using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// This spawner is used for building an infinite game, in conjunction with the difficulty multiplier 
    /// it can increment the difficulty as you progress through each sub level. It also has a clear system 
    /// for defining when to spawn waves (stream) which can give a truly infinitely random feel to the game 
    /// you are building.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Level/Infinite Spawner")]
    public class InfiniteSpawner : Spawner<InfiniteSpawner>
    {       
        /// <summary>
        /// The streams that hold all the waves that will be spawned and defines in which 
        /// (sub) levels they will be spawned in.
        /// </summary>
        [Tooltip("The streams that hold all the waves that will be spawned and defines in which sub levels" +
            " they will be spawned in.")]
        [SerializeField]
        private StreamWaves[] streams;
        public StreamWaves[] Streams
        {
            get
            {
                return streams;
            }
        }

        /// <summary>
        /// Current Sub level index, an infinite level is made up of an infinite number of sub levels 
        /// and streams are spawned according to this level index.
        /// </summary>
        private int currentLevelIndex;

        /// <summary>
        /// A list of all waves that are spawned in the current (sub) level, this list is found by 
        /// going through the streams and into their start/end level and every nth level, and finding which 
        /// streams are going to be spawned in this (sub) level.
        /// </summary>
        private List<Wave> currentLevelWaves = new List<Wave>();

        /// <summary>
        /// The array of the current level waves list.
        /// </summary>
        private Wave[] currentLevelWavesArray;

        [Header("Level Number Declartion")]
        [Tooltip("The text which will be replaced with the word Level followed by the current sub level number.")]
        [SerializeField]
        private Text levelDeclare;

        [Tooltip("The time between finishing the sub level in an infinite spawner game and showing the level " +
            "declaration text.")]
        [SerializeField]
        private float levelDeclareDelay;

        [Tooltip("For how long will the level declaration text stay active.")]
        [SerializeField]
        private float levelDeclareStay;

        /// <summary>
        /// Caches the static instance, resets the level, adds the start level to the 4th initializer stage, 
        /// checks streams end level values and the static "IsUsingInfiniteSpawner" bool in the level script.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Level.IsUsingInfiniteSpawner = true;
            currentLevelIndex = 1;
            NumberOfWavesCurrentlyInLevel = 0;

            CheckStreamEndLevel();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// In a stream, if an end level is set to zero or a negative value, it will be set to infinite.
        /// (Technically speaking to the max value an int can hold.)
        /// </summary>
        private void CheckStreamEndLevel()
        {
            for (int i = 0; i < streams.Length; i++)
            {
                if(streams[i].EndLevel <= 0)
                {
                    streams[i].EndLevel = int.MaxValue;
                }
            }
        }

        protected override void StartLevel()
        {
            if (Level.Instance == null)
            {
                Debug.Log("InfiniteSpawner.cs: infinite spawner needs a level instance to operate.");
                return;
            }

            Level.Instance.NotifyOfLevelStart(new LevelArgs(currentLevelIndex));

            if (EnemyPool.Instance == null)
            {
                Debug.Log("InfiniteSpawner.cs: if you want to spawn enemies from a spawner," +
                    " you will need to add an enemy pool to the scene.");

                return;
            }

            PrepareCurrentLevel();
            SpawnCurrentLevel();
        }

        /// <summary>
        /// Finds what streams will be involved in this level, prepares the waves increased amounts in 
        /// accordance to the (sub) level number and then adds those waves to the current level waves.
        /// </summary>
        private void PrepareCurrentLevel()
        {
            currentLevelWaves.Clear();

            for (int i =0; i < Streams.Length; i++)
            {
                if(currentLevelIndex >= Streams[i].StartLevel && currentLevelIndex <= Streams[i].EndLevel)
                {
                    if((currentLevelIndex - Streams[i].StartLevel) % Streams[i].EveryNthLevel == 0)
                    {
                        // This entire block is to find and add the increased enemy amount which requires 
                        // creating a new wave data.
                        if (Streams[i].IncreasedAmount > 0.0f)
                        {
                            Streams[i].HowManyTimesSpawned++;
                            int howManyTimeSpawned = Streams[i].HowManyTimesSpawned;
                            int currentLevelIncreasedAmount = Mathf.CeilToInt(Streams[i].IncreasedAmount * (howManyTimeSpawned-1));

                            for (int j = 0; j < Streams[i].Waves.Length; j++)
                            {
                                if (Streams[i].Waves[j].waveData is SideSpawnableWaveData sideSpawnableWaveData)
                                {
                                    SideSpawnableWaveData sideSpawnableWaveDataToIncrement = Instantiate(sideSpawnableWaveData);
                                    sideSpawnableWaveDataToIncrement.NumberOfEnemiesToSpawn = sideSpawnableWaveData.NumberOfEnemiesToSpawn + currentLevelIncreasedAmount;

                                    Wave incrementedWave = new Wave(sideSpawnableWaveDataToIncrement, Streams[i].Waves[j].TimeForNextWave);
                                    currentLevelWaves.Add(incrementedWave);
                                }
                                else if (Streams[i].Waves[j].waveData is CurveWaveData curveWaveData)
                                {
                                    CurveWaveData curveWaveDataToIncrement = Instantiate(curveWaveData);
                                    curveWaveDataToIncrement.NumberOfEnemiesToSpawn = curveWaveData.NumberOfEnemiesToSpawn + currentLevelIncreasedAmount;

                                    Wave incrementedWave = new Wave(curveWaveDataToIncrement, Streams[i].Waves[j].TimeForNextWave);
                                    currentLevelWaves.Add(incrementedWave);
                                }
                                else if (Streams[i].Waves[j].waveData is WaypointWaveData waypointWaveData)
                                {
                                    WaypointWaveData waypointWaveDataToIncrement = Instantiate(waypointWaveData);
                                    waypointWaveDataToIncrement.NumberOfEnemiesToSpawn = waypointWaveData.NumberOfEnemiesToSpawn + currentLevelIncreasedAmount;

                                    Wave incrementedWave = new Wave(waypointWaveDataToIncrement, Streams[i].Waves[j].TimeForNextWave);
                                    currentLevelWaves.Add(incrementedWave);
                                }
                                else
                                {
                                    currentLevelWaves.Add(Streams[i].Waves[j]);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < Streams[i].Waves.Length; j++)
                            {
                                currentLevelWaves.Add(Streams[i].Waves[j]);
                            }
                        }
                    }                    
                }
            }

            currentLevelWavesArray = new Wave[currentLevelWaves.Count];
            currentLevelWavesArray = currentLevelWaves.ToArray();
        }

        private void SpawnCurrentLevel()
        {
            StartCoroutine(SpawnCurrentLevelWaves());
        }

        private IEnumerator SpawnCurrentLevelWaves()
        {
            yield return new WaitForSeconds(wavesStartTime);

            int indexOfLastWaveInCurrentLevel = currentLevelWavesArray.Length - 1;

            for (int i = 0; i < currentLevelWavesArray.Length; i++)
            {
                WaveData waveData = currentLevelWavesArray[i].waveData;

                if (waveData == null)
                {
                    Debug.Log("InfiniteSpawner.cs: Wave slot: " + i + " is empty in the spawner, " +
                        "please make sure you are not missing anything.");
                    continue;
                }

                StartCoroutine(SpawnWavesByType(waveData));

                if (i >= indexOfLastWaveInCurrentLevel)
                {
                    hasStartedSpawningLastWaveInLevel = true;
                    NumberOfWavesCurrentlyInLevel = NumberOfWavesCurrentlyInLevel;

                    yield break;
                }
                else
                {
                    yield return new WaitForSeconds(currentLevelWavesArray[i].TimeForNextWave);
                }               
            }           
        }

        /// <summary>
        /// The method is responsible for ending the sub level and starting a new one.
        /// </summary>
        /// <param name="args">System event arguments which are null and are there for convention.</param>
        protected override void EndLevel(System.EventArgs args)
        {
            EnemyPool.Instance.OnAllEnemiesDespawned -= EndLevel;

            if (Level.Instance == null)
            {
                return;
            }

            if (Level.Instance.IsGameOver)
            {
                return;
            }

            Level.Instance.RaiseOnLevelEnd();

            if (levelDeclare != null)
            {
                ShowLevelDeclareUI(new LevelArgs(currentLevelIndex + 1));
            }

            try
            {
                StartNewLevel();
            }
            catch
            {

            }
        }

        private void StartNewLevel()
        {
            ResetLevel();

            currentLevelIndex++;

            Multiplier.Instance.CurrentInfiniteLevelIndex = currentLevelIndex;

            Level.Instance.NotifyOfLevelStart(new LevelArgs(currentLevelIndex));

            PrepareCurrentLevel();
            SpawnCurrentLevel();
        }

        private void ShowLevelDeclareUI(System.EventArgs args)
        {
            LevelArgs levelArgs = args as LevelArgs;

            if (levelArgs == null)
            {
                return;
            }

            try
            {
                StartCoroutine(LevelDeclareRoutine(levelArgs.LevelIndex));
            }
            catch
            {

            }
        }

        private IEnumerator LevelDeclareRoutine(int levelIndex)
        {
            yield return new WaitForSeconds(levelDeclareDelay);

            levelDeclare.gameObject.SetActive(true);
            levelDeclare.text = "Level " + levelIndex.ToString();

            yield return new WaitForSeconds(levelDeclareStay);

            levelDeclare.gameObject.SetActive(false);
        }

        /// <summary>
        /// Only used in the editor so that you can have a better understanding of what streams 
        /// are spawned in what levels.
        /// </summary>
        /// <param name="levelsLimit">The number of levels which will be previews in the logger.</param>
        public void PrintLevelStreams(int levelsLimit)
        {
            int testLevelIndex = 1;
            
            if(levelsLimit < 1)
            {
                return;
            }

            List<int> streamsNumberList = new List<int>();

            while (testLevelIndex <= levelsLimit)
            {
                streamsNumberList.Clear();

                for (int i = 0; i < Streams.Length; i++)
                {
                    bool isStreamInfinite = false;
                    bool isTestLevelIndexLessThanEndLevel = false;

                    if (Streams[i].EndLevel <= 0)
                    {
                        isStreamInfinite = true;
                    }

                    if (isStreamInfinite || testLevelIndex <= Streams[i].EndLevel)
                    {
                        isTestLevelIndexLessThanEndLevel = true;
                    }

                    if (testLevelIndex >= Streams[i].StartLevel && isTestLevelIndexLessThanEndLevel)
                    {
                        if ((testLevelIndex - Streams[i].StartLevel) % Streams[i].EveryNthLevel == 0)
                        {
                            streamsNumberList.Add(i);
                        }
                    }
                }

                Debug.Log("Level: " + testLevelIndex + ". Contains streams: " + 
                    System.String.Join(", ", streamsNumberList.ConvertAll(i => i.ToString()).ToArray()));

                testLevelIndex++;
            }
        }
    }
}