using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class SummaryScreen : MonoBehaviour
{
    public ScrollTextController DestroyedShips, LostShips, BattleSummary;
    public Button ConfirmButton;

    private AIManager _aiManager;

    public void Initialize(AIManager aiManager)
    {
        Camera.main.GetComponent<TacticalCameraController>().CanMove = false;
        ShowDestroyedShips(aiManager);
        ShowLostShips(aiManager);
        ShowBattleSummary(aiManager);
    }

    private void ShowLostShips(AIManager aiManager)
    {
        var lostShips = aiManager.GetLostShips();
        foreach(var ship in lostShips)
        {
            LostShips.AddMenuItem(ship.Key + " (" + ship.Value + ")", Color.red, IconManager.Instance.GetShipIcon(ship.Key));
        }
    }

    private void ShowDestroyedShips(AIManager aiManager)
    {
        var destroyedShips = aiManager.GetDestroyedShips();
        foreach (var ship in destroyedShips)
        {
            DestroyedShips.AddMenuItem(ship.Key + " (" + ship.Value + ")", Color.green, IconManager.Instance.GetShipIcon(ship.Key));
        }
    }

    private void ShowBattleSummary(AIManager aiManager)
    {
        var lostShips = aiManager.GetLostShips();
        var destroyedShips = aiManager.GetDestroyedShips();
        if(Player.Instance.SpawnedShips.Count == 0)
        {
            BattleSummary.AddMenuItem("All player ships destroyed! You have failed to take the sector!", true, Color.red);
        }
        else
        {
            BattleSummary.AddMenuItem("All hostile ships destroyed! You have successfully taken the sector!", true, Color.green);
            Player.SectorsTaken.Add(SectorNavigation.CurrentSector);
            Player.Level++;
        }

        BattleSummary.AddMenuItem("Lost ships ", lostShips.Values.Sum()+"", true, Color.white);
        BattleSummary.AddMenuItem("Destroyed ships ", destroyedShips.Values.Sum()+"", true, Color.white);
        BattleSummary.AddMenuItem("Sector difficulty ", UniverseMap.Sectors[SectorNavigation.CurrentSector].Difficulty+"", true, Color.white);

        int profit = 0;
        foreach(var ship in destroyedShips)
        {
            ModelInfo shipModelInfo = ObjectFactory.Instance.GetShipByName(ship.Key).GetComponent<Ship>().ShipModelInfo;
            profit += shipModelInfo.Cost;
        }
        profit = (int)(profit/2.0f);

        BattleSummary.AddMenuItem("Credits earned ", profit + "", true, Color.white);

        Player.Credits += profit;
    }

    public void SaveAndQuit()
    {
        SaveGame.Save();
        SceneManager.LoadScene("Universe");
    }
}
