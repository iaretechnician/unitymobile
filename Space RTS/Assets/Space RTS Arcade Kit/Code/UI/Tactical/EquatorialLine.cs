using System;
using UnityEngine;
using UnityEngine.UI;

public class EquatorialLine : MonoBehaviour
{

    public Transform Target
    {
        get { return _target; }
        set {
            _target = value;
            _line.startColor = _line.endColor = GetComponentInParent<Image>().color;
            RenderLine();
        }
    }
    private Transform _target;
    private LineRenderer _line;
    private float _timer = 2f;

    void Awake()
    {
        _line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        RenderLine();

        _timer -= Time.deltaTime;
        if (_timer < 0)   // TODO
        {
            _timer = 2f;
            _line.startColor = _line.endColor = GetComponentInParent<Image>().color;
        }
    }

    private void RenderLine()
    {
        if (_target != null)
        {
            _line.SetPositions(new Vector3[] {
                _target.position,
                new Vector3(_target.position.x, 0, _target.position.z)
            });
        }
    }

}
