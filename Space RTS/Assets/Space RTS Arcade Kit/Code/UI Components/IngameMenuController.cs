using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameMenuController : MonoBehaviour {

    private const int SHIP_MENU = 0;
    private const int TARGET_MENU = 1;
    private const int PLAYER_MENU = 2;
    private const int HELP_MENU = 3;

    public Text Credits, Experience;

    private int _selectedItem = -1;
    private List<ClickableImage> _menuItems;

    private CanvasController _canvasController;
    private GameObject[] _openedMenus;   // Tracks which menus are currently open

    private System.EventHandler _entryDelegate, _exitDelegate;

    void Awake () {
        _canvasController = GetComponentInParent<CanvasController>();
        _menuItems = new List<ClickableImage>();        

        foreach (Transform child in transform)
        {
            _menuItems.Add(child.gameObject.GetComponent<ClickableImage>());
        }

        _openedMenus = new GameObject[_menuItems.Count];

        _entryDelegate = new System.EventHandler(OnPointerEntry);
        _exitDelegate = new System.EventHandler(OnPointerExit);
    }
	
	void Update () {
        // Ignore input if a menu is open
        if (_canvasController.GetNumberOfOpenMenus() > 0)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_selectedItem <= 0)
            {
                _selectedItem = 0;
            }
            else
            {
                _menuItems[_selectedItem].SetColor(Color.white);
                _selectedItem--;
                GameController.Instance.PlaySound(AudioController.Instance.ScrollSound);
            }
            _menuItems[_selectedItem].SetColor(Color.red);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_selectedItem == -1) { 
                _selectedItem = 0;
            }
            else if(_selectedItem+1 < _menuItems.Count){
                _menuItems[_selectedItem].SetColor(Color.white);
                _selectedItem++;
                GameController.Instance.PlaySound(AudioController.Instance.ScrollSound);
            }
            _menuItems[_selectedItem].SetColor(Color.red);
        }
        if (Input.GetKeyDown(KeyCode.Return)){
            OnItemSelected();
        }
    }

    private void OnEnable()
    {
        transform.parent.GetComponent<Animator>().SetTrigger("OpenIngameMenu");

        _selectedItem = 0;
        _menuItems[0].SetColor(Color.red);
        for (int i=1; i<_menuItems.Count; i++)
            _menuItems[i].SetColor(Color.white);

        EventManager.PointerEntry += _entryDelegate;
        EventManager.PointerExit += _exitDelegate;

        Credits.text = "Credits: \n"+Player.Credits;
    }

    private void OnDisable()
    {
        EventManager.PointerEntry -= _entryDelegate;
        EventManager.PointerExit -= _exitDelegate;
    }

    private void OnPointerEntry(object sender, System.EventArgs e)
    {
        ClickableImage textComponent;
        if ((textComponent = ((GameObject)sender).GetComponent<ClickableImage>()) == null)
            return;
        if (_canvasController.GetNumberOfOpenMenus() > 0)
            return;

        // Set all (other) items to unselected (white)
        foreach (ClickableImage ci in _menuItems)
        {
            ci.SetColor(Color.white);
        }

        textComponent.SetColor(Color.red);
        _selectedItem = _menuItems.IndexOf(textComponent);
    }

    private void OnPointerExit(object sender, System.EventArgs e)
    {
        if (((GameObject)sender).GetComponent<ClickableImage>() == null)
            return;
        if (_canvasController.GetNumberOfOpenMenus() > 0)
            return;

        // Set all (other) items to unselected (white)
        foreach (ClickableImage ci in _menuItems)
        {
            ci.SetColor(Color.white);
        }

        _selectedItem = -1;
    }

    /// <summary>
    /// This method contains the functionality of the menu, and performs the 
    /// desired operation depending on which option was selected by the user.
    /// </summary>
    public void OnItemSelected()
    {
        if (_openedMenus[_selectedItem] != null)
            return;

        if (_selectedItem == SHIP_MENU)   // Current ship menu
        {
            _openedMenus[SHIP_MENU] = _canvasController.OpenMenu(UIElements.Instance.TargetMenu);
            TargetScrollMenu menu = _openedMenus[SHIP_MENU].GetComponent<TargetScrollMenu>();

            menu.PopulateMenuOptions(Ship.PlayerShip.gameObject);
        }
        if (_selectedItem == TARGET_MENU)  // Target Menu
        {
            GameObject target = HUDMarkers.Instance.GetCurrentSelectedTarget();
            if (target == null)
            {
                GameController.Instance.PlaySound(AudioController.Instance.ScrollSound);
                return;
            }

            _openedMenus[TARGET_MENU] = _canvasController.OpenMenu(UIElements.Instance.TargetMenu);
            TargetScrollMenu menu = _openedMenus[TARGET_MENU].GetComponent<TargetScrollMenu>();

            menu.PopulateMenuOptions(target);
        }
        if (_selectedItem == PLAYER_MENU)  // Reputation menu
        {
            _openedMenus[PLAYER_MENU] = _canvasController.OpenMenu(UIElements.Instance.ScrollText);
            ScrollTextController reputationMenu = _openedMenus[PLAYER_MENU].GetComponent<ScrollTextController>();

            reputationMenu.HeaderText.text = "Player Information";

            reputationMenu.AddMenuItem("Level: " + Player.Level, true, Color.white);
            reputationMenu.AddMenuItem("", false, Color.white);
            reputationMenu.AddMenuItem("Credits: "+Player.Credits, false, Color.white);
            reputationMenu.AddMenuItem("Ships owned: " + (Player.OwnedShips.Count), false, Color.white);
            reputationMenu.AddMenuItem("", false, Color.white);

            reputationMenu.AddMenuItem("Player reputation", true, Color.white);
            foreach (Faction otherFaction in ObjectFactory.Instance.Factions)
            {
                if (otherFaction == Player.Instance.PlayerFaction)
                    continue;

                reputationMenu.AddMenuItem(otherFaction.name + ": " + Player.Instance.PlayerFaction.RelationWith(otherFaction),
                    false, Player.Instance.PlayerFaction.GetRelationColor(otherFaction));
            }

            reputationMenu.AddMenuItem("",false, Color.white);
            reputationMenu.AddMenuItem("Fighter and capital ship kills by faction:", true, Color.white);
            reputationMenu.AddMenuItem(Player.Kills.x+", "+ Player.Kills.y, false, Color.white);

        }
        if (_selectedItem == HELP_MENU)
        {
            _openedMenus[HELP_MENU] = _canvasController.OpenMenu(UIElements.Instance.ScrollText);
            ScrollTextController menu = _openedMenus[HELP_MENU].GetComponent<ScrollTextController>();

            menu.HeaderText.text = "Game controls";
            menu.AddMenuItem("Space - switch between mouse and keyboard flight modes", false, Color.white);
            menu.AddMenuItem("<b>Mouse flight mode:</b>\nW,S - Accelerate,Decelerate\n" +
                "A,D - Strafe left and right", false, Color.white);
            menu.AddMenuItem("<b>Keyboard flight mode:</b>\nW,S - Pitch up and down\n" +
                "A,D - Yaw left and right", false, Color.white);
            menu.AddMenuItem("Q/E - roll left/right", false, Color.white);
            menu.AddMenuItem("Mousewheel - throttle control", false, Color.white);
            menu.AddMenuItem("Shift+A - toggle autopilot (depends on selected target)", false, Color.white);
            menu.AddMenuItem("Shift+C - request docking at Station", false, Color.white);
            menu.AddMenuItem("Shift+W - toggle Supercruise", false, Color.white);
            menu.AddMenuItem("H - toggle orbit camera", false, Color.white);
            menu.AddMenuItem("Z - toggle main engines", false, Color.white);
            menu.AddMenuItem("Esc - cancel current menu", false, Color.white);
            menu.AddMenuItem("Enter - ingame menu", false, Color.white);
            menu.AddMenuItem("T - target object under crosshairs", false, Color.white);
            menu.AddMenuItem("F2 - toggle HUD", false, Color.white);
        }
        GameController.Instance.PlaySound(AudioController.Instance.SelectSound);
    }

    public void OnItemSelected(int itemID)
    {
        _selectedItem = itemID;
        OnItemSelected();
    }
}
