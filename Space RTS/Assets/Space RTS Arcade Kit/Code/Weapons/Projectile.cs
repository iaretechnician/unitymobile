using UnityEngine;

/// <summary>
/// A single energy weapon projectile, with no ammo, rendered by a gameobject with cool effects.
/// </summary>
public class Projectile : MonoBehaviour {

    public TrailRenderer Trail;

    private float _range;
    private Vector3 _initialPos;
    private int _damage;

    // To prevent ships from shooting themselves...
    private float _minRange = 15f;
    private SphereCollider _projCollider;

    public bool PlayerShot = false;

    private void Awake()
    {
        _projCollider = GetComponent<SphereCollider>();
    }

    void Update () {
        float distanceTravelled = Vector3.Distance(_initialPos, transform.position);

        if(distanceTravelled > _minRange)
        {
            _projCollider.enabled = true;
        }
		if(distanceTravelled > _range)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
	}

    public void FireProjectile(Vector3 direction, float force, float range, int dmg)
    {

        GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);        
        // To prevent ships from shooting themselves...
        _projCollider.enabled = false;
        this._range = range;
        this._initialPos = transform.position;
        this._damage = dmg;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_projCollider.enabled)
            return;

        ParticleController.Instance.CreateParticleEffectAtPos(collision.contacts[0].point);

        if(collision.gameObject.tag == "Ship")
        {
            collision.gameObject.GetComponent<Ship>().TakeDamage(_damage, PlayerShot);
        }
        else if (collision.gameObject.tag == "Asteroid")
        {
            collision.gameObject.GetComponent<Asteroid>().TakeDamage(_damage);
        }

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(Trail)
            Trail.Clear();
    }
}
