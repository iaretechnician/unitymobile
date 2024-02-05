using UnityEngine;

[CreateAssetMenu(menuName = "DataHolders/SectorVisualData")]
public class SectorVisualData: SingletonScriptableObject<SectorVisualData>
{
    public Flare[] Flares;
    public Material[] Skybox;
}
