using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace ShmupBoss
{
    /// <summary>
    /// Handles information which needs to remain across levels and the main menu, such as player selection, 
    /// saved data and settings. It also handles moving from one scene to another.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Main Menu/Game Manager")]
    public class GameManager : PersistentSingleton<GameManager>
    {
        /// <summary>
        /// The background option when you build vertical levels for desktop.
        /// </summary>
        [Header("Force Asepct Ratio Background")]
        [Tooltip("The background option when you build vertical levels for desktop " +
            "and the force aspect ratio setting is enabled.")]
        [SerializeField]
        private ScreenCreatorOption screenCreatorOption;

        /// <summary>
        /// The background color that will cover the empty area on a desktop screen if 
        /// you have forced the aspect ratio for a vertical desktop level.
        /// </summary>
        [Header("If Option Is Color")]
        [Tooltip("The background color that will cover the empty area on a desktop screen if you " +
            "have forced the aspect ratio for a vertical desktop level.")]
        [SerializeField]
        private Color backgroundColor;

        /// <summary>
        /// The background image that will cover the empty area on a desktop screen if 
        /// you have forced the aspect ratio for a vertical desktop level.
        /// </summary>
        [Header("If Option Is Image")]
        [Tooltip("The background image that will cover the empty area on a desktop screen if you " +
            "have forced the aspect ratio for a vertical desktop level.")]
        [SerializeField]
        private Sprite backgroundSprite;

        /// <summary>
        /// The build index of the main menu scene.<br></br>
        /// </summary>
        [Header("Scene Indices")]
        [Tooltip("The build index of the main menu scene. In build settings, the build index is the number " +
            "next to the scene in the scenes in build.")]
        [Space]
        [SerializeField]
        private int MainMenuSceneIndex;

        /// <summary>
        /// The build index of the very first level of the game you are building.<br></br>
        /// </summary>
        [Tooltip("The build index of the first level of the game. In build settings, the build index is the number " +
            "next to the scene in the scenes in build.")]
        [SerializeField]
        private int minLevelSceneIndex;
        public int MinLevelSceneIndex
        {
            get
            {
                return minLevelSceneIndex;
            }
        }

        /// <summary>
        /// The build index of the last level of the game you are building.<br></br>
        /// </summary>
        [Tooltip("The build index of the last level of the game. In Unity build settings, the build index " +
            "is the number next to the scene in the scenes in build.")]
        [SerializeField]
        private int maxLevelSceneIndex;

        public int LevelsNumber
        {
            get
            {
                return maxLevelSceneIndex - MinLevelSceneIndex + 1;
            }
        }

        [Header("Player")]
        [Tooltip("A list of all possible player selections in the game." + "\n\n" + 
            "The amount you put here needs to match what you have in the UI, player menu, avatar canvases." + "\n\n" +
            "If you are only using one type of level in your game, such as vertical for example you will " +
            "not need to input a player for the horizontal version.")]
        [SerializeField]
        private PlayerSelection[] playerSelections;

        private int selectedPlayerIndex;
        public int SelectedPlayerIndex
        {
            get
            {
                return selectedPlayerIndex;
            }
            set
            {
                if(value < 0)
                {
                    selectedPlayerIndex = 0;
                }
                else if(playerSelections == null)
                {
                    selectedPlayerIndex = 0;
                }
                else if(value > playerSelections.Length - 1)
                {
                    selectedPlayerIndex = playerSelections.Length - 1;
                }
                else
                {
                    selectedPlayerIndex = value;
                }
            }
        }

        /// <summary>
        /// The player which has been selected by the user to play the game with from the player select menu.
        /// </summary>
        private PlayerSelection playerSelection;

        /// <summary>
        /// The prefab for the selected player which will be used in any vertical level.
        /// </summary>
        public UnityEngine.Object PlayerSelectionVertical
        {
            get
            {
                if (playerSelection == null)
                {
                    return null;
                }
                else
                {
                    return playerSelection.Vertical;
                }
            }
        }

        /// <summary>
        /// The prefab for the selected player which will be used in any horizontal level.
        /// </summary>
        public UnityEngine.Object PlayerSelectionHorizontal
        {
            get
            {
                if (playerSelection == null)
                {
                    return null;
                }
                else
                {
                    return playerSelection.Horizontal;
                }
            }
        }

        [Header("Events")]
        [Tooltip("If you do not have an event system already in the scene, you can easily create one by " +
            "right clicking in hierarchy: UI -> Event System")]
        [SerializeField]
        private EventSystem eventSystem;


        /// <summary>
        /// The current difficulty level, volume options, auto fire and input method.
        /// </summary>
        public GameSettings CurrentGameSettings
        {
            get;
            private set;
        }

        /// <summary>
        /// The current saved levels progress and selected player index.
        /// </summary>
        public SaveInfo CurrentSaveInfo
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// How many coins have been accumlated throught the game, coins are saved when a level is 
        /// complete or when a game is over and all added up.<br></br>
        /// This value is currently unused but sets the base if you want to use coins for any later expansion
        /// of this pack and adding your own custom written scripts.
        /// </summary>
        public int AccumlatedCoins
        {
            get;
            private set;
        }

        protected override void Awake()
        {
            base.Awake();

            SaveLoadManager.VerifySaveFolder();

            LoadSaveInfo();
            LoadGameSettings();
            FindEventSystem();
        }

        private void LoadSaveInfo()
        {
            CurrentSaveInfo = SaveLoadManager.LoadFile<SaveInfo>(ProjectConstants.SaveFileName);

            if (CurrentSaveInfo == null)
            {
                CurrentSaveInfo = new SaveInfo(LevelsNumber);
                return;
            }

            // In case you have made a new build which changes the number of levels while the save  
            // file still stores the old number of levels.
            if (CurrentSaveInfo.SavedLevels.Length != LevelsNumber)
            {
                CurrentSaveInfo = new SaveInfo(LevelsNumber);
                return;
            }

            SelectedPlayerIndex = CurrentSaveInfo.SelectedPlayerIndex;
            AccumlatedCoins = CurrentSaveInfo.AccumulatedCoins;
        }

        private void SaveSaveInfo()
        {
            SaveLoadManager.SaveFile(ProjectConstants.SaveFileName, CurrentSaveInfo);
        }

        private void LoadGameSettings()
        {
            CurrentGameSettings = SaveLoadManager.LoadFile<GameSettings>(ProjectConstants.SettingsFileName);

            if (CurrentGameSettings == null)
            {
                CurrentGameSettings = new GameSettings();
            }
        }

        public void SaveGameSettings()
        {
            SaveLoadManager.SaveFile(ProjectConstants.SettingsFileName, CurrentGameSettings);
        }

        public void AssignSelectedPlayer(int index)
        {
            if(playerSelections == null || playerSelections.Length < 1)
            {
                Debug.Log("GameManager.cs: If you plan to use the main menu, make sure you start the game from " +
                    "the main menu and to have filled the game manager inside the main menu with the needed player " +
                    "prefabs selectiosn.");                

                return;
            }
            
            if (index < 0)
            {
                index = 0;
            }
            else if (index >= playerSelections.Length)
            {
                index = playerSelections.Length - 1;
            }

            playerSelection = playerSelections[index];
            SelectedPlayerIndex = index;
            CurrentSaveInfo.SelectedPlayerIndex = index;

            SaveSaveInfo();
        }

        /// <summary>
        /// Saves the score and coins after a level has ended, and if it has been completed it will also open 
        /// up any levels ahead of it.
        /// </summary>
        /// <param name="newScore">The current score achieved through the level.</param>
        /// <param name="newlyAccumlatedCoins">The current accumlated score.</param>
        /// <param name="isLevelComplete">If true, it will open up the level after it if any existed.</param>
        /// <returns></returns>
        public bool SaveLevelStats(int newScore, int newlyAccumlatedCoins, bool isLevelComplete)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            int levelsArrayIndex = sceneIndex - MinLevelSceneIndex;

            if (!isIndexWithinLevelsRange(sceneIndex))
            {
                return false;
            }

            if (isLevelComplete)
            {
                CurrentSaveInfo.SavedLevels[levelsArrayIndex].Status = LevelStatus.Completed;

                if(sceneIndex +1 <= maxLevelSceneIndex)
                {
                    CurrentSaveInfo.SavedLevels[levelsArrayIndex + 1].Status = LevelStatus.Ongoing;

                    if (Level.Instance != null && Level.Instance.PlayerComp != null)
                    {
                        CurrentSaveInfo.SavedLevels[levelsArrayIndex + 1].WeaponsUpgradeLevel = Level.Instance.PlayerComp.CurrentWeaponsUpgradeStage;
                        CurrentSaveInfo.SavedLevels[levelsArrayIndex + 1].VisualUpgradeLevel = Level.Instance.PlayerComp.CurrentVisualUpgradeStage;
                    }
                }
            }

            if (newScore > CurrentHighScore())
            {
                CurrentSaveInfo.SavedLevels[levelsArrayIndex].HighScore = newScore;
            }

            CurrentSaveInfo.AccumulatedCoins = newlyAccumlatedCoins;
            AccumlatedCoins = newlyAccumlatedCoins;

            SaveSaveInfo();
            return true;
        }

        private bool isIndexWithinLevelsRange(int sceneIndex)
        {
            if (sceneIndex < MinLevelSceneIndex || sceneIndex > maxLevelSceneIndex)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the current high score of the level currently being played from the saved info.
        /// </summary>
        /// <returns>The high score of the level currently played.</returns>
        public int CurrentHighScore()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (!isIndexWithinLevelsRange(sceneIndex))
            {
                return 0;
            }

            return CurrentSaveInfo.SavedLevels[sceneIndex - MinLevelSceneIndex].HighScore;
        }

        public LevelStatus GetCurrentLevelStatus()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            int levelIndex = sceneIndex - MinLevelSceneIndex;
            if (levelIndex >= 0 && levelIndex <= maxLevelSceneIndex - MinLevelSceneIndex)
            {
                return CurrentSaveInfo.SavedLevels[levelIndex].Status;
            }
            else
            {
                return LevelStatus.NotAvailable;
            }
        }

        public void ToMainScene()
        {
            StartCoroutine(LoadLevel(MainMenuSceneIndex));
        }

        public void ToFirstLevel()
        {
            StartCoroutine(LoadLevel(MinLevelSceneIndex));
        }

        public void ToNextLevel()
        {
            int nextLevelSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextLevelSceneIndex > maxLevelSceneIndex)
            {
                return;
            }

            StartCoroutine(LoadLevel(nextLevelSceneIndex));
        }

        public void ReloadLevel()
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
        }

        public bool ToLevel(int levelIndex)
        {
            if (levelIndex >= MinLevelSceneIndex && levelIndex <= maxLevelSceneIndex)
            {
                StartCoroutine(LoadLevel(levelIndex));
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator LoadLevel(int sceneIndex)
        {
            if(screenCreatorOption == ScreenCreatorOption.Color)
            {
                ScreenCreator.CreateScreen(backgroundColor);
            }
            else if(screenCreatorOption == ScreenCreatorOption.Image)
            {
                if (backgroundSprite == null)
                {
                    Debug.Log("GameManager.cs: You have choosen an image" +
                        " as a screen creator optio yet you have forgotten to assing an image to it.");

                    ScreenCreator.CreateScreen();
                }
                else
                {
                    ScreenCreator.CreateScreen(backgroundSprite);
                }
            }

            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneIndex);

            while (!asyncLoadLevel.isDone)
            {
                Logger.Print("GameManager.cs: Loading the Scene");
                yield return null;
            }
        }

        public int GetCurrentLevelIndex()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentSceneIndex >= MinLevelSceneIndex && currentSceneIndex <= maxLevelSceneIndex)
            {
                return currentSceneIndex - MinLevelSceneIndex + 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Sets a game object as a selected UI element, this is useful to select a default UI button to be 
        /// pressed without having to manually navigate to it.
        /// </summary>
        /// <param name="selected">The game object which will be selected.</param>
        public static void SetSelectedUI(GameObject selected)
        {
            if(Instance.eventSystem == null)
            {
                Instance.FindEventSystem();
            }
            
            if (Instance.eventSystem != null)
            {
                Instance.eventSystem.SetSelectedGameObject(selected);
            }
        }

        private void FindEventSystem()
        {
            if (eventSystem == null)
            {
                eventSystem = FindObjectOfType<EventSystem>();
            }

            if(eventSystem == null)
            {
                eventSystem = GetComponent<EventSystem>();
            }

            if (eventSystem == null)
            {
                eventSystem = gameObject.AddComponent<EventSystem>();
            }
        }
    }
}