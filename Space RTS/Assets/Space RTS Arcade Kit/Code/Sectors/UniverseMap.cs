using System.Collections.Generic;
using UnityEngine;

public class UniverseMap : MonoBehaviour {

    #region static functionality
    public static Dictionary<string, SerializableUniverseSector> Sectors;

    static UniverseMap()
    {
        // Load from binary file upon start
        Sectors = SectorLoader.LoadUniverse();
        /*if (Knowledge == null)
            LoadGame.LoadPlayerKnowledge();*/
    }
    #endregion static functionality

    private class SectorConnection{
        public string Sector1;
        public string Sector2;

        public override bool Equals(object obj)
        {
            var connection = obj as SectorConnection;
            return connection != null &&
                   (Sector1 == connection.Sector1 && Sector2 == connection.Sector2) 
                   || (Sector1 == connection.Sector2 && Sector2 == connection.Sector1);
        }
    }

    public GameObject SectorIconPrefab;
    public GameObject LinePrefab;
    public UniverseCanvas Canvas;

    // Dictionary<Map object, Image icon>
    private Dictionary<SerializableUniverseSector, GameObject> _mapIcons;
    private List<SectorConnection> _sectorConnections;

    private void Awake()
    {
        _mapIcons = new Dictionary<SerializableUniverseSector, GameObject>();
        _sectorConnections = new List<SectorConnection>();
    }

    private void Start()
    {
        GameObject sectorIcon;

        // Add sector icons and connections
        foreach(SerializableUniverseSector sector in Sectors.Values)
        {
            sectorIcon = Instantiate(SectorIconPrefab);           
            sectorIcon.transform.position = GetWorldPosition(sector.SectorPosition);
            if (Player.SectorsTaken.Contains(sector.Name))
            {
                sectorIcon.GetComponentsInChildren<ParticleSystem>()[0].startColor = Color.yellow;
            }
            //sectorIcon.GetComponentInChildren<Text>().text = sector.Name;
            sectorIcon.name = sector.Name;
            Canvas.AddSectorDescription(sector.Name, sectorIcon.transform);

            _mapIcons.Add(sector, sectorIcon);

            // Add connection lines to sectors
            foreach (var adjacentSector in GetAdjacentSectors(sector.SectorPosition))
            {
                SectorConnection connection = new SectorConnection();
                connection.Sector1 = sector.Name;
                connection.Sector2 = adjacentSector.Name;

                if (!_sectorConnections.Contains(connection))
                {
                    Vector3 otherSystemPos = GetWorldPosition(adjacentSector.SectorPosition);

                    LineRenderer line = Instantiate(LinePrefab).GetComponent<LineRenderer>();
                    line.useWorldSpace = true;
                    line.SetPositions(new Vector3[] { otherSystemPos, sectorIcon.transform.position });
                    _sectorConnections.Add(connection);
                }
                
            }
        }
    }

    /// <summary>
    /// Converts the universe position of a sector to the 2D map position determined by the 
    /// size of the map rect transform. Universe positions are clamped from -100 to 100.
    /// </summary>
    private Vector3 GetWorldPosition(Vector2 sectorCoords)
    {
        return new Vector3(sectorCoords.x*5, 0, sectorCoords.y*5);
    }

    /// <summary>
    /// Gets the sector at a specific position, and returns null if such doesn't exist
    /// </summary>
    private List<SerializableUniverseSector> GetAdjacentSectors(Vector2 position)
    {
        List<SerializableUniverseSector> adjacentSectors = new List<SerializableUniverseSector>();

        foreach(var sector in Sectors)
        {
            Vector2 sectorPosition = sector.Value.SectorPosition;
            if (Vector2.Distance(sectorPosition, position) > 0f && Vector2.Distance(sectorPosition, position) < 2f)
            {
                adjacentSectors.Add(sector.Value);
            }
        }

        return adjacentSectors;
    }

}
