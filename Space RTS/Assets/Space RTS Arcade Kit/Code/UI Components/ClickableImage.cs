using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Used by the Ingame Menu this controls clicking on clickable menu images
/// </summary>
public class ClickableImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    private Image _image;

	void Awake () {
        _image = GetComponent<Image>();
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.OnPointerEnter(System.EventArgs.Empty, gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.OnPointerExit(System.EventArgs.Empty, gameObject);
    }
   
    public void SetColor(Color color)
    {
        if (_image == null)
            _image = GetComponent<Image>();
        _image.color = color;
    }
    
}
