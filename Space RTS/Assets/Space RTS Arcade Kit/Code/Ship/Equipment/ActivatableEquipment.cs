public abstract class ActivatableEquipment: Equipment
{
    /// <summary>
    /// Activates or deactivates the item. Item must be activatable (isActivatable = true)
    /// </summary>
    /// <returns>Whether the item has changed state</returns>
    public abstract bool SetActive(bool active, Ship ship);

    public override void InitItem(Ship ship)
    {
        isActivateable = true;
    }
}
