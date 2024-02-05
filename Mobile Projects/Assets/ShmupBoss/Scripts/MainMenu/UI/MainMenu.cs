using UnityEngine;
using UnityEngine.UI;

namespace ShmupBoss
{
    /// <summary>
    /// Holds references for all the other sub menus such as levels, player select, settings, quit etc.. 
    /// This will control enabling/disabling the sub menus and going back to the main menu.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Main Menu/UI/Main Menu")]
    public class MainMenu : MonoBehaviour
    {
        public static event CoreDelegate OnUIEvent;

        /// <summary>
        /// This button will start the first level.
        /// </summary>
        [Tooltip("This button will start the first level")]
        [SerializeField]
        private Button startButton;

        /// <summary>
        /// The main menu canvas that this script controls. This canvas will be activated and deactivated 
        /// based on what buttons are pressed.
        /// </summary>
        [Tooltip("The main menu canvas that this script controls. This canvas will be activated and " +
            "deactivated based on what buttons are pressed.")]
        [SerializeField]
        private RectTransform mainMenuCanvas;

        /// <summary>
        /// All of the canvases that refer to sub menus which are part of this main menu.
        /// </summary>
        [Tooltip("All of the canvases that refer to sub menus which are part of this main menu.")]
        [SerializeField]
        private CanvasByButton[] canvases;

        /// <summary>
        /// This is just to test the accumlation of coins by the player, coins are currently not used to 
        /// activate anything but make it possible for users to expand this pack and include more features.
        /// </summary>
        [Tooltip("This is just to test the accumulation of coins by the player, coins are currently " +
            "not used to activate anything but make it possible for users to expand this pack and include " +
            "more features.")]
        [SerializeField]
        private Text coinsText;

        private void Awake()
        {
            Initializer.Instance.SubscribeToStage(AssignPreSelectedPlayer, 0);
            Initializer.Instance.SubscribeToStage(UpdateCoins, 1);
        }

        private void AssignPreSelectedPlayer()
        {
            int preSelectedPlayerIndex = GameManager.Instance.SelectedPlayerIndex;
            GameManager.Instance.AssignSelectedPlayer(preSelectedPlayerIndex);
        }

        private void UpdateCoins()
        {
            if(coinsText == null)
            {
                return;
            }
            
            int accumlatedCoins = GameManager.Instance.AccumlatedCoins;
            coinsText.text = "Coins: " + accumlatedCoins.ToString();
        }

        private void Start()
        {
            AssignStartButton();
            AssignEnableCanvasListeners();
            EnableMainMenuCanvas();
        }

        private void AssignStartButton()
        {
            if(startButton == null)
            {
                return;
            }
            
            startButton.onClick.AddListener(GameManager.Instance.ToFirstLevel);
        }

        private void AssignEnableCanvasListeners()
        {
            for (int i = 0; i < canvases.Length; i++)
            {
                if (canvases[i].ActivatingButton == null)
                {
                    Debug.Log("MainMenu.cs: You forgot to assign an activating button " +
                        "to canvas index number: " + i + " in the main menu.");

                    continue;
                }

                // "i" must be saved to another variable before adding the listner of enable canvas.
                int index = i;

                canvases[index].ActivatingButton.onClick.AddListener(delegate { EnableCanvas(index); });
            }
        }

        /// <summary>
        /// Disables the main menu canvas and enables the indexed canvas in the canvases array.
        /// </summary>
        /// <param name="index">The index of the canvas you wish to enable.</param>
        private void EnableCanvas(int index)
        {
            RaiseOnUIEvent(MainUIEvent.ButtonPressed);
            mainMenuCanvas.gameObject.SetActive(false);
            canvases[index].Canvas.gameObject.SetActive(true);

            if(canvases[index].FirstSelectedButton == null)
            {
                return;
            }

            GameManager.SetSelectedUI(canvases[index].FirstSelectedButton.gameObject);
        }

        /// <summary>
        /// Disables all other sub menu canvases and enables the main menu canvas.
        /// </summary>
        public void EnableMainMenuCanvas()
        {
            RaiseOnUIEvent(MainUIEvent.ButtonPressed);

            for (int i =0; i < canvases.Length; i++)
            {
                canvases[i].Canvas.gameObject.SetActive(false);
            }

            mainMenuCanvas.gameObject.SetActive(true);

            GameManager.SetSelectedUI(canvases[0].ActivatingButton.gameObject);
        }

        public void NotifyOfUIEvent(MainUIEvent UIEvent)
        {
            RaiseOnUIEvent(UIEvent);
        }

        private void RaiseOnUIEvent(MainUIEvent UIEvent)
        {
            OnUIEvent?.Invoke(new MainUIEventArgs(UIEvent));
        }
    }
}