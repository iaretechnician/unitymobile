using System.Collections;
using UnityEngine;
using UnityEditor;

namespace ShmupBoss
{
    /// <summary>
    /// The main class in any Level, most of this pack scripts are dependent on it, it determines
    /// the level type, spawns and keeps track of the player, score, coins. Events can also be added to it
    /// when a level starts or ends.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Level/Level")]
    public sealed class Level : MonoBehaviour
    {
        public static Level Instance;
        
        public event CoreDelegate OnPlayerSpawn;
        public event CoreDelegate OnPlayerLivesChange;
        public event CoreDelegate OnLevelStart;
        public event CoreDelegate OnLevelEnd;
        public event CoreDelegate OnGameOver;
        public event CoreDelegate OnScoreChange;
        public event CoreDelegate OnCoinsChange;

        public static bool IsVertical;
        public static bool IsHorizontal;

        /// <summary>
        /// This variable is needed as a preparation for a future update when it's possible to have a 3D level.
        /// At the moment this variable seems superfluous since the current level types do not allow for a
        /// 3D level, but once a 3D level is possible it will be quite essential.
        /// </summary>
        public static bool Is2D;

        /// <summary>
        /// Is set to true when all methods in the initializer have been processed which
        /// typically includes finding the different game fields and pooling the agents, 
        /// effects and munition.
        /// </summary>
        public static bool IsFinishedLoading;

        /// <summary>
        /// This enum is currently used to select whether the level you are building is vertical or horizontal.
        /// Later on this can also be used to select any additional options such as a 3D or 2D level.
        /// </summary>
        [Header("Level")]
        [SerializeField]
        private LevelType levelType;

        /// <summary>
        /// Agents and backgrounds are spawned using a Z depth index which determines their Z position, this 
        /// space determines the distance between those depth indices.
        /// </summary>
        [SerializeField]
        [Tooltip("Agents and backgrounds are spawned using a Z depth index which determines their Z position, " +
            "this space determines the distance between those depth indices.")]
        private float spaceBetween2DDepthIndices = ProjectConstants.DefaultSpaceBetween2DDepthIndices;
        public float SpaceBetween2DDepthIndices
        {
            get
            {
                return spaceBetween2DDepthIndices;
            }
        }

        /// <summary>
        /// Player lives at the start of the level.
        /// </summary>
        [Header("Player")]
        [Tooltip("Player lives at the start of the level.")]
        [Space]
        [SerializeField]
        private int playerLives;
        public int PlayerLives
        {
            get
            {
                return playerLives;
            }
            private set
            {
                if (value > playerMaxLives)
                {
                    playerLives = playerMaxLives;
                }
                else if (value < 0)
                {
                    playerLives = 0;
                }
                else
                {
                    playerLives = value;
                }
            }
        }

        /// <summary>
        /// The maximum number of lives a player can obtain through pickups.
        /// </summary>
        [Tooltip("The maximum number of lives a player can obtain through pickups.")]
        [SerializeField]
        private int playerMaxLives;

        /// <summary>
        /// The time between the player elimination and its respawning.
        /// </summary>
        [Tooltip("The time between the player elimination and its respawning.")]
        [SerializeField]
        private float timeToRespawnPlayer = 1.0f;

        /// <summary>
        /// This determines the prefab source for the player. In case of building a full game where you have
        /// different players to select from, this must be from the game manager, but when testing your
        /// level or when building a game where there is no player selection from a main menu this should 
        /// be from the input you use in the level.
        /// </summary>
        [Tooltip("This determines the prefab source for the player. In case of building a full game where you have " +
            "different players to select from, this must be from the game manager, but when testing your level or " +
            "when building a game where there is no player selection from a main menu this should be from the input you " +
            "use in the level.")]
        [SerializeField]
        private PlayerSource playerSource;

        /// <summary>
        /// If the player source is by level, this component will be used as the prefab 
        /// source for the player, otherwise it will not be used.
        /// </summary>
        [Header("If Source is By Level")]
        [Tooltip("If the player source is by level, this component will be used as the prefab source for the player, " +
            "otherwise it will not be used.")]
        [SerializeField]
        private Player playerComp;
        public Player PlayerComp
        {
            get;
            private set;
        }

