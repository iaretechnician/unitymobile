using System;
using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// Contains all the UI elements needed inside a level which includes: score, lives, vitals, 
    /// level complete canvas, game over canvas, etc..
    /// </summary>
    [AddComponentMenu("Shmup Boss/Level/Level UI")]
    public class LevelUI : MonoBehaviour
    {
        public static LevelUI Instance;
        public static event CoreDelegate OnUIEvent;

        [Tooltip("If the level is vertical, built for dekstop and its camera is using a vertical aspect ratio; " +
            "Checking this will make the UI forced with that aspect ratio as well. Otherwise the UI will always " +
            "fill the canvas.")]
        [SerializeField]
        private bool isForcedIntoCameraAspectRatio = true;

        [Header("HUD")]
        [Tooltip("The UI canvas which holds all the heads up display such as vitals sliders, score, lives, etc..")]
        [SerializeField]
        private RectTransform hudCanvas;

        [Tooltip("The button which will pause the game and bring up the pause menu.")]
        [SerializeField]
        private Button hudPauseButton;

        [Tooltip("This text will be changed into an X character with the number of lives currently available.")]
        [SerializeField]
        private Text playerLives;

        [SerializeField]
        private Text bombsCount;

        [Tooltip("This text will show the current accumulated score during this level.")]
        [SerializeField]
        private Text playerScore;

        [Tooltip("This text will show the current accumulated coins during this level.")]
        [SerializeField]
        private Text playerCoins;

        [Tooltip("This text will show the highest score accumlated previously. It will only store the " +
            "high score after you have finished the level or the game is over.")]
        [SerializeField]
        private Text highScore;

        [Tooltip("The slider which will display the current player health.")]
        [SerializeField]
        private VitalsSlider healthSlider;

        [Tooltip("The slider which will display the current player shield.")]
        [SerializeField]
        private VitalsSlider shieldSlider;

        [Header("Pause Game")]
        [Tooltip("The UI canvas which holds all the pause menu controls.")]
        [SerializeField]
        private RectTransform pauseGameCanvas;

        [Tooltip("The button which when clicked will hide the pause menu and continue the game which " +
            "was previously paused.")]
        [SerializeField]
        private Button pauseGameResumeButton;

        [Tooltip("This button will exit the current level and return to the main menu. (Home button)")]
        [SerializeField]
        private Button pauseGameMainButton;

        [Tooltip("This button will restart the current level.")]
        [SerializeField]
        private Button pauseGameRetryButton;

        [Tooltip("The slider which will control the music volume.")]
        [SerializeField]
        private Slider pauseMusicSlider;

        [Tooltip("The slider which will control the SFX volume.")]
        [SerializeField]
        private Slider pauseSoundFXSlider;

        [Header("Game Over")]
        [Tooltip("The UI Game over canvas which holds the score, retry and home buttons.")]
        [SerializeField]
        private RectTransform gameOverCanvas;

        [Tooltip("The highest score achieved so far in this level.")]
        [SerializeField]
        private Text gameOverHighScore;

        [Tooltip("The score currently accumulated.")]
        [SerializeField]
        private Text gameOverCurrentScore;

        [Tooltip("This button will take you back to the home main menu.")]
        [SerializeField]
        private Button gameOverMainButton;

        [Tooltip("This button will restart the level.")]
        [SerializeField]
        private Button gameOverRetryButton;

        [Header("Level Complete")]
        [Tooltip("The canvas which will appear when the level is completed, showing the score and the " +
            "home and next level buttons.")]
        [SerializeField]
        private RectTransform levelCompleteCanvas;

        [Tooltip("The highest score achieved so far in this level.")]
        [SerializeField]
        private Text levelCompleteHighScore;

        [Tooltip("The score currently accumulated.")]
        [SerializeField]
        private Text levelCompleteCurrentScore;

        [Tooltip("This will take you to the next level.")]
        [SerializeField]
        private Button levelCompleteNextLevelButton;

        [Tooltip("This button will take you to the home main menu.")]
        [SerializeField]
        private Button levelCompleteMainButton;

        [Tooltip("This button will restart the level.")]
        [SerializeField]
        private Button levelCompleteRetryButton;

        [Header("Mobile Virtual Controls")]
        [Tooltip("This canvas holds the controls used when building for mobile and are only visible when " +
            "the game is built for Android or IOS.")]
        [SerializeField]
        private Canvas virtualControlsCanvas;

        [Tooltip("The virtual stick which will control the player direction if the build is for mobile.")]
        [SerializeField]
        private Joystick virtualJoyStick;
        public Joystick VirtualJoystick
        {
            get
            {
                return virtualJoyStick;
            }
        }

        [Tooltip("The button which will cause the player to fire if auto fire is disabled, if auto fire is " +
            "enabled this button will be hidden.")]
        [SerializeField]
        private HoldButton virtualFire1Button;
        public HoldButton VirtualFire1Button
        {
            get
            {
                return virtualFire1Button;
            }
        }

        [Header("Mobile Bomb Button")]
        [SerializeField]
        private Canvas virtualBombCanvas;

        [SerializeField]
        private HoldButton virtualBombButton;
        public HoldButton VirtualBombButton
        {
            get
            {
                return virtualBombButton;
            }
        }

        /// <summary>
        /// This is needed to prevent pause being pressed when a level complete or a game 
        /// over canvas is already active.
        /// </summary>
        private bool isPauseAllowed = true;

        private bool areVirtualControlsNeeded = false;
        private bool isMobileBombButtonNeeded = false;

        public int CoinsAtStart
        {
            get;
            private set;
        }

        private void Awake()
        {
            Instance = this;

            Initializer.Instance.SubscribeToStage(InitializeLevelUI, 5);
        }

        private void Update()
        {
            if (PlayerInput.IsCancelPressed)
            {
                Pause();
            }
        }

        private void InitializeLevelUI()
        {
            CoinsAtStart = GameManager.Instance.AccumlatedCoins;

            SubscribeToLevelEvents();

            InitializeHUDPauseButton();
            InitializeHudHighScore();
            InitializePauseUI();
            InitializeGameOverUI();
            InitializeLevelCompleteUI();

            if (isForcedIntoCameraAspectRatio)
            {
                AdjustUIToForceCameraAspectRatio();
            }

            isPauseAllowed = true;

            InitializeBombsCount();
        }

        private void InitializeBombsCount()
        {
            if(Bomb.Instance == null)
            {
                return;
            }
            
            UpdateBombsCount(new BombArgs(Bomb.Instance.BombsCount));
        }

        private void SubscribeToLevelEvents()
        {
            if(Level.Instance == null)
            {
                Debug.Log("LevelUI.cs: level instance is needed, a level UI cannot function without the " +
                    "level instance.");

                return;
            }
            
            Level.Instance.OnPlayerSpawn += UpdateHud;
            Level.Instance.OnPlayerLivesChange += UpdatePlayerLives;
            Level.Instance.OnScoreChange += UpdateScore;
            Level.Instance.OnCoinsChange += UpdateCoins;
            Level.Instance.OnGameOver += GameOver;

            if (!Level.IsUsingInfiniteSpawner)
            {
                Level.Instance.OnLevelEnd += FiniteLevelComplete;
            }

            if(Bomb.Instance != null)
            {
                Bomb.Instance.OnBombsCountChange += UpdateBombsCount;
            }
        }

        private void InitializeHUDPauseButton()
        {
            if (hudPauseButton != null)
            {
                hudPauseButton.onClick.AddListener(Pause);
            }
        }

        private void InitializeHudHighScore()
        {
            if (highScore != null)
            {
                highScore.text = "High Score: " + (GameManager.Instance.CurrentHighScore().ToString());
            }
        }

        private void InitializePauseUI()
        {
            if (pauseGameCanvas != null)
            {
                pauseGameCanvas.gameObject.SetActive(false);
            }

            if (pauseGameResumeButton != null)
            {
                pauseGameResumeButton.onClick.AddListener(Resume);
            }

            if (pauseGameMainButton != null)
            {
                pauseGameMainButton.onClick.AddListener(MainMenu);
            }

            if (pauseGameRetryButton != null)
            {
                pauseGameRetryButton.onClick.AddListener(Retry);
            }

            if (pauseMusicSlider != null)
            {
                pauseMusicSlider.onValueChanged.AddListener(UpdateMusicVolume);
            }

            if (pauseSoundFXSlider != null)
            {
                pauseSoundFXSlider.onValueChanged.AddListener(UpdateSFXVolume);
            }
        }

        private void InitializeGameOverUI()
        {
            if (gameOverCanvas != null)
            {
                gameOverCanvas.gameObject.SetActive(false);
            }

            if (gameOverCurrentScore != null)
            {
                gameOverCurrentScore.text = "";
            }

            if (gameOverHighScore != null)
            {
                gameOverHighScore.text = "";
            }

            if (gameOverRetryButton != null)
            {
                gameOverRetryButton.onClick.AddListener(Retry);
            }

            if (gameOverMainButton != null)
            {
                gameOverMainButton.onClick.AddListener(MainMenu);
            }
        }

        private void InitializeLevelCompleteUI()
        {
            if (levelCompleteCanvas != null)
            {
                levelCompleteCanvas.gameObject.SetActive(false);
            }

            if (levelCompleteCurrentScore != null)
            {
                levelCompleteCurrentScore.text = "";
            }

            if (levelCompleteHighScore != null)
            {
                levelCompleteHighScore.text = "";
            }

            if (levelCompleteMainButton != null)
            {
                levelCompleteMainButton.onClick.AddListener(MainMenu);
            }

            if (levelCompleteRetryButton != null)
            {
                levelCompleteRetryButton.onClick.AddListener(Retry);
            }

            if (levelCompleteNextLevelButton != null)
            {
                levelCompleteNextLevelButton.onClick.AddListener(NextLevel);
            }
        }

        private void AdjustUIToForceCameraAspectRatio()
        {
            if (LevelCamera.Instance == null)
            {
                return;
            }

            LevelCamera levelCamera = LevelCamera.Instance;

            if (levelCamera._VerticalAspectRatio == VerticalAspectRatio.None)
            {
                return;
            }

            float unusedWidth = levelCamera.GetUnusedWidth(hudCanvas);

            if (hudCanvas != null)
            {
                levelCamera.AdujstCanvasToForceAspect(hudCanvas, unusedWidth);
            }

            if (pauseGameCanvas != null)
            {
                levelCamera.AdujstCanvasToForceAspect(pauseGameCanvas, unusedWidth);
            }

            if (gameOverCanvas != null)
            {
                levelCamera.AdujstCanvasToForceAspect(gameOverCanvas, unusedWidth);
            }

            if (levelCompleteCanvas != null)
            {
                levelCamera.AdujstCanvasToForceAspect(levelCompleteCanvas, unusedWidth);
            }

            if (virtualControlsCanvas != null)
            {
                levelCamera.AdujstCanvasToForceAspect(virtualControlsCanvas.gameObject.GetComponent<RectTransform>(), unusedWidth);
            }

            if (virtualBombCanvas != null)
            {
                levelCamera.AdujstCanvasToForceAspect(virtualBombCanvas.gameObject.GetComponent<RectTransform>(), unusedWidth);
            }
        }

        public void UpdateMusicVolume(float value)
        {
            AudioManager.Instance.MusicVolume = value;
        }

        public void UpdateSFXVolume(float value)
        {
            AudioManager.Instance.SfxVolume = value;
        }

        private void UpdateHud(System.EventArgs args)
        {
            PlayerSpawnArgs playerSpawnArgs = args as PlayerSpawnArgs;

            if (playerSpawnArgs == null)
            {
                return;
            }

            if (healthSlider != null)
            {
                healthSlider.Initialize(playerSpawnArgs.PlayerComponent);
            }

            if (shieldSlider != null)
            {
                shieldSlider.Initialize(playerSpawnArgs.PlayerComponent);
            }

            UpdatePlayerLives(args);
        }

        private void UpdatePlayerLives(System.EventArgs args)
        {
            PlayerSpawnArgs playerSpawnArgs = args as PlayerSpawnArgs;
          
            if (playerLives != null)
            {
                playerLives.text = "X" + playerSpawnArgs.PlayerLives;
            }
        }

        private void UpdateBombsCount(System.EventArgs args)
        {
            BombArgs bombsArgs = args as BombArgs;

            if (bombsCount != null)
            {
                bombsCount.text = "B" + bombsArgs.BombCount;
            }
        }

        private void UpdateScore(System.EventArgs args)
        {
           ScoreArgs scoreArgs = args as ScoreArgs;

            if (scoreArgs == null)
            {
                return;
            }

            if (playerScore == null)
            {
                return;
            }

            playerScore.text = "Score : " + scoreArgs.CurrentScore.ToString();
        }

        private void UpdateCoins(System.EventArgs args)
        {
            CoinsArgs coinsArgs = args as CoinsArgs;

            if(coinsArgs == null)
            {
                return;
            }

            if (playerCoins == null)
            {
                return;
            }

            playerCoins.text = "Coins : " + coinsArgs.CurrentCoins.ToString();
        }

        private void GameOver(System.EventArgs args)
        {
            if(Level.Instance == null)
            {
                return;
            }
            
            int currentPlayerScore = Level.Instance.CurrentScore;
            int currentCoins = CoinsAtStart + Level.Instance.CurrentCoins;

            GameManager.Instance.SaveLevelStats(currentPlayerScore, currentCoins, false);

            EnableGameOverUI(GameManager.Instance.CurrentHighScore(), 
                currentPlayerScore, 
                ProjectConstants.GameOverDelayTime);
        }

        private void EnableGameOverUI(float highScore, float currentScore, float delay)
        {
            isPauseAllowed = false;
            HideVirtualControls();
            RaiseOnUIEvent(new LevelUIEventArgs(LevelUIEvent.GameOverCanvasAppear));

            if (gameOverCanvas != null)
            {
                gameOverCanvas.gameObject.SetActive(true);
                GameManager.SetSelectedUI(gameOverRetryButton.gameObject);
            }

            if (gameOverCurrentScore != null)
            {
                gameOverCurrentScore.text = currentScore.ToString();
            }

            if (gameOverHighScore != null)
            {
                gameOverHighScore.text = highScore.ToString();
            }
        }

        public void MainMenu()
        {
            RaiseOnUIEvent(new LevelUIEventArgs(LevelUIEvent.MainMenuButtonPressed));
            GameManager.Instance.ToMainScene();
            TimeManager.UnPause();
        }

        public void Retry()
        {
            RaiseOnUIEvent(new LevelUIEventArgs(LevelUIEvent.RetryButtonPressed));
            GameManager.Instance.ReloadLevel();
            TimeManager.UnPause();
        }

        public void NextLevel()
        {
            RaiseOnUIEvent(new LevelUIEventArgs(LevelUIEvent.NextLevelButtonPressed));
            GameManager.Instance.ToNextLevel();
        }

        private void Pause()
        {
            if (!isPauseAllowed)
            {
                return;
            }

            if (pauseGameCanvas != null)
            {
                pauseGameCanvas.gameObject.SetActive(true);
                GameManager.SetSelectedUI(pauseGameResumeButton.gameObject);
            }

            TimeManager.Pause();

            if (AudioManager.IsInitialized)
            {
                AudioManager.Instance.PauseMusic();
            }

            HideVirtualControls();

            RaiseOnUIEvent(new LevelUIEventArgs(LevelUIEvent.PauseButtonPressed));
        }

        public void Resume()
        {
            TimeManager.UnPause();

            if (AudioManager.IsInitialized)
            {
                AudioManager.Instance.UnPauseMusic();
            }

            ShowVirtualControls();

            RaiseOnUIEvent(new LevelUIEventArgs(LevelUIEvent.ResumeButtonPressed));

            pauseGameCanvas.gameObject.SetActive(false);
        }

        private void ShowVirtualControls()
        {
            if (virtualControlsCanvas != null && areVirtualControlsNeeded)
            {
                virtualControlsCanvas.gameObject.SetActive(true);

                if(InputHandler.Instance != null)
                {
                    InputHandler.Instance.ReactivateMobileControls();
                }
            }

            if (virtualBombCanvas != null && isMobileBombButtonNeeded)
            {
                virtualBombCanvas.gameObject.SetActive(true);
            }
        }

        private void FiniteLevelComplete(System.EventArgs args)
        {
            isPauseAllowed = false;
            HideVirtualControls();
            RaiseOnUIEvent(new LevelUIEventArgs(LevelUIEvent.LevelCompleteAppear));

            if (levelCompleteCanvas != null)
            {
                levelCompleteCanvas.gameObject.SetActive(true);
                GameManager.SetSelectedUI(levelCompleteNextLevelButton.gameObject);
            }

            if (Level.Instance == null)
            {
                return;
            }

            levelCompleteCurrentScore.text = Level.Instance.CurrentScore.ToString();

            if (!GameManager.IsInitialized)
            {
                return;
            }

            levelCompleteHighScore.text = GameManager.Instance.CurrentHighScore().ToString();
        }

        public void SetActiveVirtualControls(
            bool areVirtualControlsNeeded, 
            bool isFireButtonNeeded, 
            bool isMobileBombButtonNeeded)
        {
            this.areVirtualControlsNeeded = areVirtualControlsNeeded;
            virtualControlsCanvas.gameObject.SetActive(areVirtualControlsNeeded);

            if (areVirtualControlsNeeded)
            {
                virtualFire1Button.gameObject.SetActive(isFireButtonNeeded);
            }
            else
            {
                virtualFire1Button.gameObject.SetActive(false);
            }

            this.isMobileBombButtonNeeded = isMobileBombButtonNeeded;
            virtualBombCanvas.gameObject.SetActive(isMobileBombButtonNeeded);
        }

        private void HideVirtualControls()
        {
            if (virtualControlsCanvas != null)
            {
                virtualControlsCanvas.gameObject.SetActive(false);
            }

            if(InputHandler.Instance != null)
            {
                InputHandler.Instance.ResetVirtualInput();
            }

            if (virtualBombCanvas != null)
            {
                virtualBombCanvas.gameObject.SetActive(false);
            }
        }

        private void RaiseOnUIEvent(LevelUIEventArgs uiEvent)
        {
            OnUIEvent?.Invoke(uiEvent);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}