using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TacticalCameraController : MonoBehaviour
{
    public float ScreenEdgeBorderThickness = 5.0f; // distance from screen edge. Used for mouse movement
    public Vector3 tacticalCameraOffset = new Vector3(-200, 200, 200);
    public InputHandler InputHandler;

    [Header("Movement Speeds")]
    [Space]
    public float MinPanSpeed;
    public float MaxPanSpeed;
    public float SecToMaxSpeed; //seconds taken to reach max speed;
    public float ZoomSpeed;
    public bool CanMove = true;

    private Vector2 _zoomLimit;

    private float _panSpeed;
    private Vector3 _panMovement;
    private Vector3 _pos;
    private Vector3 _lastMousePosition;
    private float _panIncrease = 0.0f;

    private GridOverlay _tacticalGrid;
    private bool _isTrackingTarget = false;

    private void Awake()
    {
        _tacticalGrid = GetComponent<GridOverlay>();
    }

    private void OnEnable()
    {
        _tacticalGrid.shouldRender = true;
    }

    private void OnDisable()
    {
        _tacticalGrid.shouldRender = false;
    }

    // Use this for initialization
    void Start()
    {
        _zoomLimit.x = 15;
        _zoomLimit.y = 65;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _isTrackingTarget = !_isTrackingTarget;
        }

        #region Zoom
        Camera.main.fieldOfView -= Input.mouseScrollDelta.y * ZoomSpeed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, _zoomLimit.x, _zoomLimit.y);
        #endregion

        #region Movement
        if (_isTrackingTarget)
        {
            // Keep constant distance to target
            if (InputHandler.SelectedShips.Count == 1)
            {
                Vector3 targetPosition = InputHandler.SelectedShips[0].transform.position;
                transform.position = targetPosition + tacticalCameraOffset;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CanMove = !CanMove;
        }       

        if (!CanMove) return;

        _panMovement = Vector3.zero;
        Vector3 forwardVector = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        Vector3 rightVector = Vector3.ProjectOnPlane(transform.right, Vector3.up);

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - ScreenEdgeBorderThickness)
        {
            _panMovement += forwardVector * _panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= ScreenEdgeBorderThickness)
        {
            _panMovement -= forwardVector * _panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= ScreenEdgeBorderThickness)
        {
            _panMovement += -rightVector * _panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - ScreenEdgeBorderThickness)
        {
            _panMovement += rightVector * _panSpeed * Time.deltaTime;
            //pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            _panMovement += Vector3.up * _panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            _panMovement += Vector3.down * _panSpeed * Time.deltaTime;
        }

        transform.Translate(_panMovement, Space.World);

        //increase pan speed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q)
            || Input.mousePosition.y >= Screen.height - ScreenEdgeBorderThickness
            || Input.mousePosition.y <= ScreenEdgeBorderThickness
            || Input.mousePosition.x <= ScreenEdgeBorderThickness
            || Input.mousePosition.x >= Screen.width - ScreenEdgeBorderThickness)
        {
            _panIncrease += Time.deltaTime / SecToMaxSpeed;
            _panSpeed = Mathf.Lerp(MinPanSpeed, MaxPanSpeed, _panIncrease);
        }
        else
        {
            _panIncrease = 0;
            _panSpeed = MinPanSpeed;
        }
        #endregion
       
    }

}
