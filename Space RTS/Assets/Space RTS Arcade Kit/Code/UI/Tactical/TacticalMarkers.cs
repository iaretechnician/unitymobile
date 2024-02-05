using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputHandler))]
public class TacticalMarkers: MonoBehaviour
{
    public GameObject SelectedIndicatorPrefab; 
    public GameObject NonSelectedIndicatorPrefab;
    public GameObject PlayerShipIndicatorPrefab;

    private Image[] _selectedMarkerPool;
    private Image[] _nonSelectedMarkerPool;
    private Image _playerShipMarker;

    private int _poolSize = 30;

    private float _hScreenWidth, _hScreenHeight;
    private Dictionary<int, GameObject> _selectedUnitMap;
    private Dictionary<int, GameObject> _nonSelectedUnitMap;

    private InputHandler _unitSelection;

    private void Start()
    {
        _unitSelection = GetComponent<InputHandler>();

        _selectedUnitMap = new Dictionary<int, GameObject>();
        _nonSelectedUnitMap = new Dictionary<int, GameObject>();
        _hScreenHeight = Screen.height / 2;
        _hScreenWidth = Screen.width / 2;

        _selectedMarkerPool = new Image[_poolSize];
        _nonSelectedMarkerPool = new Image[_poolSize];
       
        // Initialize marker pool
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject selected = Instantiate(SelectedIndicatorPrefab, this.transform);
            _selectedMarkerPool[i] = selected.GetComponent<Image>();            
            selected.SetActive(false);
            _selectedUnitMap.Add(i, null);

            GameObject nonSelected = Instantiate(NonSelectedIndicatorPrefab, this.transform);
            _nonSelectedMarkerPool[i] = nonSelected.GetComponent<Image>();
            nonSelected.SetActive(false);
            _nonSelectedUnitMap.Add(i, null);
        }

        _playerShipMarker = Instantiate(PlayerShipIndicatorPrefab, this.transform).GetComponent<Image>();
    }

    void Update()
    {
        // Process player ship marker
        if (IsObjectOnScreen(Ship.PlayerShip.transform))
        {
            _playerShipMarker.gameObject.SetActive(true);
            // Update marker position
            _playerShipMarker.rectTransform.localPosition = GetScreenPosOfObject(Ship.PlayerShip.transform);
        }
        else
        {
            _playerShipMarker.gameObject.SetActive(false);
        }

        // Selected ships
        var selectedShips = _unitSelection.SelectedShips;
        ProcessTargetMarkers(selectedShips, _selectedUnitMap, _selectedMarkerPool);

        // Non-selected ships
        List<GameObject> shipsInRange =
            SectorNavigation.Instance.GetShipsInRange(Camera.main.transform, 9999, _poolSize);
        shipsInRange.RemoveAll(new Predicate<GameObject>(IsSelectedShip));
        ProcessTargetMarkers(shipsInRange, _nonSelectedUnitMap, _nonSelectedMarkerPool);
    }

    private bool IsSelectedShip(GameObject ship)
    {
        return _unitSelection.SelectedShips.Contains(ship);
    }

    private void ProcessTargetMarkers(List<GameObject> targetList, Dictionary<int, GameObject> markerObjectMap, Image[] markerPool)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            GameObject obj = targetList[i];
            // Check if obj is already attached to a marker
            bool alreadyUsed = false;
            foreach (var markerObj in markerObjectMap.Values)
                if (obj == markerObj)
                {
                    alreadyUsed = true;
                    break;
                }

            if (alreadyUsed)
                continue;

            if (IsObjectOnScreen(obj.transform))
            {
                // Find first available HUD marker
                for (int j = 0; j < _poolSize; j++)
                {
                    if (markerObjectMap[j] == null)
                    {
                        // Assign marker to onscreen object
                        markerObjectMap[j] = obj;

                        markerPool[j].GetComponentInChildren<HealthBar>().SetTarget(obj);
                        markerPool[j].gameObject.SetActive(true);
                        markerPool[j].color = Player.Instance.PlayerFaction.GetTargetColor(obj);
                        markerPool[j].rectTransform.localPosition = GetScreenPosOfObject(obj.transform);
                        markerPool[j].GetComponent<MarkerClickHandler>().Target = obj;
                        markerPool[j].GetComponentInChildren<EquatorialLine>().Target = obj.transform;

                        break;
                    }
                }
            }
        }

        // Pass all markers, turn off unused ones
        for (int j = 0; j < _poolSize; j++)
        {
            if (markerObjectMap[j] != null)
            {
                GameObject obj = markerObjectMap[j];
                if (!IsObjectOnScreen(obj.transform) || !targetList.Contains(obj))
                {
                    // Turn off marker
                    markerPool[j].gameObject.SetActive(false);
                    markerObjectMap[j] = null;
                }
                else
                {
                    // Update marker position
                    markerPool[j].rectTransform.localPosition = GetScreenPosOfObject(obj.transform);
                    // and color.
                    markerPool[j].color = Player.Instance.PlayerFaction.GetTargetColor(obj);
                }
            }
            else
            {
                // Turn off marker
                markerPool[j].gameObject.SetActive(false);
            }
        }
    }

    private bool IsObjectOnScreen(Transform obj)
    {
        float x = Camera.main.WorldToScreenPoint(obj.position).x;
        float y = Camera.main.WorldToScreenPoint(obj.position).y;
        float z = Camera.main.WorldToScreenPoint(obj.position).z;

        // Check if Target is off-screen            
        if (x < 0 || x > Screen.width || y < 0 || y > Screen.height)
        {
            return false;
        }
        else if (z > 0) // Target is in front of the camera
        {
            return true;
        }
        else // Target is behind the camera
        {
            return false;
        }

    }

    private Vector3 GetScreenPosOfObject(Transform target)
    {
        float x = Camera.main.WorldToScreenPoint(target.position).x - _hScreenWidth;
        float y = Camera.main.WorldToScreenPoint(target.position).y - _hScreenHeight;

        return new Vector3(
            Mathf.Clamp(x, -_hScreenWidth, _hScreenWidth),
            Mathf.Clamp(y, -_hScreenHeight, _hScreenHeight), 0f);
    }
}
