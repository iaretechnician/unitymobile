using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SectorSaver
{
  
    public static void SaveSectorToFile(GameObject[] stations, GameObject[] jumpgates, GameObject[] fields, int sectorSize, string fileName)
    {
        SerializableSectorData data = new SerializableSectorData();

        // SYSTEM
        data.SkyboxIndex = GetSkyboxIndex();
        data.StarIndex = GetSunIndex();
        data.Size = sectorSize;

        // currently only STATIONS
        data.Stations = new List<SerializableSpaceObjectData>();
        SerializableSpaceObjectData stationData;
        foreach (var station in stations)
        {
            stationData = new SerializableSpaceObjectData();
            stationData.Position = station.transform.position;
            stationData.Rotation = station.transform.eulerAngles;
            data.Stations.Add(stationData);
        }

        // ENVIRONMENT
        data.Fields = new List<SerializableFieldData>();
        SerializableFieldData fieldData;
        foreach (var fieldObj in fields)
        {
            AsteroidField field = fieldObj.GetComponent<AsteroidField>();
            fieldData = new SerializableFieldData();
            fieldData.Position = field.transform.position;
            fieldData.Rotation = field.transform.eulerAngles;
            fieldData.Range = field.range;
            fieldData.RockCount = field.asteroidCount;
            fieldData.RockScaleMinMax = field.scaleRange;
            fieldData.Velocity = field.velocity;
            fieldData.AngularVelocity = field.angularVelocity;
            data.Fields.Add(fieldData);
        }

        // JUMPGATES
        data.Jumpgates = new List<SerializableGateData>();
        SerializableGateData gateData;
        foreach (var gate in jumpgates)
        {
            gateData = new SerializableGateData();
            gateData.Position = gate.transform.position;
            gateData.Rotation = gate.transform.eulerAngles;
            data.Jumpgates.Add(gateData);
        }

        // Save it
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(SectorLoader.SECTORS_ROOT + fileName, FileMode.OpenOrCreate);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveUniverse()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(SaveGame.PERSISTANCE_PATH + "Data/Universe", FileMode.OpenOrCreate);
        formatter.Serialize(stream, UniverseMap.Sectors);
        stream.Close();
    }

    private static int GetSkyboxIndex()
    {
        for (int i = 0; i < SectorVisualData.Instance.Skybox.Length; i++)
            if (SectorVisualData.Instance.Skybox[i] == RenderSettings.skybox)
                return i;
        return 0;
    }

    private static int GetSunIndex()
    {
        Flare sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>().flare;

        for (int i = 0; i < SectorVisualData.Instance.Flares.Length; i++)
            if (SectorVisualData.Instance.Flares[i] == sun)
                return i;
        return 0;
    }

}
