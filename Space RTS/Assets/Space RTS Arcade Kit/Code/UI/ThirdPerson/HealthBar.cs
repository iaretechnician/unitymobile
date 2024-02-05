using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the target healthbar hovering above the 
/// selected target together with the marker.
/// </summary>
public class HealthBar : MonoBehaviour {

    private Slider healthSlider;
    private GameObject target;

    private Ship targetShip;

    public void Start()
    {
        healthSlider = GetComponent<Slider>();

        SetMaxValue(1f);
    }

    private void Update()
    {
        if (target != null && target.tag == "Ship")
        {
            UpdateSlider(targetShip.Armor/(float)targetShip.MaxArmor);
        }        
    }

    public void SetMaxValue(float value)
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        healthSlider.maxValue = value;
        healthSlider.value = value;
    }

    public void UpdateSlider(float value)
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        healthSlider.value = value;
    }

    public void SetTarget(GameObject targetObject)
    {
        target = targetObject;
        if (target.tag == "Ship")
        {
            this.gameObject.SetActive(true);
            targetShip = target.GetComponent<Ship>();
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public GameObject GetTarget()
    {
        return target;
    }

}
