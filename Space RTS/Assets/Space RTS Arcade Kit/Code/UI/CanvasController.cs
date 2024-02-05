using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : Singleton<CanvasController> {

    public GameObject IngameMenu;
    public SummaryScreen GameSummary;

    private Stack<GameObject> openMenus;

    private void Awake()
    {
        openMenus = new Stack<GameObject>();

        EventManager.CloseClicked += ((sender, e) =>
        {
            CloseMenu();
        });
    }

    // Either the ingame menu or popup menus can be open
    void Update () {
        
        if (Input.GetKeyDown(KeyCode.Return) && openMenus.Count == 0)
        {
            IngameMenu.SetActive(true);
            Ship.PlayerShip.UsingMouseInput = false;
            GameController.Instance.IsShipInputDisabled = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(openMenus.Count == 0) { 
                IngameMenu.SetActive(false);
                GameController.Instance.IsShipInputDisabled = false;
            }
        }

        if(Input.GetMouseButtonDown(0)) { 
            if(Input.mousePosition.x > 0.25*Screen.width && Input.mousePosition.x < 0.75 * Screen.width && Input.mousePosition.y > Screen.height - 10)
                if (openMenus.Count == 0 && !Ship.PlayerShip.PlayerInput.useMouseInput)
                {
                    IngameMenu.SetActive(true);
                    GameController.Instance.IsShipInputDisabled = true;
                }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            var exitGameMenu = OpenMenu(UIElements.Instance.ConfirmDialog)
                .GetComponent<PopupConfirmMenuController>();

            exitGameMenu.HeaderText.text = "Exit to Main Menu?";
            exitGameMenu.AcceptButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Universe");
            });
            exitGameMenu.CancelButton.onClick.AddListener(() =>
            {
                CloseMenu();
            });
        }
    }

    /// <summary>
    /// Opens a requested menu and returns the reference to the UI gameobject.
    /// </summary>
    /// <param name="menu">Menu prefab from the UIElements object</param>
    /// <returns>UI element gameobject</returns>
    public GameObject OpenMenu(GameObject menu)
    {
        GameObject menuInstance = GameObject.Instantiate(menu, this.transform);
        // Set currently open menu to inactive
        if (openMenus.Count > 0)
            openMenus.Peek().SetActive(false);
        // Add new open menu
        openMenus.Push(menuInstance);

        return menuInstance;
    }

    /// <summary>
    /// Opens a requested menu at a given 2D position (usually mousePosition) 
    /// and returns the reference to the UI gameobject.
    /// </summary>
    /// <param name="menu">Menu prefab from the UIElements object</param>
    /// <param name="position">2D screen position of UI element's upper-right corner</param>
    /// <returns>UI element gameobject</returns>
    public GameObject OpenMenuAtPosition(GameObject menu, Vector2 position, bool hideMenuBelow = true)
    {
        GameObject menuInstance = null;

        if (!hideMenuBelow)
        {
            menuInstance = GameObject.Instantiate(menu, this.transform);

            // Add new open menu
            openMenus.Push(menuInstance);
        }
        else
            menuInstance = OpenMenu(menu);

        menuInstance.GetComponent<RectTransform>().anchoredPosition = position;

        return menuInstance;
    }

   

    /// <summary>
    /// Closes the currently visible (open) menu (and opens a menu one layer below, if such
    /// exists)
    /// </summary>
    public void CloseMenu()
    {
        if(openMenus.Count > 0) {

            GameObject menu = openMenus.Pop();
            GameObject.Destroy(menu);
        }
        if (openMenus.Count > 0)
        {
            openMenus.Peek().SetActive(true);
        }
    }

    public int GetNumberOfOpenMenus()
    {
        return openMenus.Count;
    }

    /// <summary>
    /// Closes all opened menus, including the special menus (ingame menu and map)
    /// </summary>
    public void CloseAllMenus()
    {
        while (GetNumberOfOpenMenus() > 0)
            CloseMenu();
    }

    /// <summary>
    /// Opens the summary screen shown when the game is over, either by loss of all
    /// player owned ships or by sector takeover.
    /// </summary>
    /// <param name="aiManager"></param>
    public void OpenSummaryScreen(AIManager aiManager)
    {
        GameSummary.gameObject.SetActive(true);
        GameSummary.GetComponent<SummaryScreen>().Initialize(aiManager);
        var viewController = GetComponent<CanvasViewController>();
        viewController.TacticalCanvas.gameObject.SetActive(false);
        viewController.FlightCanvas.gameObject.SetActive(false);
        GameController.Instance.IsShipInputDisabled = true;
    }
}
