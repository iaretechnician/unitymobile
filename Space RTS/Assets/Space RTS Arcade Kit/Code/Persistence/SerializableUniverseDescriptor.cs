using System;

[Serializable]
public class SerializableUniverseSector
{
    public string Name;
    public SerializableVector2 SectorPosition;    
    public int Difficulty;
    public bool IsTaken;

    public SerializableUniverseSector(string name, int x, int y, int difficulty)
    {
        Name = name;
        SectorPosition.x = x;
        SectorPosition.y = y;
        Difficulty = difficulty;
    }
}