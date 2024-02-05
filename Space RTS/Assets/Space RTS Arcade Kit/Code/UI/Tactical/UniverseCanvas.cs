using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UniverseCanvas : MonoBehaviour
{
    public GameObject FleetWindow;
    public GameObject TextItemPrefab;
    public GameObject PlayerInfo;
    private Dictionary<Transform, Text> _items;

    private void Awake()
    {
        _items = new Dictionary<Transform, Text>();
    }

    /// <summary>
    /// Adds a label to a sector icon which enables the user to click on the sector to start a game and
    /// shows an info panel about the currently selected sector.
    /// </summary>
    /// <param name="sectorName">Text which will be shown in the text label</param>
    /// <param name="worldPosition">Position which is converted into screen position</param>
    public void AddSectorDescription(string sectorName, Transform worldPosition)
    {
        Text textItem = Instantiate(TextItemPrefab, transform).GetComponent<Text>();
        textItem.text = sectorName;
        _items.Add(worldPosition, textItem);

        textItem.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSectorIconClicked(sectorName);
        });
        textItem.GetComponent<SectorIconHoverHandler>().SetSectorData(UniverseMap.Sectors[sectorName]);
    }

    private void Update()
    {
        foreach(var sectorTextItem in _items)
        {
            sectorTextItem.Value.rectTransform.localPosition = GetScreenPosOfObject(sectorTextItem.Key);
        }
    }

    private Vector3 GetScreenPosOfObject(Transform target)
    {
        float x = Camera.main.WorldToScreenPoint(target.position).x - (Screen.width / 2f);
        float y = Camera.main.WorldToScreenPoint(target.position).y - (Screen.height / 2f);

        return new Vector2(x, y);
    }

    private void OnSectorIconClicked(string sectorName)
    {
        if(Player.OwnedShips.Count > 0) { 
            Camera.main.GetComponent<CameraAnimator>().SwitchToSector(sectorName);
            SaveGame.Save();
        }
        else
        {
            TextFlash.ShowInfoText("Unable to start game - player has no ships!");
            Debug.LogWarning("Unable to start game - player has no ships!");
        }
    }

    /// <summary>
    /// Invoked when opening the Fleet Assembly window. Deactivates the interactive universe map labels.
    /// </summary>
    public void OnFleetAssemblyOpened()
    {
        FleetWindow.SetActive(true);
        foreach(var textItem in _items.Values)
        {
            textItem.enabled = false;
        }
        FleetWindow.GetComponent<FleetAssemblyScreen>().PopulateMenu();
    }

    /// <summary>
    /// Invoked by close buttons on UI panels on the Universe scene. Closes menu panels and reactivates the 
    /// interactive universe map.
    /// </summary>
    public void OnMenuClosed()
    {
        FleetWindow.SetActive(false);
        PlayerInfo.SetActive(false);
        foreach (var textItem in _items.Values)
        {
            textItem.enabled = true;
        }
    }

    /// <summary>
    /// Invoked by a UI button on the Universe scene. Opens the player info panel with basic player profile data.
    /// </summary>
    public void OnPlayerInformationOpened()
    {
        PlayerInfo.SetActive(true);
        foreach (var textItem in _items.Values)
        {
            textItem.enabled = false;
        }

        var infoPanel = PlayerInfo.GetComponent<ScrollTextController>();
        infoPanel.ClearItems();

        infoPanel.AddMenuItem("Profile Name: " + Player.Name, true, Color.white);
        infoPanel.AddMenuItem("Level: " + Player.Level, false, Color.white);
        infoPanel.AddMenuItem("", false, Color.white);

        infoPanel.AddMenuItem("Owned ships: (" + Player.OwnedShips.Count+")", true, Color.white);
        foreach(var ship in Player.OwnedShips)
        {
            infoPanel.AddMenuItem(ship.ModelName, false, Color.white);
        }
        infoPanel.AddMenuItem("", false, Color.white);

        infoPanel.AddMenuItem("Sectors taken: (" + Player.SectorsTaken.Count + ")", true, Color.white);
        foreach (var sectorName in Player.SectorsTaken)
        {
            infoPanel.AddMenuItem(sectorName, false, Color.white);
        }
    }

    public void OnMainMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
