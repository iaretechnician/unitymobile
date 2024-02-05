using UnityEngine;

public class Explosion : MonoBehaviour {

    private ParticleSystem _ps;
    private Light _explosionLight;
    private float _initialIntensity;

	void Start () {
		_ps = GetComponent<ParticleSystem>();
        _explosionLight = GetComponent<Light>();
        //GetComponent<AudioSource>().Play();
        if (_explosionLight)
            _initialIntensity = _explosionLight.intensity;
    }
	
	void Update () {
        if (_ps.isStopped)
        {
            Destroy(gameObject);
        }
        if (_explosionLight)
        {
            _explosionLight.intensity = Mathf.Lerp(_initialIntensity, 0, _ps.time / _ps.main.duration);
        }
    }
}
