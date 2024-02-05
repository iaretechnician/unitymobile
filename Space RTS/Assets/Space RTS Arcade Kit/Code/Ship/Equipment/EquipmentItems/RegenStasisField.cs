using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/RegenerativeStasisField")]
public class RegenStasisField : ActivatableEquipment
{
    public int ItemCost;
    public float Duration;
    public float RegenPercentage;

    private bool _isActive = false;
    private float _timer;
    private float _regenAmount, _initialArmor;

    public override int Cost
    {
        get
        {
            return ItemCost;
        }
    }


    public override void InitItem(Ship ship)
    {
        base.InitItem(ship);
        isSingleUse = true;
        _isActive = false;       
    }

    public override void RemoveItem(Ship ship)
    {
        ship.Equipment.UnmountEquipmentItem(this);
    }

    public override void UpdateItem(Ship ship)
    {
        if (_isActive)
        {
            _timer += Time.deltaTime;
            ship.Armor = _initialArmor + _timer / Duration * _regenAmount;

            if (_timer > Duration)
            {
                // When finished with operation
                ship.Armor = Mathf.Clamp(_initialArmor + _regenAmount, 0, ship.MaxArmor);
                RemoveItem(ship);
            }
        }
    }

    public override bool SetActive(bool isActive, Ship ship)
    {
        // Dont use item if there is nothing to heal
        if (ship.Armor == ship.MaxArmor)
            return false;

        isActive = true;
        _timer = 0;
        _regenAmount = RegenPercentage * ship.MaxArmor * 0.01f;
        _initialArmor = ship.Armor;
        return true;
    }
}
