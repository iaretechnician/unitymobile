using UnityEngine;

public class GenerateRandomSector
{
    public static int SectorSize
    {
        get { return sectorSize; }
    }
    private static int sectorSize = 3000;

    private const float FIELD_SPAWN_PROBABILITY = 0.7f;
    private const float NEBULA_SPAWN_PROBABILITY = 0.6f;

    public static void Randomize(Faction sectorFaction = null, float influence = 0)
    {
        Flare[] flares = SectorVisualData.Instance.Flares;
        Material[] skybox = SectorVisualData.Instance.Skybox;

        GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>().flare = flares[Random.Range(0, flares.Length)];
        RenderSettings.skybox = skybox[Random.Range(0, skybox.Length)];
        // This fixes the ambient light problem when dynamically changing skyboxes
        DynamicGI.UpdateEnvironment();

        sectorSize = Random.Range(2, 5) * 1000;
        Debug.Log("Generated sector size: " + sectorSize);

        // Spawn stations
        for(int i=0; i<Random.Range(0,3); i++)
        {
            GameObject.Instantiate(ObjectFactory.Instance.Station, GetRandomPosition(), GetRandomRotation());
        }

        // Spawn asteroid fields
        AsteroidField field;
        for (int i = 0; i < 3; i++)
        {
            if (Random.value > FIELD_SPAWN_PROBABILITY) 
                continue;

            field = GameObject.Instantiate(ObjectFactory.Instance.AsteroidFieldPrefab, GetRandomPosition(), Quaternion.identity).GetComponent<AsteroidField>();
            field.range = Random.Range(800, 3500);
            field.velocity = Random.Range(0, 15);
            field.asteroidCount = Random.Range(2, 15) * 100;
            int rockSizeMin = Random.Range(2, 15);
            field.scaleRange = new Vector2(rockSizeMin, rockSizeMin+Random.Range(2, 15));
        }

        // Spawn jumpgates
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            GameObject.Instantiate(ObjectFactory.Instance.JumpGatePrefab, GetRandomPosition(), Quaternion.Euler(0, Random.Range(0, 360), 0));
        }

    }

    #region Utils
    private static Faction GetRandomFaction(Faction[] factions)
    {
        return factions[Random.Range(0, factions.Length - 1)];
    }

    private static Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(-sectorSize/2, sectorSize / 2),
            Random.Range(-50, 50),
            Random.Range(-sectorSize / 2, sectorSize / 2)
            );
    }

    public static Color GetRandomColor()
    {
        return new Color(
            Random.value*0.5f + 0.5f,
            Random.value * 0.5f + 0.5f,
            Random.value * 0.5f + 0.5f
            );
    }

    private static Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }
    #endregion Utils
}
