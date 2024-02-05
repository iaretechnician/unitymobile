using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns AI ships and ensures they have something to do, as well as registering
/// the number of destroyed and lost ships in the current game.
/// </summary>
public class AIManager : MonoBehaviour
{
    [Tooltip("Determines how often idling ships (without an order) will receive new orders " +
        "(if they've completed the previous one)")]
    public float CheckInterval = 3f;
    private float _timer = 0;

    // Should account for number of ships spawned, spawn period and type of enemy
    private int _difficulty;
    private ShipSpawner[] _shipSpawners;
    private Dictionary<string, int> _killsByShipType;
    private Dictionary<string, int> _lossesByShipType;
    private int _remainingShips, _shipsSpawned = 0;

    void Start()
    {
        _difficulty = UniverseMap.Sectors[SectorNavigation.CurrentSector].Difficulty;

        var jumpgates = SectorNavigation.Instance.GetJumpgates();
        _shipSpawners = new ShipSpawner[jumpgates.Length];
        for(int i = 0; i < jumpgates.Length; i++)
        {
            _shipSpawners[i] = jumpgates[i].GetComponent<ShipSpawner>();
        }

        _killsByShipType = new Dictionary<string, int>();
        _lossesByShipType = new Dictionary<string, int>();

        Ship.ShipDestroyedEvent += OnShipDestroyed;

        StartCoroutine(SpawnShips());
    }
  
    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > CheckInterval)
        {
            _timer = 0;

            GameObject[] aiShips = SectorNavigation.Ships.ToArray();
            Ship currentShip;

            // Check each ship to see if one has finished its order
            foreach (GameObject ship in aiShips)
            {
                currentShip = ship.GetComponent<Ship>();
                if (currentShip.faction == Player.Instance.PlayerFaction)
                    continue;

                if (currentShip.AIInput.CurrentOrder == null)     // give new order
                    currentShip.AIInput.AttackAll();
            }
        }
    }

    private void OnShipDestroyed(object sender, System.EventArgs e)
    {
        Ship ship = ((GameObject)sender).GetComponent<Ship>();
        if(ship.faction == Player.Instance.PlayerFaction)
        {
            RegisterLoss(ship.ShipModelInfo);
        }
        else
        {
            RegisterKill(ship.ShipModelInfo);
        }
    }

    private IEnumerator SpawnShips() { 
        int spawnPoints = _shipSpawners.Length;
        _remainingShips = 0;
        for(int i=0; i<_difficulty; i++)
        {
            _shipSpawners[Random.Range(0, spawnPoints)].SpawnRandomShip();
            _shipsSpawned++;
            _remainingShips++;
            yield return new WaitForSeconds(5f);
        }
    }

    public void RegisterKill(ModelInfo ship)
    {
        if (_killsByShipType.ContainsKey(ship.ModelName))
            _killsByShipType[ship.ModelName]++;
        else
            _killsByShipType.Add(ship.ModelName, 1);

        TextFlash.ShowInfoText("Enemy "+ship.ModelName + " destroyed!");
        if(--_remainingShips == 0 && _shipsSpawned == _difficulty)
        {
            CanvasController.Instance.OpenSummaryScreen(this);
        }
    }

    public void RegisterLoss(ModelInfo ship)
    {
        if (_lossesByShipType.ContainsKey(ship.ModelName))
            _lossesByShipType[ship.ModelName]++;
        else
            _lossesByShipType.Add(ship.ModelName, 1);

        TextFlash.ShowInfoText("A ship has been destroyed in action!");

        if (Player.Instance.SpawnedShips.Count == 0)
        {
            CanvasController.Instance.OpenSummaryScreen(this);
        }
    }

    public Dictionary<string, int> GetDestroyedShips()
    {
        return _killsByShipType;
    }

    public Dictionary<string, int> GetLostShips()
    {
        return _lossesByShipType;
    }
}
