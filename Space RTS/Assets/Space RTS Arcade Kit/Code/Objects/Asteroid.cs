using UnityEngine;

/// <summary>
/// Allows you to blow up some 'roids
/// </summary>
public class Asteroid : MonoBehaviour {

    [Tooltip("Amount of punishment a 'roid can take")]
    public float Health = 1000;
	
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            ParticleController.Instance.CreateShipExplosionAtPos(transform.position);

            GameObject.Destroy(this.gameObject);
            return;
        }
    }
}