        /// <summary>
        /// The player spawn position on the X axis in relation to the play field.
        /// </summary>
        [Header("Player Spawn Position")]
        [Tooltip("The player spawn position on the X axis in relation to the play field.")]
        [Space]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float spawnXPositionRatio = 0.5f;

        /// <summary>
        /// The player spawn position on the Y axis in relation to the play field.
        /// </summary>
        [Tooltip("The player spawn position on the Y axis in relation to the play field.")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float spawnYPositionRatio = 0.5f;

        /// <summary>
        /// The Z depth index the player will be located in.
        /// </summary>
        [Tooltip("The Z depth index the player will be located in.")]
        [SerializeField]
        [Range(0, ProjectConstants.DepthIndexLimit)]
        private int PlayerDepthIndex;

#if UNITY_EDITOR
        [SerializeField]
        private bool isDrawingPlayerSpawnPosition;
        private PlayField playFieldInEditor;
        private Vector2 player2DSpawnPosition;
#endif

        // Note: The variable below is created so that if a 3D level option is added, the space will be calculated 
        // differently, at the moment without a 3D level it is somewhat redundant but this would change if 
        // a 3D level option is added.

        /// <summary>
        /// The global Z depth space between indices.  
        /// </summary>
        public static float SpaceBetweenIndices
        {
            get;
            set;
        }

        /// <summary>
        /// Ued by different trackers to know if they should be tracking the player or not.
        /// </summary>
        public bool IsPlayerAlive
        {
            get;
            private set;
        }

        /// <summary>
        /// Will be verified before declaring level completion so that the level wouldn't be completed in the case when 
        /// the player is eliminated just before all enemies have been eliminated as well.
        /// </summary>
        public bool IsGameOver
        {
            get;
            private set;
        }

        /// <summary>
        /// The player spawn position which is caluclated from the ratio and the player Z depth index.
        /// </summary>
        public Vector3 PlayerSpawnPosition
        {
            get;
            private set;
        }

        public int CurrentScore
        {
            get;
            private set;
        }

        public int CurrentCoins
        {
            get;
            private set;
        }

        /// <summary>
        /// To know if this level is finite or infinite, this information is helpful for the level UI 
        /// and difficulty multiplier.
        /// </summary>
        public static bool IsUsingInfiniteSpawner
        {
            get;
            set;
        }

        private void OnValidate()
        {
            DetermineLevelType();
            ValidateSpaceBetween2DDepthIndices();
            ValidateLives();
        }

        private void Awake()
        {
            Instance = this;

            IsFinishedLoading = false;
            IsGameOver = false;

            InGamePoolManager.ClearIndex();

            DetermineLevelType();

            Initializer.Instance.SubscribeToStage(CreatePlayer, 1);
            Initializer.Instance.SubscribeToStage(InitializePlayerPosition, 3);
            Initializer.Instance.SubscribeToStage(RaiseOnPlayerSpawn, 6);
            Initializer.Instance.SubscribeToStage(SetFinishedLoadingToTrue, 7);
        }

        private void OnDestroy()
        {
            IsFinishedLoading = false;

            StopAllCoroutines();
        }

        /// <summary>
        /// Determines the different static fields which are related to the level type, such as whether the level is 
        /// vertical or horizontal. Most scripts of this pack are dependent on knowing these static variable 
        /// to know how to operate.
        /// </summary>
        private void DetermineLevelType()
        {
            switch (levelType)
            {
                case LevelType.VerticalScrolling2D:
                    {
                        IsVertical = true;
                        IsHorizontal = false;
                        Is2D = true;
                        break;
                    }

                case LevelType.HorizontalScrolling2D:
                    {
                        IsVertical = false;
                        IsHorizontal = true;
                        Is2D = true;
                        break;
                    }

                default:
                    {
                        IsVertical = true;
                        IsHorizontal = false;
                        Is2D = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// Validates the space between 2D level layers is neither zero nor is in the negative.
        /// </summary>
        private void ValidateSpaceBetween2DDepthIndices()
        {
            if (spaceBetween2DDepthIndices <= 0.0f)
            {
                spaceBetween2DDepthIndices = 1.0f;
            }
        }

        /// <summary>
        /// Validates player lives are neither zero nor are in the negative.
        /// </summary>
        private void ValidateLives()
        {
            if (playerLives <= 0)
            {
                playerLives = 1;
            }

            if (playerMaxLives < playerLives)
            {
                playerMaxLives = playerLives;
            }
        }

        private void CreatePlayer()
        {           
            if(playerSource == PlayerSource.ByLevel)
            {
                if (playerComp == null)
                {
                    Debug.Log("Level.cs: you need to input a player in the player comp slot.");
                    return;
                }

                PlayerComp = Instantiate(playerComp);
            }
            else
            {
                GameObject playerGO = null;
                bool canfindGameManagerPlayer = true;

                if (IsVertical)
                {
                    if (GameManager.Instance.PlayerSelectionVertical == null)
                    {
                        Debug.Log("Level.cs: If you select the game manager option " +
                            "for the player in the level script you will need to have a proper player " +
                            "assigned to the main menu scene and to start the level from the " +
                            "main menu.");

                        canfindGameManagerPlayer = false;
                    }
                    else
                    {
                        playerGO = Instantiate((GameObject)GameManager.Instance.PlayerSelectionVertical);
                    }
                }
                else if (IsHorizontal)
                {
                    if (GameManager.Instance.PlayerSelectionHorizontal == null)
                    {
                        Debug.Log("Level.cs: If you select the game manager option " +
                            "for the player in the level script you will need to have a proper player " +
                            "assigned to the main menu scene and to start the level from the " +
                            "main menu.");

                        canfindGameManagerPlayer = false;
                    }
                    else
                    {
                        playerGO = Instantiate((GameObject)GameManager.Instance.PlayerSelectionHorizontal);
                    }
                }

                Player player;

                if (canfindGameManagerPlayer)
                {
                    player = playerGO.GetComponent<Player>();

                    if (player == null)
                    {
                        Debug.Log("Level.cs: was unable to find the player component from the player extracted " +
                            "from the game manager, did you assign a proper player to the game manager " +
                            "in the main menu?");

                        return;
                    }

                    PlayerComp = player;
                }
                else
                {
                    if (playerComp == null)
                    {
                        Debug.Log("Level.cs: attempting to use the level player instead of the game manager and " +
                            "that failed as well, please input a player prefab in the level player slot.");

                        return;
                    }

                    PlayerComp = Instantiate(playerComp);
                }
            }

            PlayerComp.gameObject.layer = ProjectLayers.Player;
            PlayerComp.name = playerComp.name;
            PlayerComp.transform.parent = new GameObject("----- Player -----").transform;
            PlayerComp.gameObject.SetActive(true);

            PlayerComp.Subscribe(InitiateRespawnPlayer, AgentEvent.Elimination);

            IsPlayerAlive = true;
        }

        public void InitiateRespawnPlayer(System.EventArgs eventArgs)
        {
            StartCoroutine(RespawnPlayer());
        }

        IEnumerator RespawnPlayer()
        {
            IsPlayerAlive = false;

            if(PlayerLives <= 0)
            {
                IsGameOver = true;
                RaiseOnGameOver();

                yield break;
            }

            yield return new WaitForSeconds(timeToRespawnPlayer);

            if (PlayerComp == null)
            {
                Debug.Log("Level.cs: is missing its player componenet.");
                yield break;
            }

            PlayerComp.transform.position = PlayerSpawnPosition;
            PlayerComp.gameObject.SetActive(true);
            IsPlayerAlive = true;

            RaiseOnPlayerSpawn();
        }

        private void InitializePlayerPosition()
        {
            if(PlayerComp == null)
            {
                return;
            }

            float zPosition = Dimensions.FindZPositionInPlayField(PlayerDepthIndex);

            if (PlayField.Instance.Boundries.size != Vector2.zero)
            {
                float distanceFromXMin = PlayField.Instance.Boundries.width * spawnXPositionRatio;
                float distanceFromYMin = PlayField.Instance.Boundries.height * spawnYPositionRatio;

                float xPosition = PlayField.Instance.Boundries.xMin + distanceFromXMin;
                float yPosition = PlayField.Instance.Boundries.yMin + distanceFromYMin;

                PlayerSpawnPosition = new Vector3(xPosition, yPosition, zPosition);
            }

            PlayerComp.transform.position = PlayerSpawnPosition;
        }

        private void SetFinishedLoadingToTrue()
        {
            IsFinishedLoading = true;
        }

        public void AdjustPlayerLives(int adjustment)
        {
            PlayerLives += adjustment;

            if(adjustment != 0)
            {
                RaiseOnPlayerLivesChange();
            }
        }

        public void AddScore(int addedScore)
        {
            CurrentScore += addedScore;
            RaiseOnScoreChange(new ScoreArgs(CurrentScore));
        }

        public void AddCoins(int addedCoins)
        {
            CurrentCoins += addedCoins;
            RaiseOnCoinsChange(new CoinsArgs(CurrentCoins));
        }

        public void SetPlayerToInvincible(bool isInvincible)
        {
            if(PlayerComp == null)
            {
                return;
            }

            if (!IsPlayerAlive)
            {
                return;
            }
            
            PlayerComp.IsInvincibleTrigger2 = isInvincible;
        }

        public void NotifyOfLevelStart(LevelArgs levelArgs)
        {
            RaiseOnLevelStart(levelArgs);
        }

        private void RaiseOnLevelStart(LevelArgs levelArgs)
        {
            OnLevelStart?.Invoke(levelArgs);
        }

        public void RaiseOnLevelEnd()
        {
            OnLevelEnd?.Invoke(null);
        }

        private void RaiseOnPlayerSpawn()
        {
            if (PlayerComp == null)
            {
                return;
            }

            OnPlayerSpawn?.Invoke(new PlayerSpawnArgs(PlayerComp, PlayerLives));
        }

        private void RaiseOnPlayerLivesChange()
        {
            if (PlayerComp == null)
            {
                return;
            }

            OnPlayerLivesChange?.Invoke(new PlayerSpawnArgs(PlayerComp, PlayerLives));
        }

        private void RaiseOnGameOver()
        {
            OnGameOver?.Invoke(null);
        }

        private void RaiseOnScoreChange(ScoreArgs args)
        {
            OnScoreChange?.Invoke(args);
        }

        private void RaiseOnCoinsChange(CoinsArgs args)
        {
            OnCoinsChange?.Invoke(args);
        }

#if UNITY_EDITOR
        private void FindPlayer2DSpawnPositionInEditor()
        {
            if (playFieldInEditor == null)
            {
                playFieldInEditor = FindObjectOfType<PlayField>();
            }

            if (playFieldInEditor == null)
            {
                return;
            }

            if (!EditorApplication.isPlaying)
            {
                playFieldInEditor.FindBoundriesInEditor();
            }

            if (playFieldInEditor.Boundries.size != Vector2.zero)
            {
                float distanceFromXMin = playFieldInEditor.Boundries.width * spawnXPositionRatio;
                float distanceFromYMin = playFieldInEditor.Boundries.height * spawnYPositionRatio;

                float xPosition = playFieldInEditor.Boundries.xMin + distanceFromXMin;
                float yPosition = playFieldInEditor.Boundries.yMin + distanceFromYMin;

                player2DSpawnPosition = new Vector2(xPosition, yPosition);
            }
        }

        /// <summary>
        /// This draws the player spawn position.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!isDrawingPlayerSpawnPosition)
            {
                return;
            }
            
            FindPlayer2DSpawnPositionInEditor();

            Gizmos.color = Color.yellow;
            GizmosExtension.DrawCircle(player2DSpawnPosition, 1.0f);
        }
#endif
    }
}