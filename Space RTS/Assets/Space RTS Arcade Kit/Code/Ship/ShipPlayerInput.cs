using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Class specifically to deal with input.
/// </summary>
public class ShipPlayerInput : MonoBehaviour
{
    [Tooltip("When using Keyboard/Joystick input, should roll be added to horizontal stick movement.")]
    public bool addRoll = true;
    [Tooltip("When true, the mouse and mousewheel are used for ship input and A/D can be used for strafing like in many arcade space sims.")]
    public bool useMouseInput = true;

    [Range(-1, 1)]
    public float pitch;
    [Range(-1, 1)]
    public float yaw;
    [Range(-1, 1)]
    public float roll;
    [Range(-1, 1)]
    public float strafe;
    [Range(-0.1f, 1)]
    public float throttle;


    // How quickly the throttle reacts to input.
    private const float THROTTLE_SPEED = 0.5f;

    // Keep a reference to the ship this is attached to just in case.
    private Ship ship;

    private Light[] engineTorches;
    private TrailRenderer[] engineTrails;

    private void Awake()
    {
        ship = GetComponent<Ship>();
        engineTorches = GetComponentsInChildren<Light>();
        engineTrails = GetComponentsInChildren<TrailRenderer>();
    }

    private void Update()
    {
        if (!ship.IsPlayerControlled)
            return;

        if (GameController.Instance.IsShipInputDisabled)
            return;

        if (useMouseInput)
        {
            strafe = Input.GetAxis("Horizontal");
            SetStickCommandsUsingMouse();
            if (!ship.InSupercruise)
            {
                UpdateMouseWheelThrottle();
                UpdateKeyboardThrottle(KeyCode.W, KeyCode.S);
            }
        }
        else
        {
            pitch = Input.GetAxis("Vertical");
            yaw = Input.GetAxis("Horizontal");

            if (addRoll)
                roll = -Input.GetAxis("Horizontal") * 0.5f;

            strafe = 0.0f;
            UpdateKeyboardThrottle(KeyCode.R, KeyCode.F);
        }

        roll = Input.GetAxis("Roll");

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            throttle = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleEngines();
        }

        if (Input.GetMouseButtonDown(0))
        {
            CheckTargetSelection();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            CheckTargetSelection();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W))
        {
            ToggleSupercruise();
        }

    }

    public void ToggleSupercruise()
    {
        // Make sure the engine is on
        if (!ship.Physics.IsEngineOn)
        {
            ToggleEngines();
        }

        ship.InSupercruise = !ship.InSupercruise;

        if (ship.InSupercruise)
        {
            StartCoroutine(EngageSupercruise());
            for (int i = 0; i < engineTorches.Length; i++)
            {
                engineTorches[i].intensity = 5f;
            }
        }
        if (!ship.InSupercruise)
        {
            throttle = 1f;
            for (int i = 0; i < engineTorches.Length; i++)
            {
                engineTorches[i].intensity = 1f;
            }
        }
    }

    private void CheckTargetSelection()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        if (Physics.Raycast(ray, out hit, 1000.0f, layerMask))
        {
            if (hit.transform.gameObject == Ship.PlayerShip.gameObject)
                return;

            string tag = hit.transform.gameObject.tag;
            if (tag == "Ship" || tag == "Station" || tag == "Asteroid"
                || tag == "Waypoint" || tag == "Jumpgate")
                HUDMarkers.Instance.SetGUITarget(hit.transform.gameObject);
        }
    }

    private void ToggleEngines()
    {
        ship.ToggleEngine();

        for (int i = 0; i < engineTorches.Length; i++)
        {
            if (ship.Physics.IsEngineOn)
            {
                engineTorches[i].intensity = 1.0f;
                engineTrails[i].gameObject.SetActive(true);
            }
            else
            {
                engineTorches[i].intensity = 0f;
                engineTrails[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Accelerate ship to 3x max throttle
    /// </summary>
    /// <returns></returns>
    private IEnumerator EngageSupercruise()
    {
        while(throttle < 3.0f)
        {
            if (!ship.InSupercruise)
                yield break;

            throttle = Mathf.MoveTowards(throttle, 3.0f, Time.deltaTime * THROTTLE_SPEED);

            yield return null;
        }
        yield return null;
    }


    /// <summary>
    /// Freelancer style mouse controls. This uses the mouse to simulate a virtual joystick.
    /// When the mouse is in the center of the screen, this is the same as a centered stick.
    /// </summary>
    private void SetStickCommandsUsingMouse()
    {
        Vector3 mousePos = Input.mousePosition;

        // Figure out most position relative to center of screen.
        // (0, 0) is center, (-1, -1) is bottom left, (1, 1) is top right.      
        pitch = (mousePos.y - (Screen.height * 0.5f)) / (Screen.height * 0.5f);
        yaw = (mousePos.x - (Screen.width * 0.5f)) / (Screen.width * 0.5f);

        // Make sure the values don't exceed limits.
        pitch = -Mathf.Clamp(pitch, -1.0f, 1.0f);
        yaw = Mathf.Clamp(yaw, -1.0f, 1.0f);
    }

    /// <summary>
    /// Uses R and F to raise and lower the throttle.
    /// </summary>
    private void UpdateKeyboardThrottle(KeyCode increaseKey, KeyCode decreaseKey)
    {
        if(ship.InSupercruise && Input.GetKey(decreaseKey))
        {
            throttle = 1.0f;
            ship.InSupercruise = false;
            return;
        }
        if (ship.InSupercruise)
            return;

        float target = throttle;

        if (Input.GetKey(increaseKey))
            target = 1.0f;
        else if (Input.GetKey(decreaseKey))
            target = -0.1f;

        throttle = Mathf.MoveTowards(throttle, target, Time.deltaTime * THROTTLE_SPEED);
    }

    /// <summary>
    /// Uses the mouse wheel to control the throttle.
    /// </summary>
    private void UpdateMouseWheelThrottle()
    {
        throttle += Input.GetAxis("Mouse ScrollWheel");
        throttle = Mathf.Clamp(throttle, -0.1f, 1.0f);
    }
}