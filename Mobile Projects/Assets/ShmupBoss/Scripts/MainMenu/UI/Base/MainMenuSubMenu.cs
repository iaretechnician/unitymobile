using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for the sub menus in the main menu, such as settings, player select menu, etc..
    /// </summary>
    [RequireComponent(typeof(MainMenu))]
    public abstract class MainMenuSubMenu : MonoBehaviour
    {
        protected MainMenu mainMenu;

        protected virtual void Awake()
        {
            mainMenu = GetComponent<MainMenu>();

            if(mainMenu == null)
            {
                Debug.Log("MainMenuSubMenu.cs: was unable to find the main menu, please only apply " +
                    "sub menu classes to an object which contains the main menu.");
            }
        }

        protected void OpenMainMenu()
        {
            mainMenu.NotifyOfUIEvent(MainUIEvent.ButtonPressed);
            mainMenu.EnableMainMenuCanvas();
        }
    }
}