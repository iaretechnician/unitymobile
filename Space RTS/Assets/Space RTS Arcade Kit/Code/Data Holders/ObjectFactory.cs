using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataHolders/ObjectFactory")]
public class ObjectFactory : SingletonScriptableObject<ObjectFactory>{ 

    public GameObject WaypointPrefab;
    public GameObject JumpGatePrefab;
    public GameObject AsteroidFieldPrefab;
    public GameObject Station;

    public Faction[] Factions;
    public Faction EnemyFaction;

    public GameObject[] Ships;
    public ProjectileWeaponData[] Weapons;
    public Equipment[] Equipment;

    private Dictionary<string, GameObject> _shipPrefabs;
    private Dictionary<string, GameObject> _stationPrefabs;
    private Dictionary<string, ProjectileWeaponData> _weaponPrefabs;
    private Dictionary<string, Equipment> _equipmentPrefabs;

    private void Awake()
    {
        _shipPrefabs = new Dictionary<string, GameObject>();
        _weaponPrefabs = new Dictionary<string, ProjectileWeaponData>();

        foreach (GameObject ShipPrefab in Ships)
            _shipPrefabs.Add(ShipPrefab.GetComponent<Ship>().ShipModelInfo.ModelName, ShipPrefab);
        foreach (ProjectileWeaponData WeaponPrefab in Weapons)
            _weaponPrefabs.Add(WeaponPrefab.name, WeaponPrefab);
    }

    public GameObject GetShipByName(string shipName)
    {
        if(_shipPrefabs == null)
        {
            _shipPrefabs = new Dictionary<string, GameObject>();
            foreach (GameObject ShipPrefab in Ships)
                _shipPrefabs.Add(ShipPrefab.GetComponent<Ship>().ShipModelInfo.ModelName, ShipPrefab);
        }

        if (_shipPrefabs.ContainsKey(shipName))
            return _shipPrefabs[shipName];
        else {
            Debug.LogError("Ship " + shipName + " not found in ObjectFactory!");
            return null;
        }
    }

    public ProjectileWeaponData GetWeaponByName(string weaponName)
    {
        if (weaponName == null || weaponName=="")
            return null;

        if (_weaponPrefabs == null)
        {
            _weaponPrefabs = new Dictionary<string, ProjectileWeaponData>();
            foreach (ProjectileWeaponData WeaponPrefab in Weapons)
                _weaponPrefabs.Add(WeaponPrefab.name, WeaponPrefab);
        }

        if (_weaponPrefabs.ContainsKey(weaponName))
            return _weaponPrefabs[weaponName];
        else
            return null;
    }

    public Equipment GetEquipmentByName(string itemName)
    {
        if (itemName == null || itemName == "")
            return null;

        if (_equipmentPrefabs == null)
        {
            _equipmentPrefabs = new Dictionary<string, Equipment>();
            foreach (Equipment equipmentPrefab in Equipment) {
                _equipmentPrefabs.Add(equipmentPrefab.name, equipmentPrefab);
            }
        }

        if (_equipmentPrefabs.ContainsKey(itemName))
            return _equipmentPrefabs[itemName];
        else
            return null;
    }

    public Faction GetFactionFromName(string name)
    {
        foreach (Faction f in Factions)
        {
            if (name == f.name)
                return f;
        }

        return null;
    }

    /// <summary>
    ///  Static version of the GetFactionFromName function
    /// </summary>
    public static Faction GetFactionFromName(string name, Faction[] factions)
    {
        foreach (Faction f in factions)
        {
            if (name == f.name)
                return f;
        }

        return null;
    }
}
