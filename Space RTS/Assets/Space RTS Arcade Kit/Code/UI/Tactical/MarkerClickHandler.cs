using UnityEngine;
using UnityEngine.EventSystems;

public class MarkerClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject Target
    {
        set
        {
            _target = value;
        }
    }
    private GameObject _target;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Ship selected
            if (_target.tag == "Ship")
            {
                InputHandler.Instance.OnMarkerLeftClicked(_target);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Attack command given
            if (_target.tag == "Ship")
            {
                InputHandler.Instance.OnMarkerRightClicked(_target);
            }
        }
    }
}