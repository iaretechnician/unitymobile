using UnityEngine;
using UnityEngine.EventSystems; 

/// <summary>
/// Handles selection of UI targets by clicking on them.
/// </summary>
public class NonSelectedHUDMarker : MonoBehaviour, IPointerClickHandler
{
    public GameObject markerTarget;

    public void OnPointerClick(PointerEventData eventData)
    {
        HUDMarkers.Instance.SetGUITarget(markerTarget);
    }
}
