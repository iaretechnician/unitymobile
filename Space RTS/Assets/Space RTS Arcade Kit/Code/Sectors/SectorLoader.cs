using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SectorLoader : MonoBehaviour {

    public static string SECTORS_ROOT = SaveGame.PERSISTANCE_PATH + "Data/Sectors/";

    // Loading sequence: 
    // Sector to be loaded is written in SectorNavigation.CurrentSector
    void Awake() {
        // Stage 1
        LoadSectorData(SectorNavigation.CurrentSector);

        // Stage 2
        LoadGame.LoadAutosave();
        LoadGame.SpawnPlayerShips();

        // Remove the sectorloader once it has loaded everything
        Destroy(gameObject);
    }

    public static Dictionary<string, SerializableUniverseSector> LoadUniverse()
    {
        Dictionary<string, SerializableUniverseSector> data = (Dictionary < string, SerializableUniverseSector > )LoadBinaryFile(SaveGame.PERSISTANCE_PATH + "Data/Universe");
        if(data == null) { 
            Debug.LogWarning("Tried to load universe but universe was not found! (creating Universe...)");
            data = new Dictionary<string, SerializableUniverseSector>();
        }

        return data;
    }

    public static object LoadBinaryFile(string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        if (!File.Exists(path))
        {
            return null;
        }

        FileStream stream = new FileStream(path, FileMode.Open);

        object data = formatter.Deserialize(stream);
        stream.Close();

        return data;
    }

    public static void LoadSectorData(string currentSector)
    {
        Flare[] Flares = SectorVisualData.Instance.Flares;
        Material[] Skybox = SectorVisualData.Instance.Skybox;

        BinaryFormatter formatter = new BinaryFormatter();
        string path = SECTORS_ROOT + currentSector;

        if (!File.Exists(path))
        {
            Debug.LogError("Tried to load sector but file "+path+" was not found!");
            return;
        }

        FileStream stream = new FileStream(path, FileMode.Open);
        SerializableSectorData data = formatter.Deserialize(stream) as SerializableSectorData;
        stream.Close();

        GameObject spawnedObject;
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        SectorNavigation.SectorSize = data.Size;
        GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>().flare = Flares[data.StarIndex];
        RenderSettings.skybox = Skybox[data.SkyboxIndex];
        // This fixes the ambient light problem when dynamically changing skyboxes
        DynamicGI.UpdateEnvironment();

        foreach(var stationData in data.Stations) {
            // Spawn the station
            spawnPosition = stationData.Position;
            spawnRotation = Quaternion.Euler(stationData.Rotation);
            spawnedObject = GameObject.Instantiate(ObjectFactory.Instance.Station,
                spawnPosition, spawnRotation);
        }

        foreach (var jumpgateData in data.Jumpgates)
        {
            spawnPosition = jumpgateData.Position;
            spawnRotation = Quaternion.Euler(jumpgateData.Rotation);
            spawnedObject = GameObject.Instantiate(ObjectFactory.Instance.JumpGatePrefab,
                spawnPosition, spawnRotation);

            spawnedObject.name = "Jumpgate " + jumpgateData.Sector;
        }

        foreach (var fieldData in data.Fields)
        {
            spawnPosition = fieldData.Position;
            spawnRotation = Quaternion.Euler(fieldData.Rotation);
            spawnedObject = GameObject.Instantiate(ObjectFactory.Instance.AsteroidFieldPrefab,
                spawnPosition, spawnRotation);

            AsteroidField asteroidField = spawnedObject.GetComponent<AsteroidField>();
            asteroidField.range = fieldData.Range;
            asteroidField.asteroidCount = fieldData.RockCount;
            asteroidField.scaleRange = fieldData.RockScaleMinMax;
            asteroidField.velocity = fieldData.Velocity;
            asteroidField.angularVelocity = fieldData.AngularVelocity;
        }
    }

}
