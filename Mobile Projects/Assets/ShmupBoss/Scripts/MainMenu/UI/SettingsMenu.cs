using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// Sub menu in the main menu which contains controls for changing game settings such as volume, 
    /// input method and difficulty.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Main Menu/UI/Settings Menu")]
    public class SettingsMenu : MainMenuSubMenu
    {
        [SerializeField]
        private Slider musicSlider;

        [SerializeField]
        private Slider soundFXSlider;

        [SerializeField]
        private Button inputMethodNextButton;

        [SerializeField]
        private Button inputMethodPreviousButton;

        [Tooltip("The text showing if controls or pointer is used which are selected by the " +
            "next and previous buttons.")]
        [SerializeField]
        private Text inputMethodText;

        [SerializeField]
        private Button difficultyNextButton;

        [SerializeField]
        private Button difficultyPreviousButton;

        [Tooltip("The text showing the difficulty level which is selected by the " +
            "next and previous buttons.")]
        [SerializeField]
        private Text difficultyText;

        [Tooltip("This toggle will enable/disable auto fire mode which makes the player shooting continuously " +
            "or activated by the fire button (or touch if pointers input method selected).")]
        [SerializeField]
        private Toggle autoFireToggle;

        [Tooltip("Will take you back to the main menu after saving the settings.")]
        [SerializeField]
        private Button backButton;
        public Button BackButton
        {
            get;
        }

        private int currentInputMethodIndex;

        private readonly InputMethod[] availableInputMethodsArray = new InputMethod[]
        {
            InputMethod.Controls,
            InputMethod.Pointer
        };

        private InputMethod selectedInputMethod
        {
            get
            {
                return availableInputMethodsArray[currentInputMethodIndex];
            }
            set
            {
                for (int i = 0; i < availableInputMethodsArray.Length; i++)
                {
                    if (value == availableInputMethodsArray[i])
                    {
                        currentInputMethodIndex = i;

                        if (inputMethodText != null)
                        {
                            inputMethodText.text = selectedInputMethod.ToString();
                        }
                    }
                }
            }
        }

        private int currentDifficultyLevelIndex;

        /// <summary>
        /// This array length must correspond to the Multiplier data array length in the Multiplier script.<br></br>
        /// If you would like to increase the difficulties you must adjust the enum, this array and the 
        /// multiplier data.
        /// </summary>
        private readonly DifficultyName[] availableDifficultyNamesArray = new DifficultyName[]
        {
            DifficultyName.Easy,
            DifficultyName.Normal,
            DifficultyName.Hard,
        };

        private DifficultyName selectedDifficultyLevel
        {
            get
            {                
                return availableDifficultyNamesArray[currentDifficultyLevelIndex];
            }
            set
            {
                for (int i = 0; i < availableDifficultyNamesArray.Length; i++)
                {
                    if (value == availableDifficultyNamesArray[i])
                    {
                        currentDifficultyLevelIndex = i;

                        if (difficultyText != null)
                        {
                            difficultyText.text = selectedDifficultyLevel.ToString();
                        }
                    }
                }
            }
        }

        void Start()
        {
            LoadSettings();
            SetupControls();
        }

        private void SetupControls()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OpenMainMenu);
                backButton.onClick.AddListener(SaveSettings);
            }

            if (musicSlider != null)
            {
                musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
            }

            if (soundFXSlider != null)
            {
                soundFXSlider.onValueChanged.AddListener(UpdateSFXVolume);
            }

            if (inputMethodNextButton != null)
            {
                inputMethodNextButton.onClick.AddListener(NextInputMethod);
            }

            if (inputMethodPreviousButton != null)
            {
                inputMethodPreviousButton.onClick.AddListener(PreviousInputMethod);
            }

            if (difficultyNextButton != null)
            {
                difficultyNextButton.onClick.AddListener(NextDifficultyLevel);
            }

            if (difficultyPreviousButton != null)
            {
                difficultyPreviousButton.onClick.AddListener(PreviousDifficultyLevel);
            }

            if (autoFireToggle != null)
            {
                autoFireToggle.onValueChanged.AddListener(AutoFireChange);
            }
        }

        public void NextInputMethod()
        {
            mainMenu.NotifyOfUIEvent(MainUIEvent.ButtonPressed);
            selectedInputMethod++;
            GameManager.Instance.CurrentGameSettings.CurrentInputMehtod = selectedInputMethod;
        }

        public void PreviousInputMethod()
        {
            mainMenu.NotifyOfUIEvent(MainUIEvent.ButtonPressed);

            selectedInputMethod--;
            GameManager.Instance.CurrentGameSettings.CurrentInputMehtod = selectedInputMethod;
        }

        public void NextDifficultyLevel()
        {
            mainMenu.NotifyOfUIEvent(MainUIEvent.ButtonPressed);

            selectedDifficultyLevel++;
            GameManager.Instance.CurrentGameSettings.DifficultyLevel = currentDifficultyLevelIndex;
        }

        public void PreviousDifficultyLevel()
        {
            mainMenu.NotifyOfUIEvent(MainUIEvent.ButtonPressed);

            selectedDifficultyLevel--;
            GameManager.Instance.CurrentGameSettings.DifficultyLevel = currentDifficultyLevelIndex;
        }

        private void UpdateMasterVolume(float value)
        {
            AudioManager.Instance.MasterVolume = value;
        }

        public void UpdateMusicVolume(float value)
        {
            AudioManager.Instance.MusicVolume = value;
        }

        public void UpdateSFXVolume(float value)
        {
            AudioManager.Instance.SfxVolume = value;
        }

        public void AutoFireChange(bool value)
        {
            mainMenu.NotifyOfUIEvent(MainUIEvent.ToggleChange);

            GameManager.Instance.CurrentGameSettings.IsAutoFiring = value;
        }

        public void LoadSettings()
        {
            GameSettings settings = GameManager.Instance.CurrentGameSettings;

            if (musicSlider != null)
            {
                musicSlider.value = settings.MusicVolume;
            }

            if (soundFXSlider != null)
            {
                soundFXSlider.value = settings.SfxVolume;
            }

            if (autoFireToggle != null)
            {
                autoFireToggle.isOn = settings.IsAutoFiring;
            }

            selectedInputMethod = settings.CurrentInputMehtod;
            currentDifficultyLevelIndex = settings.DifficultyLevel;
            selectedDifficultyLevel = availableDifficultyNamesArray[currentDifficultyLevelIndex];
        }

        public void SaveSettings()
        {
            GameSettings settings = GameManager.Instance.CurrentGameSettings;

            if (musicSlider != null)
            {
                settings.MusicVolume = musicSlider.value;
            }

            if (soundFXSlider != null)
            {
                settings.SfxVolume = soundFXSlider.value;
            }

            if (autoFireToggle != null)
            {
                settings.IsAutoFiring = autoFireToggle.isOn;
            }

            settings.CurrentInputMehtod = selectedInputMethod;
            settings.DifficultyLevel = currentDifficultyLevelIndex;

            GameManager.Instance.SaveGameSettings();
        }
    }
}