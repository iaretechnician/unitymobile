using System.Collections.Generic;
using UnityEngine;
using TurretOrder = TurretCommands.TurretOrder;

public partial class ShipEquipment : MonoBehaviour {

    #region weapons
    [Header("Weapons")]
    // Guns are player/ship controlled
    public GunHardpoint[] Guns;
    // Turrets are individually/automatically controlled
    public TurretHardpoint[] Turrets;

    // Set by AI ship controller when firing conditions are met
    [HideInInspector] public bool IsFiring = false;

    private TurretOrder TurretCmd = TurretOrder.AttackEnemies;
    #endregion

    #region energy management
    [HideInInspector] public float energyCapacity;
    [HideInInspector] public float energyRegenRate;
    [HideInInspector] public float energyAvailable;
    #endregion

    #region equipment
    public List<Equipment> MountedEquipment
    {
        get { return mountedEquipment; }
    }
    private List<Equipment> mountedEquipment;
    #endregion equipment

    private Ship ship;

    private void Awake()
    {
        ship = gameObject.GetComponent<Ship>();
        energyCapacity = ship.ShipModelInfo.GeneratorPower;
        energyRegenRate = ship.ShipModelInfo.GeneratorRegen;
        mountedEquipment = new List<Equipment>();
    }

    private void Update()
    {
        CheckWeaponInput();
        ComputeEnergyRegen();
        UpdateMountedEquipment();
    }

    #region weapons
    private void CheckWeaponInput()
    {
        // Player input
        if (Ship.PlayerShip != null && ship == Ship.PlayerShip && !ship.InSupercruise) {
            if (GameController.Instance.IsShipInputDisabled)
                return;

            if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftControl))
            {
                foreach (GunHardpoint gun in Guns)
                    gun.OnTriggerFireGun(true);

                if (TurretCmd == TurretOrder.Manual)
                    foreach (TurretHardpoint turret in Turrets)
                        turret.OnTriggerFireGun(true);

                IsFiring = false;
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                foreach (GunHardpoint gun in Guns)
                    gun.OnTriggerFireGun(false);

                IsFiring = false;
            }

        }

        // AI input
        if (IsFiring)
        {
            foreach (GunHardpoint gun in Guns)
                gun.OnTriggerFireGun(true);

            IsFiring = false;
        }

    }

    /// <summary>
    /// Sets all turrets to a given state.
    /// </summary>
    /// <param name="order">New order issued to all turrets</param>
    public void SetTurretCommand(TurretOrder order)
    {
        TurretCmd = order;
        foreach (TurretHardpoint turret in Turrets)
            turret.Command = TurretCmd;
    }

    /// <summary>
    /// Get the range of the ship's forward mounted weapons array.
    /// </summary>
    /// <returns></returns>
    public float GetWeaponRange()
    {
        foreach (GunHardpoint gun in Guns)
            if(gun.mountedWeapon != null)
                return gun.mountedWeapon.Range;

        return 0;
    }
    #endregion weapons

    #region energy

    /// <summary>
    /// Apply the energy drain caused by firing the weapon by reducing the available power 
    /// </summary>
    /// <param name="drain">Amount of energy used by the weapon fired</param>
    public void WeaponFired(float drain)
    {
        if(ship.faction == Player.Instance.PlayerFaction)
            energyAvailable = Mathf.Clamp(energyAvailable - drain, 0, energyCapacity);
        else
            energyAvailable = Mathf.Clamp(energyAvailable - drain*1.5f, 0, energyCapacity);
    }

    private void ComputeEnergyRegen()
    {
        if (ship.InSupercruise)
            return;
        energyAvailable = Mathf.Clamp(energyAvailable + Time.deltaTime * energyRegenRate, 0, energyCapacity);
    }

    public void SupercruiseDrain()
    {
        if(energyAvailable > 0)
            energyAvailable = Mathf.Clamp(energyAvailable - Time.deltaTime * energyRegenRate * 3, 0, energyCapacity);
    }
    #endregion energy

    #region equipment
    private void UpdateMountedEquipment()
    {
        // Apply all mounted items
        foreach(Equipment item in mountedEquipment)
        {
            item.UpdateItem(ship);
        }
    }

    /// <summary>
    /// Mounts the specified equipment on the ship, filling an equipment slot
    /// </summary>
    /// <param name="item">Equipment item to mount</param>
    public void MountEquipmentItem(Equipment item)
    {
        // Check if all slots are full
        if(mountedEquipment.Count < ship.ShipModelInfo.EquipmentSlots)
        {
            mountedEquipment.Add(item);
            item.InitItem(ship);
        }
    }

    /// <summary>
    /// Removes the equipment item from the ship. This is invoked when selling equipment
    /// and when saving game.
    /// </summary>
    /// <param name="item">Equipment item to unmount</param>
    public void UnmountEquipmentItem(Equipment item)
    {
        mountedEquipment.Remove(item);
        item.RemoveItem(ship);
    }
    #endregion equipment
}
