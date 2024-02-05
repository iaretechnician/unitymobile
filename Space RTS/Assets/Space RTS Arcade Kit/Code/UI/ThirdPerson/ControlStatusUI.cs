using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the lower center information regardin the player ship: flight mode and supercruise 
/// </summary>
public class ControlStatusUI : Singleton<ControlStatusUI>
{
    public Image SuperCruiseImage;
    private Text _text;

    void Awake()
    {
        _text = GetComponent<Text>();
        if(SuperCruiseImage == null)
            SuperCruiseImage = transform.GetChild(0).gameObject.GetComponent<Image>();
    }

    public static void SetControlStatusText(string controlStatus)
    {
        if (Instance != null)
        {
            if (Instance._text == null)
                Instance.Awake();
            Instance._text.text = controlStatus;
        }
    }

    public static void SetSupercruiseIcon(bool InSupercruise)
    {
        if(Instance != null)
        {
            if (Instance.SuperCruiseImage != null)
            {
                Color color = Color.white;
                color.a = Ship.PlayerShip.InSupercruise ? 1 : 0.5f;
                Instance.SuperCruiseImage.color = color;
            }
        }
    }

    #region on click listeners for UI buttons

    public void OnControlStatusClicked()
    {
        Camera.main.GetComponent<FlightCameraController>().ToggleFlightMode();
    }

    public void OnSupercruiseClicked()
    {
        Ship.PlayerShip.PlayerInput.ToggleSupercruise();
    }

    #endregion on click listeners for UI buttons
}
