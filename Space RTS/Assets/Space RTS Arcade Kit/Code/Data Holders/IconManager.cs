using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DataHolders/Icon Manager")]
public class IconManager : SingletonScriptableObject<IconManager>
{
    [Serializable]
    public struct NamedIcon
    {
        public string name;
        public Sprite icon;
    }

    public Sprite Placeholder;
    public NamedIcon[] Equipment;
    public NamedIcon[] Ships;
    public Sprite[] Weapons;

    private Dictionary<string, Sprite> _equipmentIcons;
    private Dictionary<string, Sprite> _shipIcons;

    public enum EquipmentIcons { Gun = 0, Turret = 1, Equipment = 2 }


    private void Init()
    {
        // Create a hashmap for O(1) access
        _equipmentIcons = new Dictionary<string, Sprite>();
        foreach (var pair in Equipment)
        {
            _equipmentIcons.Add(pair.name, pair.icon);
        }

        _shipIcons = new Dictionary<string, Sprite>();
        foreach (var pair in Ships)
        {
            _shipIcons.Add(pair.name, pair.icon);
        }
    }

    public Sprite GetEquipmentIcon(string itemName)
    {
        if (_equipmentIcons == null)
            Init();

        return _equipmentIcons.ContainsKey(itemName) && _equipmentIcons[itemName] != null
            ? _equipmentIcons[itemName] : Placeholder;
    }

    public Sprite GetShipIcon(string shipName)
    {
        if (_shipIcons == null)
            Init();

        return _shipIcons.ContainsKey(shipName) && _shipIcons[shipName] != null
            ? _shipIcons[shipName] : Placeholder;
    }

    public Sprite GetWeaponIcon(int index)
    {
        return Weapons[index];
    }
}
