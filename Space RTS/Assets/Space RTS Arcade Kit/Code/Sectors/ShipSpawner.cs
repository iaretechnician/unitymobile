using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Attach to jumpgates. Spawns ships periodically.
/// </summary>
public class ShipSpawner : MonoBehaviour {

    [Header("Spawn properties")]
    //public float ProbabilitySquadron = 0.4f;
    public Transform SpawnPos;

    private float spawnTimer;
    private Vector3[] escortOffsets = { new Vector3(30, 0, 0), new Vector3(30, 30, 0), new Vector3(0, 30, 0)};
    private GameObject[] shipPrefabs;

    void Awake()
    {
        shipPrefabs = ObjectFactory.Instance.Ships;
    }

    public void SpawnRandomShip()
    {
        Ship ship;

        // Spawn random ship
        ship = GameObject.Instantiate(
                     shipPrefabs[Random.Range(0, shipPrefabs.Length)],
                     SpawnPos.position,
                     SpawnPos.rotation).GetComponent<Ship>();

        ship.IsPlayerControlled = false;

        // Generate random faction
        Faction shipFaction = ObjectFactory.Instance.EnemyFaction;
        ship.faction = shipFaction;
        ship.gameObject.name = shipFaction.name + " " + ship.ShipModelInfo.ModelName;
        // Give a random loadout
        ShipLoadout.ApplyLoadoutToShip(shipFaction.GetRandomLoadout(), ship);


        // Assign order to ship
        ship.GetComponent<ShipAI>().AttackAll();

        // Generate escort ships, if needed
        /*if (Random.value < ProbabilitySquadron)
        {
            SpawnEscortsFor(ship);
        }*/
    }

    private void SpawnEscortsFor(Ship escortLeader)
    {
        Ship escort = null; 
        int numEscorts = Random.Range(1, 3);

        for (int e_i = 0; e_i < numEscorts; e_i++)
        {
            // Spawn random ship
            escort = GameObject.Instantiate(
                     shipPrefabs[Random.Range(0, shipPrefabs.Length)],
                     SpawnPos.position,
                     SpawnPos.rotation).GetComponent<Ship>();
            escort.IsPlayerControlled = false;
            // Generate random faction
            escort.faction = escortLeader.faction;
            // Give a random loadout
            ShipLoadout.ApplyLoadoutToShip(escortLeader.faction.GetRandomLoadout(), escort);

            escort.gameObject.name = escort.faction.name + " Escort " + escort.ShipModelInfo.ModelName;
            // Assign order to ship
            escort.gameObject.GetComponent<ShipAI>().Follow(escortLeader.transform);
            escort.gameObject.transform.position += escortOffsets[e_i];
        }
        
    }

}
