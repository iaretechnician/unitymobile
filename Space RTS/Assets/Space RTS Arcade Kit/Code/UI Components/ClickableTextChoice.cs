using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clickable text item whose value is changable via click or keyboard
/// </summary>
public class ClickableTextChoice : ClickableText {

    public Text ValueText;

    private string[] _options;
    private int _selectedOption;

	void Start () {
        _selectedOption = 0;

        // When this button is clicked cycle the options
        button.onClick.AddListener(() =>
        {
            _selectedOption = (_selectedOption + 1) % _options.Length;
            ValueText.text = _options[_selectedOption];
        });
	}
	
	void Update () {
        // Disregard input if item not selected
        if (text.color != Color.red)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _selectedOption = (_selectedOption - 1) % _options.Length;
            ValueText.text = _options[_selectedOption];
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _selectedOption = (_selectedOption + 1) % _options.Length;
            ValueText.text = _options[_selectedOption];
        }
        
    }

    public void SetOptions(string[] givenOptions)
    {
        _options = givenOptions;
        ValueText.text = _options[_selectedOption];
    }

    override public Button.ButtonClickedEvent GetButtonOnClick()
    {
        return button.onClick;
    }
}
