using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Armor")]
public class AppliqueArmor : Equipment
{
    public float ArmorMultiplier;
    public int ItemCost;

    public override int Cost
    {
        get
        {
            return ItemCost;
        }
    }

    public override void InitItem(Ship ship)
    {
        ship.MaxArmor *= (int)ArmorMultiplier;
        ship.Armor *= ArmorMultiplier;
    }

    public override void RemoveItem(Ship ship)
    {
        ship.MaxArmor /= (int)ArmorMultiplier;
        if(ship.Armor > ship.MaxArmor)
            ship.Armor = ship.MaxArmor;
    }

    public override void UpdateItem(Ship ship)
    {
    }

}
