using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// Holds buttons that will allow access to the game levels depending if they have been completed or not.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Main Menu/UI/Levels Menu")]
    public class LevelsMenu : MainMenuSubMenu
    {
        /// <summary>
        /// This scroll view will be propagated with the levels buttons and its size adapted to how many 
        /// levels in the game there are.
        /// </summary>
        [Tooltip("This scroll view will be propagated with the levels buttons and its size adapted to how " +
            "many levels in the game there are.")]
        [SerializeField]
        private RectTransform scrollViewContent;

        /// <summary>
        /// The button used here will be duplicated to create the rest of the levels buttons.
        /// </summary>
        [Tooltip("The button used here will be duplicated to create the rest of the levels buttons.")]
        [SerializeField]
        private RectTransform levelButtonTemplate;

        /// <summary>
        /// The button which will disable this levels canvas and enable the main menu canvas.
        /// </summary>
        [Tooltip("The button which will disable this levels canvas and enable the main menu canvas.")]
        [SerializeField]
        private Button backButton;

        /// <summary>
        /// All the levels will be listed in the scroll view, this space determines the distance between each button.
        /// </summary>
        [Tooltip("All the levels will be listed in the scroll view, this space determines the distance between " +
            "each button.")]
        [SerializeField]
        private float speceBetweenSelectLevelButtons;

        /// <summary>
        /// The height used by the level button adding to it some margin.
        /// </summary>
        [Tooltip("Use the height used by the level button adding to it some margin.")]
        [SerializeField]
        private float levelButtonHeight = 130.0f;

        [Tooltip("The margin for the scroll view which will show the levels buttons.")]
        [SerializeField]
        private float scrollviewMargin = 30.0f;

        private void Start()
        {
            SetupLevelSelectButtons();

            if (backButton != null)
            {
                backButton.onClick.AddListener(OpenMainMenu);
            }
        }

        /// <summary>
        /// Adapts the size of the scroll view and creates the levels buttons adding the high score if needed.
        /// </summary>
        private void SetupLevelSelectButtons()
        {
            int levelsNumber = GameManager.Instance.LevelsNumber;

            SavedLevel[] savedLevels = GameManager.Instance.CurrentSaveInfo.SavedLevels;

            AdaptTheSizeOfTheScrollviewToTheNumberOfLevels(scrollViewContent, levelsNumber);

            for (int i = 0; i < levelsNumber; i++)
            {
                if (savedLevels[i].Status != LevelStatus.NotAvailable)
                {
                    CreateLevelsButton(savedLevels[i].HighScore, i + 1, true);
                }
                else
                {
                    CreateLevelsButton(savedLevels[i].HighScore, i + 1, false);
                }
            }

            levelButtonTemplate.gameObject.SetActive(false);
        }

        private void AdaptTheSizeOfTheScrollviewToTheNumberOfLevels(RectTransform contentRect, int levelsNumber)
        {
            float rectWidth = contentRect.sizeDelta.x;
            float rectHeight = (levelButtonHeight + scrollviewMargin) + (levelButtonHeight * (levelsNumber - 1));

            contentRect.sizeDelta = new Vector2(rectWidth, rectHeight);
        }

        private LevelButton CreateLevelsButton(int levelScore, int levelIndex, bool isInteractable)
        {
            RectTransform levelButton = Instantiate(levelButtonTemplate, levelButtonTemplate.transform.parent);

            Vector2 offsetMax = levelButton.offsetMax;
            Vector2 offsetMin = levelButton.offsetMin;

            float displacementOnY = (levelIndex - 1) * (speceBetweenSelectLevelButtons + levelButton.rect.size.y);

            offsetMin.y -= displacementOnY;
            offsetMax.y -= displacementOnY;

            levelButton.offsetMax = offsetMax;
            levelButton.offsetMin = offsetMin;

            levelButton.name = "Level " + levelIndex.ToString();

            LevelButton buttonScript = levelButton.GetComponent<LevelButton>();

            if (buttonScript != null)
            {
                buttonScript.LevelIndex = levelIndex - 1;
            }

            Button button = levelButton.GetComponent<Button>();
            button.interactable = isInteractable;

            Text buttonText = levelButton.GetComponentInChildren<Text>();

            if (buttonText != null)
            {
                buttonText.text = "Level " + (levelIndex).ToString() + " " + "Score :" + levelScore.ToString();
            }

            levelButton.SetParent(scrollViewContent.transform);

            return buttonScript;
        }
    }
}