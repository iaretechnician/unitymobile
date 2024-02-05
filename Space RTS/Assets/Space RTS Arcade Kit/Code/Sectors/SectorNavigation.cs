using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Keeps references to all interactable game objects in the current sector (scene).
/// Is used to obtain ships, stations, jumpgates, cargo crates, etc.
/// </summary>
public class SectorNavigation : Singleton<SectorNavigation> {

    #region static members
    public static string CurrentSector {
        get { return _currentSector; }
    }
    private static string _currentSector = "Sector1";

    public static string PreviousSector
    {
        get { return _previousSector; }
    }
    private static string _previousSector = null;

    public static int SectorSize;
    public static List<GameObject> Ships;
    public static List<GameObject> Stations;
    public static List<GameObject> Jumpgates;

    /// <summary>
    /// Sets the current sector when jumping or loading game.
    /// </summary>
    /// <param name="newSector">Sector name to be set</param>
    /// <param name="markPreviousSector">Set true if jumping and false if loading</param>
    public static void ChangeSector(string newSector, bool markPreviousSector)
    {
        if(markPreviousSector)
            _previousSector = _currentSector;
        _currentSector = newSector;
        Ships = new List<GameObject>();
        Stations = new List<GameObject>();
        Jumpgates = new List<GameObject>();
    }

    private static void GetExistingObjects()
    {
        Ships = new List<GameObject>();
        Stations = new List<GameObject>();
        Jumpgates = new List<GameObject>();

        // Find all pre-existing sector entities
        Ships.AddRange(GameObject.FindGameObjectsWithTag("Ship"));
        Stations.AddRange(GameObject.FindGameObjectsWithTag("Station"));
        Jumpgates.AddRange(GameObject.FindGameObjectsWithTag("Jumpgate"));
    }

    #endregion static members


    void Awake()
    {
        GetExistingObjects();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetExistingObjects();
    }

    public List<GameObject> GetNearbyShips(Transform position, int distance, int number)
    {
        var ships = GetShipsInRange(position, distance, number);
        for(int i=0; i< ships.Count; i++)
        {
            if (ships[i].GetComponent<Ship>().faction == Player.Instance.PlayerFaction)
                ships.RemoveAt(i);
        }

        return ships;
    }

    /// <summary>
    /// Returns a required number of selectable objects (ships, stations, loot, etc.)
    /// within a desired range of a given object.
    /// </summary>
    /// <param name="shipPosition">Source of the scanner</param>
    /// <param name="scannerRange">Range of the scanner</param>
    /// <param name="num">Maximum number of required targets</param>
    /// <returns></returns>
    public List<GameObject> GetClosestObjects(Transform shipPosition, float scannerRange, int num)
    {
        List<GameObject> objectsInRange = new List<GameObject>();

        objectsInRange.AddRange(GetShipsInRange(shipPosition, scannerRange, num));
  
        foreach (GameObject obj in Stations)
        {
            if (Vector3.Distance(shipPosition.position, obj.transform.position) < scannerRange)
            {
                objectsInRange.Add(obj);
                num--;

                if (num <= 0)
                    return objectsInRange;
            }
        }

        foreach (GameObject obj in Jumpgates)
        {
            if (Vector3.Distance(shipPosition.position, obj.transform.position) < scannerRange)
            {
                objectsInRange.Add(obj);
                num--;

                if (num <= 0)
                    return objectsInRange;
            }
        }

        return objectsInRange;
    }

    /// <summary>
    /// Returns a required number of ships
    /// within a desired range of a given object.
    /// </summary>
    /// <param name="shipPosition">Source of the scanner</param>
    /// <param name="scannerRange">Range of the scanner</param>
    /// <param name="num">Maximum number of required targets</param>
    /// <returns></returns>
    public List<GameObject> GetShipsInRange(Transform shipPosition, float scannerRange, int num)
    {
        List<GameObject> objectsInRange = new List<GameObject>();

        foreach (GameObject ship in Ships)
        {
            if (Vector3.Distance(shipPosition.position, ship.transform.position) < scannerRange)
            {
                if(ship != shipPosition.gameObject)
                    objectsInRange.Add(ship);
                num--;

                if (num <= 0)
                    return objectsInRange;
            }

        }

        return objectsInRange;
    }

    /// <summary>
    /// Returns a the closes found ship which is of a hostile faction.
    /// </summary>
    /// <param name="shipPosition">Source of the scanner</param>
    /// <param name="scannerRange">Range of the scanner</param>
    /// <returns></returns>
    public List<GameObject> GetClosestEnemyShip(Transform shipPosition, float scannerRange)
    {
        Dictionary<GameObject, float> shipDistances = new Dictionary<GameObject, float>();
        Faction myfaction = shipPosition.gameObject.GetComponent<Ship>().faction;
        float distance;

        foreach (GameObject ship in Ships)
        {
            distance = Vector3.Distance(shipPosition.position, ship.transform.position);
            if (distance < scannerRange)
            {
                if (ship == shipPosition.gameObject)
                    continue;

                Faction shipFaction = ship.GetComponent<Ship>().faction;

                if(myfaction.RelationWith(shipFaction) < 0)
                    shipDistances.Add(ship, distance);
            }

        }

        // Sort by distance to get closest targets
        List<KeyValuePair<GameObject, float>> shipList = shipDistances.ToList();

        shipList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

        List<GameObject> closestContacts = new List<GameObject>();

        foreach(KeyValuePair<GameObject, float> pair in shipList){
            closestContacts.Add(pair.Key);
        }

        return closestContacts;
    }

    public Transform[] GetWaypoints()
    {
        List<Transform> waypoints = new List<Transform>();

        foreach (GameObject child in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
             waypoints.Add(child.transform);
        }

        return waypoints.ToArray();
    }

    public GameObject[] GetJumpgates()
    {
        // Why is this happening? TODO
        if(Jumpgates == null || Jumpgates.Count == 0) {
            Jumpgates = new List<GameObject>();
            Jumpgates.AddRange(GameObject.FindGameObjectsWithTag("Jumpgate"));
        }

        return Jumpgates.ToArray();
    }

}
