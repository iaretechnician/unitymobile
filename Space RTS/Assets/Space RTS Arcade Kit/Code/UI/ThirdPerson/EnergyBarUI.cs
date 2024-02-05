using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the available energy level on the player's ship
/// </summary>
public class EnergyBarUI : MonoBehaviour {

    public Image EnergyBar;
    public Image ArmorBar;

    void Update () {
        if(Ship.PlayerShip != null) {
            ArmorBar.fillAmount =
               Ship.PlayerShip.Armor / Ship.PlayerShip.MaxArmor;
            EnergyBar.fillAmount = 
                Ship.PlayerShip.Equipment.energyAvailable / Ship.PlayerShip.Equipment.energyCapacity;
        }
    }
}
