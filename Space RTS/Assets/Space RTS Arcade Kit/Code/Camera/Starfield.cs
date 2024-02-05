using UnityEngine;
using System.Collections;

public class Starfield : MonoBehaviour
{   
    public int StarsMax = 1500;
    public float StarSize = 0.8f;
    public float StarDistance = 500;
    public float StarClipDistance = 1;

    private float _starDistanceSqr;
    private float _starClipDistanceSqr;

    private ParticleSystem.Particle[] _points;
    private Vector3[] _velocities;
    private ParticleSystem _ps;

    void Start()
    {
        _ps = GetComponent<ParticleSystem>();
        _starDistanceSqr = StarDistance * StarDistance;
        _starClipDistanceSqr = StarClipDistance * StarClipDistance;
    }


    private void CreateStars()
    {
        _points = new ParticleSystem.Particle[StarsMax];
        _velocities = new Vector3[StarsMax];

        for (int i = 0; i < StarsMax; i++)
        {
            _points[i].position = Random.insideUnitSphere * StarDistance + transform.position;
            _points[i].startColor = new Color(1, 1, 1, 1);
            _points[i].startSize = StarSize;
            _velocities[i] = new Vector3(Random.value - .5f, Random.value - .5f, Random.value - .5f) * 0.2f;
        }

        _ps.SetParticles(_points, _points.Length);
    }


    void LateUpdate()
    {
        if (_points == null) CreateStars();

        for (int i = 0; i < StarsMax; i++)
        {

            if ((_points[i].position - transform.position).sqrMagnitude > _starDistanceSqr)
            {
                _points[i].position = Random.insideUnitSphere.normalized * StarDistance + transform.position;
            }

            if ((_points[i].position - transform.position).sqrMagnitude <= _starClipDistanceSqr)
            {
                float percent = (_points[i].position - transform.position).sqrMagnitude / _starClipDistanceSqr;
                _points[i].startColor = new Color(1, 1, 1, percent);
                _points[i].startSize = percent * StarSize;
            }

            _points[i].position += _velocities[i];

        }

        _ps.SetParticles(_points, _points.Length);
        
    }
}