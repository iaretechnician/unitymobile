using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Clickable text menu item with an image icon, with hover highlighting
/// </summary>
public class ClickableTextAndImage : ClickableText, IPointerExitHandler
{
    public Color NormalColor, HoverColor;
    public Image Icon;

    private Image _background;

    protected new void Awake()
    {
        _background = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
        button = GetComponent<Button>();
    }

    private void Start()
    {
        _background.color = NormalColor;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.MenuEventArgs args = new EventManager.MenuEventArgs();

        // Save the owner menu object of this clickabletext
        args.Menu = OwnerMenu;

        EventManager.OnPointerEnter(args, gameObject);
        _background.color = HoverColor;
        _background.transform.localScale = Vector3.one*1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _background.color = NormalColor;
        _background.transform.localScale = Vector3.one;
    }

}
