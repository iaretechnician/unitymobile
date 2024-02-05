using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class LoadGame {    

    public static void LoadAutosave()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        if (!File.Exists(SaveGame.PERSISTANCE_PATH + "Data/Profiles/" + ProfileMenuController.PLAYER_PROFILE + "/Autosave"))
        {
            Debug.LogWarning("Tried to load autosave but autosave was not found!");
            SaveGame.CreateNewProfile();
        }
        FileStream stream = new FileStream(SaveGame.PERSISTANCE_PATH + "Data/Profiles/" + ProfileMenuController.PLAYER_PROFILE + "/Autosave", FileMode.Open);

        SerializablePlayerData data = formatter.Deserialize(stream) as SerializablePlayerData;
        stream.Close();

        Player.Name = data.Name;
        Player.Level = data.Level;
        Player.Credits = data.Credits;
        Player.Kills = data.Kills;
        Player.SectorsTaken = data.SectorsTaken;

        Player.ShipDescriptor playerShip;
        Player.OwnedShips.Clear();
        // Load Ships
        foreach (var shipModel in data.Ships)
        {
            playerShip = new Player.ShipDescriptor();
            playerShip.ModelName = shipModel.Model;
            playerShip.IsPlayerShip = shipModel.IsPlayerShip;
            Player.OwnedShips.Add(playerShip);
        }

    }

    public static void SpawnPlayerShips()
    {
        GameObject shipObj;
        Ship ship = null;
        foreach (var shipModel in Player.OwnedShips)
        {
            shipObj = GameObject.Instantiate(ObjectFactory.Instance.GetShipByName(shipModel.ModelName), Player.Instance.transform);
            shipObj.transform.position = new Vector3(Random.Range(-10, 10) * 10, 0, Random.Range(-10, 10) * 10);

            ship = shipObj.GetComponent<Ship>();
            ship.faction = Player.Instance.PlayerFaction;

            Player.Instance.SpawnedShips[shipModel] = shipObj;
        }
        if(ship == null)
        {
            Debug.LogError("There are no player ships! Please check the loaded profile - this shouldn't happen.");
            return;
        }
        ship.IsPlayerControlled = true;
    }

}
