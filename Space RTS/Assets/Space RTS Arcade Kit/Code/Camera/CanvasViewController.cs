using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CanvasViewController : MonoBehaviour
{
    public GameObject Hud;
    public RectTransform FlightCanvas, TacticalCanvas;
    public float AnimationTime = 1f;
    public AnimationCurve TacticalAnimationCurve;
    public AnimationCurve UniverseAnimationCurve;
    public GameObject TargetMarkers;

    public static bool TacticalModeActive = true;
    private FlightCameraController _thirdPersonCamera;
    private TacticalCameraController _tacticalCamera;

    private static Vector3 TACTICAL_CAMERA_ANGLE = new Vector3(45, 135, 0);
    private static Vector3 CAMERA_UNIVERSE_POSITION = new Vector3(0, 30000, 30000);

    private void Start()
    {
        _thirdPersonCamera = Camera.main.GetComponent<FlightCameraController>();
        _tacticalCamera = Camera.main.GetComponent<TacticalCameraController>();
        Camera.main.transform.position = CAMERA_UNIVERSE_POSITION;
        StartCoroutine(AnimateUniverseTransition(_thirdPersonCamera.GetTargetCameraPosition() + _tacticalCamera.tacticalCameraOffset, false));
        GameController.Instance.IsShipInputDisabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TacticalModeActive = !TacticalModeActive;
            CanvasController.Instance.CloseAllMenus();
            if (TacticalModeActive)
            {
                StartCoroutine(AnimateCameraToTactical());
            }
            else
            {
                StartCoroutine(AnimateCameraToShip());
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(AnimateUniverseTransition(CAMERA_UNIVERSE_POSITION, true));
        }
    }

    private IEnumerator AnimateUniverseTransition(Vector3 endposition, bool shouldLoadUniverseScene)
    {
        float t = 0;
        Vector3 startPosition = Camera.main.transform.position;

        TacticalCanvas.gameObject.SetActive(false);
        Camera.main.gameObject.GetComponent<GridOverlay>().shouldRender = false;

        while (t < AnimationTime)
        {
            t += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startPosition, endposition, UniverseAnimationCurve.Evaluate(t / AnimationTime));
            Camera.main.transform.rotation = Quaternion.Euler(TACTICAL_CAMERA_ANGLE);
            yield return null;
        }

        if (shouldLoadUniverseScene)
        {
            SceneManager.LoadSceneAsync("Universe", LoadSceneMode.Single);
        }
        else
        {
            Camera.main.gameObject.GetComponent<GridOverlay>().shouldRender = true;
            TacticalCanvas.gameObject.SetActive(true);
        }
    }

    private IEnumerator AnimateCameraToShip()
    {
        OnCameraAnimationStart();

        float t = 0;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;

        while (t < AnimationTime)
        {
            t += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startPosition, _thirdPersonCamera.GetTargetCameraPosition(), TacticalAnimationCurve.Evaluate(t / AnimationTime));
            Camera.main.transform.rotation = Quaternion.Lerp(startRotation, Ship.PlayerShip.transform.rotation, TacticalAnimationCurve.Evaluate(t / AnimationTime));
            yield return null;
        }

        // Activate UI elements
        TargetMarkers.SetActive(true);
        FlightCanvas.gameObject.SetActive(true);
        Hud.SetActive(true);

        // Enable direct ship control
        GameController.Instance.IsShipInputDisabled = false;
        // Swap camera scripts
        _thirdPersonCamera.enabled = !TacticalModeActive;
        _tacticalCamera.enabled = TacticalModeActive;

        TextFlash.ShowInfoText("Press M to show tactical overview");
    }

    private IEnumerator AnimateCameraToTactical()
    {
        OnCameraAnimationStart();

        float t = 0;
        Vector3 startPosition = Camera.main.transform.position;
        Quaternion startRotation = Camera.main.transform.rotation;
        Vector3 endposition = Camera.main.transform.position + _tacticalCamera.tacticalCameraOffset;
        Quaternion endRotation = Quaternion.Euler(TACTICAL_CAMERA_ANGLE);

        while (t < AnimationTime)
        {
            t += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startPosition, endposition, TacticalAnimationCurve.Evaluate(t / AnimationTime));
            Camera.main.transform.rotation = Quaternion.Lerp(startRotation, endRotation, TacticalAnimationCurve.Evaluate(t / AnimationTime));
            yield return null;
        }

        Camera.main.transform.rotation = endRotation;

        // Activate UI elements
        TargetMarkers.SetActive(true);
        TacticalCanvas.gameObject.SetActive(true);

        // Disable direct ship control
        GameController.Instance.IsShipInputDisabled = true;
        // Swap camera scripts
        _thirdPersonCamera.enabled = !TacticalModeActive;
        _tacticalCamera.enabled = TacticalModeActive;

        TextFlash.ShowInfoText("Press M to return to ship");
    }

    private void OnCameraAnimationStart()
    {
        TargetMarkers.SetActive(false);

        // Deactivate active UI elements
        FlightCanvas.gameObject.SetActive(false);
        Hud.SetActive(false);
        TacticalCanvas.gameObject.SetActive(false);

        // Terminate direct ship control
        GameController.Instance.IsShipInputDisabled = true;
    }
}
