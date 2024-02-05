using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// Sub menu in the main menu to enable selecting the player.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Main Menu/UI/Player Menu")]
    public class PlayerMenu : MainMenuSubMenu
    {
        /// <summary>
        /// Will assign the currently active player as the selected player and take you back to the main menu.
        /// </summary>
        [Tooltip("Will assign the currently active player as the selected player " +
            "and take you back to the main menu.")]
        [SerializeField]
        private Button acceptButton;

        /// <summary>
        /// Will take you back to the main menu without assigning any selected player.
        /// </summary>
        [Tooltip("Will take you back to the main menu without assigning any selected player.")]
        [SerializeField]
        private Button backButton;

        /// <summary>
        /// Displays the next player selection.
        /// </summary>
        [Tooltip("Displays the next player selection.")]
        [SerializeField]
        private Button nextButton;

        /// <summary>
        /// Displays the previous player selection.
        /// </summary>
        [Tooltip("Displays the previous player selection.")]
        [SerializeField]
        private Button previousButton;

        /// <summary>
        /// The canvases which will be displayed when the user scrolls through the player selections, 
        /// the selected canvas array index will determine the selected player.<br></br>
        /// The size of this array must be exactly the same as the player selection in the game manager.
        /// </summary>
        [Tooltip("The canvases which will be displayed when the user scrolls through the player " +
            "selections, the selected canvas array index will determine the selected player." + "\n\n" + 
            "The size of this array must be exactly the same as the player selection in the game manager.")]
        [SerializeField]
        private RectTransform[] avatarCanvases;

        /// <summary>
        /// The avatar (canvas for player) which is currently displayed.
        /// </summary>
        private int avatarIndex;

        protected override void Awake()
        {
            base.Awake();
            
            Initializer.Instance.SubscribeToStage(GetAvatarIndex, 1);
        }

        private void GetAvatarIndex()
        {
            avatarIndex = GameManager.Instance.SelectedPlayerIndex;

            SetupButtons();
            DisableAllAvatarCanvases();
            EnableAvatarCanvas(avatarIndex);
        }

        private void SetupButtons()
        {
            if (acceptButton != null)
            {
                acceptButton.onClick.AddListener(Accept);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(OpenMainMenu);
            }

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(NextAvatar);
            }

            if (previousButton != null)
            {
                previousButton.onClick.AddListener(PreviousAvatar);
            }
        }

        private void DisableAllAvatarCanvases()
        {
            foreach (RectTransform avatarCanvas in avatarCanvases)
            {
                avatarCanvas.gameObject.SetActive(false);
            }
        }

        private void EnableAvatarCanvas(int index)
        {
            avatarCanvases[index].gameObject.SetActive(true);
        }


        private void Accept()
        {
            mainMenu.NotifyOfUIEvent(MainUIEvent.ButtonPressed);

            GameManager.Instance.AssignSelectedPlayer(avatarIndex);

            mainMenu.EnableMainMenuCanvas();
        }

        private void NextAvatar()
        {
            if (avatarIndex + 1 > avatarCanvases.Length - 1)
            {
                return;
            }

            avatarIndex++;

            DisableAllAvatarCanvases();
            EnableAvatarCanvas(avatarIndex);

            mainMenu.NotifyOfUIEvent(MainUIEvent.ButtonPressed);
        }

        private void PreviousAvatar()
        {
            if (avatarIndex - 1 < 0)
            {
                return;
            }

            avatarIndex--;

            DisableAllAvatarCanvases();
            EnableAvatarCanvas(avatarIndex);

            mainMenu.NotifyOfUIEvent(MainUIEvent.ButtonPressed);
        }
    }
}