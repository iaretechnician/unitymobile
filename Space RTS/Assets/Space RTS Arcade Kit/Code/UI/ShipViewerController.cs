using UnityEngine;  
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Displays and handles the selection of ships for the model viewer scene.
/// </summary>
public class ShipViewerController : MonoBehaviour {
    public Dropdown ShipSelector;
    public Transform ShipPosition;
    public Button zoomIn, zoomOut;

    private GameObject _ship;

    private void Awake()
    {
        ShipSelector.ClearOptions();
        ShipSelector.onValueChanged.AddListener(OnShipModelSelected);

        foreach(var model in ObjectFactory.Instance.Ships)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(model.GetComponent<Ship>().ShipModelInfo.ModelName);
            ShipSelector.options.Add(option);
        }        

        zoomIn.onClick.AddListener(() => {
            if (Camera.main.fieldOfView >= 30)
                Camera.main.fieldOfView -= 10f;
        });
        zoomOut.onClick.AddListener(() => {
            if (Camera.main.fieldOfView <= 70)
                Camera.main.fieldOfView += 10f;
        });
    }

    private void Start()
    {
        ShipSelector.value = 1;
    }

    private void OnShipModelSelected(int index)
    {
        string shipName = ShipSelector.options[index].text;
        Debug.Log("Selected ship " + shipName);

        if (_ship != null)
            GameObject.Destroy(_ship);

        _ship = GameObject.Instantiate(ObjectFactory.Instance.GetShipByName(shipName), ShipPosition.position, Quaternion.identity);

        // Disable turrets to avoid errors at runtime
        var hardpoints = _ship.GetComponentsInChildren<TurretHardpoint>();
        foreach(var hardpoint in hardpoints)
            hardpoint.enabled = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitToMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShipSelector.Show();
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
        {
            if(Camera.main.fieldOfView >= 30)
                Camera.main.fieldOfView -= 10f;
        }
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (Camera.main.fieldOfView <= 70)
                Camera.main.fieldOfView += 10f;
        }
    }

    private void FixedUpdate()
    {
        if (_ship != null)
        {
            // Rotate slowly
            _ship.transform.Rotate(Vector3.up, 0.3f);
        }
    }
    
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
