using UnityEngine;
using UnityEngine.UI;

public class FleetAssemblyScreen : MonoBehaviour
{
    public ScrollMenuController ShipyardMenu, PlayerMenu;
    public ScrollTextController DetailsView;
    public Text CreditsText;
    public Text InfoText;
    public Button ConfirmButton;

    private void Start()
    {
        ShipyardMenu.HeaderText.text = "Fleet Assembly";
        PopulateMenu();

        // Disable station menu and focus ship menu
        ShipyardMenu.DisableKeyInput = true;
        PlayerMenu.DisableKeyInput = false;
        PlayerMenu.selectedOption = 0;
    }

    private void UpdateCredits()
    {
        // Keep credits display updated
        CreditsText.text = "Credits: " + Player.Credits;
    }

    private void Update()
    {
        if (ShipyardMenu.SubMenu != null || PlayerMenu.SubMenu != null)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ShipyardMenu.DisableKeyInput = true;
            PlayerMenu.DisableKeyInput = false;
            PlayerMenu.selectedOption = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShipyardMenu.DisableKeyInput = false;
            ShipyardMenu.selectedOption = 0;
            PlayerMenu.DisableKeyInput = true;
        }
    }

    /// <summary>
    /// Invoked when a trade menu was opened for a ship docked on a station.
    /// </summary>
    /// <param name="ship">Ship trading with station</param>
    /// <param name="station">Station to trade with</param>
    public void PopulateMenu()
    {
        PlayerMenuSetOptions();
        StationMenuSetOptions();
        UpdateCredits();
    }

    /// <summary>
    /// Resets and updates ship menu options
    /// </summary>
    private void PlayerMenuSetOptions()
    {
        PlayerMenu.ClearMenuOptions();
        foreach(var playerShip in Player.OwnedShips)
        {
            PlayerMenu.AddMenuOption(playerShip.ModelName, Color.white,
                IconManager.Instance.GetShipIcon(playerShip.ModelName), 1f, 80)
                .AddListener(() => OnOwnedShipClicked(playerShip));
        }
    }

    /// <summary>
    /// Resets and updates ship menu options
    /// </summary>
    private void StationMenuSetOptions()
    {
        ShipyardMenu.ClearMenuOptions();
        var shipsForSale = ObjectFactory.Instance.Ships;
        foreach (var shipForSale in shipsForSale)
        {
            string modelName = shipForSale.GetComponent<Ship>().ShipModelInfo.ModelName;
            ShipyardMenu.AddMenuOption(modelName, Color.white, IconManager.Instance.GetShipIcon(modelName), 1f, 80)
                .AddListener(() => OnDealerShipClicked(shipForSale));
        }
    }

    /// <summary>
    /// Adds a ship mounted equipment item to the left-side menu and handles its onClick.
    /// </summary>
    /// <param name="cargo">Cargo item to sell</param>
    public void OnOwnedShipClicked(Player.ShipDescriptor ownedShip)
    {
        // Check for open submenus
        if (ShipyardMenu.SubMenu != null)
            GameObject.Destroy(ShipyardMenu.SubMenu);
        if (PlayerMenu.SubMenu != null)
            GameObject.Destroy(PlayerMenu.SubMenu);

        ModelInfo shipModelInfo = ObjectFactory.Instance.GetShipByName(ownedShip.ModelName).GetComponent<Ship>().ShipModelInfo;
        PopulateDetailsView(shipModelInfo);

        if (Player.OwnedShips.Count < 2) {
            InfoText.text = "Cannot sell your last ship!";
            return;
        }

        ConfirmButton.GetComponentInChildren<Text>().text = "Sell";
        ConfirmButton.onClick.RemoveAllListeners();
        ConfirmButton.gameObject.SetActive(true);
        ConfirmButton.onClick.AddListener(() => {
            int salePrice = (int)shipModelInfo.Cost / 2;

            // Open Confirm Dialog
            GameObject SubMenu = GameObject.Instantiate(UIElements.Instance.ConfirmDialog, transform.parent);
            // Reposition submenu
            RectTransform rt = SubMenu.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, 0);

            PopupConfirmMenuController confirmSaleMenu = SubMenu.GetComponent<PopupConfirmMenuController>();
            confirmSaleMenu.HeaderText.text = "Confirm selling " + ownedShip.ModelName + " for " + salePrice + " cr.";
            confirmSaleMenu.AcceptButton.onClick.AddListener(() => {
                Player.OwnedShips.Remove(ownedShip);
                Player.Credits += salePrice;
                UpdateCredits();
                ConfirmButton.gameObject.SetActive(false);
                ConfirmButton.onClick.RemoveAllListeners();

                GameObject.Destroy(confirmSaleMenu.gameObject);
                PopulateMenu();
            });
            confirmSaleMenu.CancelButton.onClick.AddListener(() => {
                ConfirmButton.gameObject.SetActive(false);
                ConfirmButton.onClick.RemoveAllListeners();

                GameObject.Destroy(confirmSaleMenu.gameObject);
            });
        });
    }

    private void OnDealerShipClicked(GameObject boughtShip)
    {
        // Check for open submenus
        if (ShipyardMenu.SubMenu != null)
            GameObject.Destroy(ShipyardMenu.SubMenu);
        if (PlayerMenu.SubMenu != null)
            GameObject.Destroy(PlayerMenu.SubMenu);

        Ship shipForSale = boughtShip.GetComponent<Ship>();
        PopulateDetailsView(shipForSale.ShipModelInfo);

        ConfirmButton.GetComponentInChildren<Text>().text = "Buy";
        ConfirmButton.gameObject.SetActive(true);
        ConfirmButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.AddListener(() =>
        {
            int cost = shipForSale.ShipModelInfo.Cost;

            if (cost > Player.Credits) {
                InfoText.text = "Not enough credits to purchase this ship.";
                return;
            }

            // Open Confirm Dialog
            GameObject SubMenu = GameObject.Instantiate(UIElements.Instance.ConfirmDialog, transform.parent);
            // Reposition submenu
            RectTransform rt = SubMenu.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, 0);

            var buyDialog = SubMenu.GetComponent<PopupConfirmMenuController>();
            buyDialog.HeaderText.text = "Buy " + shipForSale.ShipModelInfo.ModelName + " for " + cost + " credits?";

            // What happens when Ok or Cancel is pressed
            buyDialog.AcceptButton.onClick.AddListener(() =>
            {
                Player.OwnedShips.Add(new Player.ShipDescriptor(shipForSale.ShipModelInfo.ModelName));
                Player.Credits -= cost;
                ConsoleOutput.PostMessage("Ship " + shipForSale.ShipModelInfo.ModelName + " purchased!", Color.green);

                ConfirmButton.gameObject.SetActive(false);
                ConfirmButton.onClick.RemoveAllListeners();

                GameObject.Destroy(buyDialog.gameObject);
                PopulateMenu();
            });
            buyDialog.CancelButton.onClick.AddListener(() =>
            {
                ConfirmButton.gameObject.SetActive(false);
                ConfirmButton.onClick.RemoveAllListeners();

                GameObject.Destroy(buyDialog.gameObject);
            });
        });
    }

    private void PopulateDetailsView(ModelInfo shipModelInfo)
    {
        DetailsView.ClearItems();

        DetailsView.AddMenuItem("Ship", shipModelInfo.ModelName, true, Color.white);
        DetailsView.AddMenuItem("Requires external docking", shipModelInfo.ExternalDocking + "", true, Color.white);
        DetailsView.AddMenuItem("Cost", shipModelInfo.Cost + "\n", true, Color.white);

        DetailsView.AddMenuItem("Class", shipModelInfo.Class + "", false, Color.white);
        DetailsView.AddMenuItem("Armor", shipModelInfo.MaxArmor + "", false, Color.white);
        DetailsView.AddMenuItem("Equipment slots", shipModelInfo.EquipmentSlots + "", false, Color.white);
        DetailsView.AddMenuItem("Cargo capacity", shipModelInfo.CargoSize + "", false, Color.white);
        DetailsView.AddMenuItem("Generator power", shipModelInfo.GeneratorPower + "", false, Color.white);
        DetailsView.AddMenuItem("Generator regeneration", shipModelInfo.GeneratorRegen + "", false, Color.white);
    }

    private Color GetComparedColor(int newValue, int currentValue)
    {
        if (newValue > currentValue)
            return Color.green;
        if (newValue < currentValue)
            return Color.red;
        return Color.white;
    }

}
