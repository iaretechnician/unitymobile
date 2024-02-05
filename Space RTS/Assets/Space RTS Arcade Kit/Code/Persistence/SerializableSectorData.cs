using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableSectorData
{
    public int Size;
    public int StarIndex;
    public int SkyboxIndex;

    public List<SerializableSpaceObjectData> Stations;
    public List<SerializableGateData> Jumpgates;
    public List<SerializableFieldData> Fields;
}

[Serializable]
public class SerializableSpaceObjectData
{
    public SerializableVector3 Position;
    public SerializableVector3 Rotation;
    public string ObjectName;

    public static SerializableSpaceObjectData FromSpaceObject(GameObject sectorObject, string name)
    {
        SerializableSpaceObjectData data = new SerializableSpaceObjectData();

        data.Position = sectorObject.transform.position;
        data.Rotation = sectorObject.transform.rotation.eulerAngles;
        data.ObjectName = name;

        return data;
    }
}

[Serializable]
public class SerializableGateData
{
    public SerializableVector3 Position;
    public SerializableVector3 Rotation;
    public string Sector;

    public static SerializableGateData FromGate(GameObject gate)
    {
        SerializableGateData data = new SerializableGateData();

        data.Position = gate.transform.position;
        data.Rotation = gate.transform.rotation.eulerAngles;

        return data;
    }
}

[Serializable]
public class SerializableFieldData
{
    public SerializableVector3 Position;
    public SerializableVector3 Rotation;
    public int RockCount;
    public float Range;
    public SerializableVector2 RockScaleMinMax;
    public float Velocity;
    public float AngularVelocity;
    public string Resource;
    public SerializableVector2 YieldMinMax;

    public static SerializableFieldData FromField(AsteroidField field)
    {
        SerializableFieldData data = new SerializableFieldData();

        data.Position = field.transform.position;
        data.Rotation = field.transform.rotation.eulerAngles;
        data.Range = field.range;
        data.RockCount = field.asteroidCount;
        data.RockScaleMinMax = field.scaleRange;
        data.Velocity = field.velocity;
        data.AngularVelocity = field.angularVelocity;

        return data;
    }
}