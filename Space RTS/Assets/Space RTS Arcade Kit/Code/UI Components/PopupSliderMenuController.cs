using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSliderMenuController : MonoBehaviour {

    public Text HeaderText;
    public Text InfoText;
    public Text AmountText;
    public Slider Slider;
    public Button AcceptButton, CancelButton;

    private float _keyPressTimer;
    private bool _fastScrollLeft, _fastScrollRight;

    private void Start()
    {
        AmountText.text = "0";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Slider.value --;

            _keyPressTimer = 0.5f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _keyPressTimer -= Time.deltaTime;
            if (_keyPressTimer < 0 && !_fastScrollLeft)
            {
                _fastScrollLeft = true;
                _keyPressTimer = 0.05f;
            }
            if (_keyPressTimer < 0 && _fastScrollLeft)
            {
                Slider.value--;
                _keyPressTimer = 0.05f;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            _fastScrollLeft = false;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Slider.value ++;
       
            _keyPressTimer = 0.5f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _keyPressTimer -= Time.deltaTime;
            if (_keyPressTimer< 0 && !_fastScrollRight)
            {
                _fastScrollRight = true;
                _keyPressTimer = 0.05f;
            }
            if (_keyPressTimer< 0 && _fastScrollRight)
            {
                Slider.value++;
                _keyPressTimer = 0.05f;
            }
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            _fastScrollRight = false;
        }


        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnAcceptClicked();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnCloseClicked();
        }
    }

    public float GetSliderValue()
    {
        return Slider.value;
    }

    #region button callbacks
    public void OnAcceptClicked()
    {
        AcceptButton.onClick.Invoke();
        GameObject.Destroy(this.gameObject);
    }

    public void OnCloseClicked()
    {
        CancelButton.onClick.Invoke();
        GameObject.Destroy(this.gameObject);
    }
    #endregion button callbacks

    #region text setters
    public void SetHeaderText(string newText)
    {
        HeaderText.text = newText;
    }

    public void SetInfoText(string newText)
    {
        InfoText.text = newText;
    }

    public void SetTextFields(string header, string text1)
    {
        HeaderText.text = header;
        InfoText.text = text1;
    }
    #endregion text setters

}
