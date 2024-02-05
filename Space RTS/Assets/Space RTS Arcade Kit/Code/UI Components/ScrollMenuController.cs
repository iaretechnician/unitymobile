using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A menu with text or image options which has scrollable, interactive content.
/// </summary>
public class ScrollMenuController : MenuController
{
    // Reference to scroll content used for adding objects
    public Scrollbar ScrollBar;

    private float _keyPressTimer;
    private bool _fastScrollDown, _fastScrollUp;
 
    protected void Update()
    {
        if (DisableKeyInput) {
            for (int i = 0; i < _availableOptions.Count; i++)
                _availableOptions[i].SetColor(Color.white);
            return;
        }

        base.Update();

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {           
            ScrollBar.value -= 1f / _availableOptions.Count;

            _keyPressTimer = 0.5f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _keyPressTimer -= Time.deltaTime;
            if(_keyPressTimer < 0 && !_fastScrollDown)
            {
                _fastScrollDown = true;
                _keyPressTimer = 0.1f;
            }
            if (_keyPressTimer < 0 && _fastScrollDown)
            {
                _availableOptions[selectedOption].SetColor(Color.white);
                if (selectedOption + 1 < _availableOptions.Count)
                    selectedOption++;

                ScrollBar.value -= 1f / _availableOptions.Count;
                GameController.Instance.PlaySound(AudioController.Instance.ScrollSound);
                _keyPressTimer = 0.1f;
            }
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            _fastScrollDown = false;
        }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {            
            ScrollBar.value += 1f / _availableOptions.Count;

            _keyPressTimer = 0.5f;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _keyPressTimer -= Time.deltaTime;
            if (_keyPressTimer < 0 && !_fastScrollUp)
            {
                _fastScrollUp = true;
                _keyPressTimer = 0.1f;
            }
            if (_keyPressTimer < 0 && _fastScrollUp)
            {
                _availableOptions[selectedOption].SetColor(Color.white);
                if (selectedOption > 0)
                    selectedOption--;

                ScrollBar.value += 1f / _availableOptions.Count;
                GameController.Instance.PlaySound(AudioController.Instance.ScrollSound);

                _keyPressTimer = 0.1f;
            }
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            _fastScrollUp = false;
        }        

    }

    /// <summary>
    /// This method is invoked by the element that populates this menu. The method 
    /// returns an onclick handler which can be used by the invoker to process the 
    /// option click/selection events.
    /// </summary>
    /// <param name="text">Title/text of the menu option</param>
    /// <param name="color">Color of the item in the list</param>
    /// <param name="icon">Icon of the item</param>
    /// <param name="iconAspect">Aspect ratio of the icon image</param>
    /// <returns>OnClick event handler to be used by the invoker</returns>
    public Button.ButtonClickedEvent AddMenuOption(string text, Color color, Sprite icon, float iconAspect = 1, int optionHeight = 40, Color? textColor = null)
    {
        GameObject listItem = Instantiate(UIElements.Instance.ClickableImageText);
        listItem.name = text;

        RectTransform rt = listItem.GetComponent<RectTransform>();
        rt.SetParent(OptionContainer.transform);

        ClickableTextAndImage nli = listItem.GetComponent<ClickableTextAndImage>();
        nli.SetText(text);
        nli.Icon.color = color;
        nli.Icon.sprite = icon;
        nli.Icon.GetComponent<AspectRatioFitter>().aspectRatio = iconAspect;
        nli.OwnerMenu = this.gameObject;

        nli.Text.color = textColor ?? Color.white;

        listItem.GetComponent<LayoutElement>().preferredHeight = optionHeight;

        _availableOptions.Add(nli);

        return nli.GetButtonOnClick();
    }

    #region component-specific functions   

    protected override void OnOptionSelected(int option)
    {
        if (SubMenu != null)
            return;

        GameController.Instance.PlaySound(AudioController.Instance.SelectSound);

        _availableOptions[option].GetButtonOnClick().Invoke();

    }

    #endregion component-specific functions
}
