using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Faction")]
public class Faction : ScriptableObject {

    public Faction[] Friendly, Hostile;

    public Dictionary<Faction, float> cache = new Dictionary<Faction, float>();

    public ShipLoadout[] Loadouts;

    private float _defaultDisposition = 0;

    /// <summary>
    /// Returns relation of this faction with a given faction.
    /// </summary>
    /// <param name="otherFaction">The faction to compare with</param>
    /// <returns>1 if friendly, 1 if hostile, default disposition otherwise</returns>
    public float RelationWith(Faction otherFaction)
    {
        if (cache.ContainsKey(otherFaction))
            return cache[otherFaction];

        float output = _defaultDisposition;

        foreach(Faction faction in Friendly)
        {
            if (faction == otherFaction)
                return 1;
        }

        foreach (Faction faction in Hostile)
        {
            if (faction == otherFaction)
                return -1;
        }

        cache.Add(otherFaction, output);

        return output;
    }

    /// <summary>
    /// Gets the marker color of the target depending on the requestee's faction.
    /// </summary>
    /// <param name="target">Target gameobject</param>
    /// <returns>Red, green or white</returns>
    public Color GetTargetColor(GameObject target)
    {
        Faction targetFaction = null;

        if (target.tag == "Waypoint")
            return Color.magenta;

        if (target.tag == "Ship")
        {
            Ship ship = target.GetComponent<Ship>();
            if(ship != null)
                targetFaction = ship.faction;
        }

        return GetRelationColor(targetFaction);
    }

    public Color GetRelationColor(Faction other)
    {
        float relation = 0;

        // Cyan for player owned ships
        if (other != null && other == Ship.PlayerShip.faction)
            return Color.cyan;

        relation = other == null ? 0 : RelationWith(other);

        if (relation > 0.5)
            return Color.green;
        else if (relation < 0)
            return Color.red;
        else
            return Color.white;
    }

    // Gets a random ship loadout
    public ShipLoadout GetRandomLoadout()
    {
        if (Loadouts == null || Loadouts.Length == 0)
            return null;

        return Loadouts[Random.Range(0, Loadouts.Length)];
    }

}
