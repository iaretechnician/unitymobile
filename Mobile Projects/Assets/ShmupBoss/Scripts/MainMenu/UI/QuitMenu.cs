using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// A sub menu for quitting the game or going back to the main menu.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Main Menu/UI/Quit Menu")]
    public class QuitMenu : MainMenuSubMenu
    {
        [Tooltip("The button which when pressed will take you out of the game.")]
        [SerializeField]
        private Button yesButton;

        [Tooltip("The button which when pressed will take you back to the main menu.")]
        [SerializeField]
        private Button noButton;

        private void Start()
        {
            if (yesButton != null)
            {
                yesButton.onClick.AddListener(AcceptQuit);
            }

            if (noButton != null)
            {
                noButton.onClick.AddListener(OpenMainMenu);
            }
        }

        public void AcceptQuit()
        {
            mainMenu.NotifyOfUIEvent(MainUIEvent.ButtonPressed);
            Application.Quit();
        }
    }
}