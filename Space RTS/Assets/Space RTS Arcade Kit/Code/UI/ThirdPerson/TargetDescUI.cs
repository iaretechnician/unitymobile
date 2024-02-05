using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// When a target is selected its name and distance are displayed in the lower middle.
/// </summary>
public class TargetDescUI : MonoBehaviour {

    private Text _text;
    private GameObject _target;

	void Awake () {
        _text = GetComponent<Text>();
	}
	
	void Update () {
        _target = HUDMarkers.Instance.GetCurrentSelectedTarget();
        
        _text.text = _target == null
            ? "Target: none" 
            : "Target: " + _target.name + "\nDistance: " + (int)Vector3.Distance(Ship.PlayerShip.transform.position, _target.transform.position);
	}
}
