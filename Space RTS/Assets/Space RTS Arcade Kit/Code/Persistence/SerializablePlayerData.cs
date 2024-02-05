using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializablePlayerData
{
    // General player info
    public string Name;
    public long Credits;
    public int Level;
    public SerializableVector2 Kills;
    public List<string> SectorsTaken;

    // Ships
    public List<SerializablePlayerShip> Ships;
}

[Serializable]
public class SerializablePlayerShip
{
    public string Model;
    public bool IsPlayerShip;
    
    /// <summary>
    /// Returns the serializable representation of ship data. The nextSector variable is passed because
    /// if the player ship jumps, the game is saved and the player's position needs to be saved as the next system.
    /// </summary>
    public static SerializablePlayerShip FromShip(Ship ship)
    {
        SerializablePlayerShip data = new SerializablePlayerShip();

        data.Model = ship.ShipModelInfo.ModelName;
        data.IsPlayerShip = ship.IsPlayerControlled;

        return data;
    }

}

[Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    public SerializableVector3(Vector3 rValue)
    {
        x = rValue.x;
        y = rValue.y;
        z = rValue.z;
    }

    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}

[Serializable]
public struct SerializableVector2
{
    public float x;
    public float y;

    public SerializableVector2(float rX, float rY)
    {
        x = rX;
        y = rY;
    }

    public override string ToString()
    {
        return String.Format("[{0}, {1}]", x, y);
    }

    public static implicit operator Vector2(SerializableVector2 rValue)
    {
        return new Vector3(rValue.x, rValue.y);
    }

    public static implicit operator SerializableVector2(Vector2 rValue)
    {
        return new SerializableVector2(rValue.x, rValue.y);
    }
}

[Serializable]
public class SerializableKeyValuePair<K, V>
{
    public SerializableKeyValuePair(K key, V value)
    {
        Key = key;
        Value = value;
    }

    public K Key { get; set; }
    public V Value { get; set; }
}
