using UnityEngine;
using UnityEngine.UI;

public class LeadIndicatorUI : MonoBehaviour {

    private Image _image;    

	void Start () {
        _image = GetComponent<Image>();
        _image.enabled = false;
	}
	
	void Update () {
        GameObject target = HUDMarkers.Instance.GetCurrentSelectedTarget();

        // Ship has no weapons
        if (Ship.PlayerShip == null || (Ship.PlayerShip.Equipment.Guns.Length == 0 && Ship.PlayerShip.Equipment.Turrets.Length == 0))
            return;

        float projectileSpeed = Ship.PlayerShip.Equipment.Guns.Length > 0 ? 
            Ship.PlayerShip.Equipment.Guns[0].ProjectileSpeed : Ship.PlayerShip.Equipment.Turrets[0].ProjectileSpeed;

        if (target != null || Ship.PlayerShip == null)
        {
            GameObject shooter = Ship.PlayerShip.gameObject;           

            _image.enabled = true;
            _image.rectTransform.anchoredPosition =
                Targeting.PredictTargetLead2D(Camera.main.gameObject, target, projectileSpeed) - 
                new Vector2(Screen.width/2, Screen.height/2);
        }
        else
        {
            _image.enabled = false;
        }
	}
}
