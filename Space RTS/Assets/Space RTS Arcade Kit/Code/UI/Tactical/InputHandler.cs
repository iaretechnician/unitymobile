using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(IntentLineManager))]
public class InputHandler: Singleton<InputHandler>
{
    public enum OrderType
    {
        MOVE, ATTACK, MOVE_WP
    }

    [HideInInspector]
    public List<GameObject> SelectedShips;
    public TargetInfocard Infocard;     

    private IntentLineManager _intentLineManager;
    private bool _isPlayerShipSelected = true;

    private List<Vector3> _selectedWaypoints;
    private bool _isMultipleSelecting = false;

    #region Selection Utility Rectangles
    private static Texture2D _whiteTexture;
    private static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }


    private static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    private static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    private static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    private static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    bool isSelecting = false;
    Vector3 mousePosition1;

    private bool IsWithinSelectionBounds(GameObject gameObject)
    {
        var camera = Camera.main;
        var viewportBounds =
            GetViewportBounds(camera, mousePosition1, Input.mousePosition);

        return viewportBounds.Contains(
            camera.WorldToViewportPoint(gameObject.transform.position));
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            // Create a rect from both mouse positions
            var rect = GetScreenRect(mousePosition1, Input.mousePosition);
            // Draw transparent rectangle
            DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            // Draw rectangle border
            DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
    #endregion Selection Utility Rectangles

    private void Awake()
    {
        _intentLineManager = GetComponent<IntentLineManager>();
        Ship.ShipDestroyedEvent += OnShipDestroyed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isPlayerShipSelected)
        {
            Ship otherShip = SelectedShips[0].GetComponent<Ship>();

            Ship.PlayerShip.IsPlayerControlled = false;
            otherShip.IsPlayerControlled = true;
            Camera.main.GetComponent<FlightCameraController>().SetTargetShip(otherShip);
        }

        if (Input.GetMouseButtonDown(0))
        {
            mousePosition1 = Input.mousePosition;
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SelectedShips.Clear();
                Infocard.InfocardPanel.SetActive(false);                
            }
        }
        if(Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, mousePosition1) > 10)
        {
            isSelecting = true;
        }
        if (Input.GetMouseButtonUp(0))
        {

            if (!isSelecting)
            {
                return;
            }

            isSelecting = false;
            SelectedShips.Clear();
            Infocard.InfocardPanel.SetActive(false);

            foreach (GameObject playerShip in Player.Instance.SpawnedShips.Values){
                if (IsWithinSelectionBounds(playerShip))
                {
                    SelectedShips.Add(playerShip);
                    _isPlayerShipSelected = true;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (!_isMultipleSelecting)
                    OnPositionClicked(Input.mousePosition);
                else
                    OnPositionAdded(Input.mousePosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isMultipleSelecting = true;
            _selectedWaypoints = new List<Vector3>();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isMultipleSelecting = false;
            if(_selectedWaypoints.Count > 0)
            {
                // Issue order
                foreach (GameObject selectedShip in SelectedShips)
                {
                    Ship shipComp = selectedShip.GetComponent<Ship>();
                    if(shipComp.faction != Player.Instance.PlayerFaction)
                    {
                        continue;
                    }
                    shipComp.IsPlayerControlled = false;
                    shipComp.AIInput.MoveWaypoints(_selectedWaypoints);
                    _intentLineManager.RegisterGivenOrder(selectedShip, OrderType.MOVE_WP, _selectedWaypoints.ToArray());
                }
            }
        }
    }

    private void OnShipDestroyed(object sender, EventArgs e)
    {
        GameObject ship = ((GameObject)sender);
        if (SelectedShips.Contains(ship))
        {
            SelectedShips.Remove(ship);
        }
    }

    private void OnPositionAdded(Vector3 mousePosition)
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        float enter = 0.0f;
        if (plane.Raycast(ray, out enter))
        {
            Vector3 positionClicked = ray.GetPoint(enter);
            _selectedWaypoints.Add(positionClicked);
        }
    }

    private void OnPositionClicked(Vector3 mousePosition)
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        float enter = 0.0f;
        if(plane.Raycast(ray, out enter))
        {
            Vector3 positionClicked = ray.GetPoint(enter);
            // Issue order
            foreach (GameObject selectedShip in SelectedShips)
            {
                Ship shipComp = selectedShip.GetComponent<Ship>();
                if (shipComp.faction != Player.Instance.PlayerFaction)
                {
                    continue;
                }
                shipComp.IsPlayerControlled = false;
                shipComp.AIInput.MoveTo(positionClicked);
                _intentLineManager.RegisterGivenOrder(selectedShip, OrderType.MOVE, new Vector3[] { });
            }
        }
    }

    public void OnMarkerLeftClicked(GameObject ship)
    {
        if (ship.GetComponent<Ship>().faction == Player.Instance.PlayerFaction)
        {
            _isPlayerShipSelected = true;
            TextFlash.ShowInfoText("Press R to switch player ship\nPress F to start tracking");
        }
        else
        {
            _isPlayerShipSelected = false;
            TextFlash.ShowInfoText("Press F to start tracking");
        }

        // Select ship
        SelectedShips = new List<GameObject>();
        SelectedShips.Add(ship);
        Infocard.InitializeInfocard(ship.GetComponent<Ship>());

    }

    public void OnMarkerRightClicked(GameObject ship)
    {
        // Issue order
        foreach (GameObject selectedShip in SelectedShips)
        {
            Ship shipComp = selectedShip.GetComponent<Ship>();
            if (shipComp.faction != Player.Instance.PlayerFaction)
            {
                continue;
            }
            shipComp.IsPlayerControlled = false;
            shipComp.AIInput.Attack(ship);
            _intentLineManager.RegisterGivenOrder(selectedShip, OrderType.ATTACK, new Vector3[] { });
        }
    }
}
