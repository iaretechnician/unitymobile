using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains player data, including a list of in-sector (spawned) and 
/// out-of-sector (owned) ships. 
/// </summary>
public class Player : Singleton<Player> {

    #region static functionality
    static Player()
    {
        // Load from binary file upon start
        LoadGame.LoadAutosave();
        Ship.ShipDestroyedEvent += OnShipDestroyed;
    }

    public static string Name;
    public static long Credits;
    public static int Level;
    // Fighter Kills, Capship Kills
    public static Vector2 Kills;
    public static List<string> SectorsTaken;
    public static List<ShipDescriptor> OwnedShips
    {
        get
        {
            if (_oosShips == null)
                _oosShips = new List<ShipDescriptor>();
            return _oosShips;
        }
    }
    private static List<ShipDescriptor> _oosShips;

    #endregion static functionality

    public Faction PlayerFaction;
    public Dictionary<ShipDescriptor, GameObject> SpawnedShips {
        get
        {
            if (_spawnedShips == null)
                _spawnedShips = new Dictionary<ShipDescriptor, GameObject>();
            return _spawnedShips;
        }
    }
    private Dictionary<ShipDescriptor, GameObject> _spawnedShips;

    /// <summary>
    /// Stores data for player owned ships in other (not player) sectors.
    /// Out-of-sector ships are not simulated.
    /// </summary>
    public class ShipDescriptor
    {
        public string ModelName;
        public bool IsPlayerShip = false;

        public ShipDescriptor(string modelName)
        {
            ModelName = modelName;
        }

        public ShipDescriptor() { }
    }

    private static void OnShipDestroyed(object sender, EventArgs e)
    {
        Ship ship = ((GameObject)sender).GetComponent<Ship>();
        if (ship.faction == Instance.PlayerFaction)
        {
            // Remove player ship record, it's been destroyed
            var item = Instance.SpawnedShips.First(kvp => kvp.Value == (GameObject)sender);
            OwnedShips.Remove(item.Key);
            Instance.SpawnedShips.Remove(item.Key);
        }
    }
}
