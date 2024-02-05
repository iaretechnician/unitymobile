using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SaveGame
{
#if UNITY_EDITOR
    public const string PERSISTANCE_PATH = "Assets/Space RTS Arcade Kit/";
#else
    public const string PERSISTANCE_PATH = "";
#endif

    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(PERSISTANCE_PATH + "Data/Profiles/" + ProfileMenuController.PLAYER_PROFILE + "/Autosave", FileMode.OpenOrCreate);

        // Save progress
        SerializablePlayerData data = GetPlayerData();
        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved profile "+ ProfileMenuController.PLAYER_PROFILE+" successfully!");
    }

    public static void CreateNewProfile()
    {
        string fullPath = PERSISTANCE_PATH + "Data/Profiles/" + ProfileMenuController.PLAYER_PROFILE;
        BinaryFormatter formatter = new BinaryFormatter();
        if (!Directory.Exists(fullPath)){
            Directory.CreateDirectory(fullPath);
        }
        FileStream stream = new FileStream(fullPath + "/Autosave", FileMode.OpenOrCreate);

        // Save progress
        SerializablePlayerData data = GetNewPlayerData(ProfileMenuController.PLAYER_PROFILE);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    private static SerializablePlayerData GetPlayerData()
    {
        SerializablePlayerData data = new SerializablePlayerData();

        // General player info
        data.Name = Player.Name;
        data.Credits = Player.Credits;
        data.Level = Player.Level;
        data.Kills = new SerializableVector2(Player.Kills.x, Player.Kills.y);
        data.SectorsTaken = Player.SectorsTaken;

        data.Ships = new List<SerializablePlayerShip>();
        foreach (Player.ShipDescriptor ownedShip in Player.OwnedShips)
        {
            SerializablePlayerShip shipModel = new SerializablePlayerShip();
            shipModel.IsPlayerShip = false;
            shipModel.Model = ownedShip.ModelName;
            data.Ships.Add(shipModel);
        }

        return data;
    }

    private static SerializablePlayerData GetNewPlayerData(string profileName)
    {
        SerializablePlayerData data = new SerializablePlayerData();

        // General player info
        data.Name = profileName;
        data.Credits = 30000;
        data.Level = 1;
        data.Kills = new SerializableVector2(0f, 0f);
        data.SectorsTaken = new List<string>();
        data.Ships = new List<SerializablePlayerShip>();

        for(int i=0; i<3; i++)
        {
            SerializablePlayerShip shipModel = new SerializablePlayerShip();
            shipModel.IsPlayerShip = true;
            shipModel.Model = "Fighter";
            data.Ships.Add(shipModel);
        }
        

        return data;
    }

}
