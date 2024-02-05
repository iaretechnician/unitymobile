using UnityEngine;

[CreateAssetMenu(menuName = "DataHolders/ParticleController")]
public class ParticleController : SingletonScriptableObject<ParticleController> {

    public GameObject ParticleEffectPrefab;
    public GameObject ShipExplosionPrefab;

    /// <summary>
    /// Creates a default particle effect used as placeholder, on a given position
    /// </summary>
    public void CreateParticleEffectAtPos(Vector3 position)
    {
        Instantiate(ParticleEffectPrefab, position, Quaternion.identity);
    }

    /// <summary>
    /// Creates a large ship explosion on a given position
    /// </summary>
    public void CreateShipExplosionAtPos(Vector3 position)
    {
        Instantiate(ShipExplosionPrefab, position, Quaternion.identity);
    }

}
