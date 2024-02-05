using System.Collections;
using UnityEngine;
[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
public class FlightCameraController : MonoBehaviour
{
	[HideInInspector] public Transform target;

    public enum CameraState
    {
        Chase, Orbit
    }

	public float distance;
	public float xSpeed;
	public float ySpeed;
	public float yMinLimit;
	public float yMaxLimit;
	public float distanceMin;
	public float distanceMax;
	public float smoothTime;
    public float cameraFollowSpeed = 15f;

	private float _rotationYAxis = 0.0f;
    private float _rotationXAxis = 0.0f;
    private float _velocityX = 0.0f;
    private float _velocityY = 0.0f;

    public CameraState State;

    [HideInInspector]
    public bool AutoOrbit = false;

    // Chase camera
    public float rotateSpeed = 90.0f;
    private Vector3 _startOffset;

    void Start()
	{
        Vector3 angles = transform.eulerAngles;
		_rotationYAxis = angles.y;
		_rotationXAxis = angles.x;
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}

        if (Ship.PlayerShip == null)
            Debug.Log("Player ship is null.");
        else if (target == null)
        {
            _startOffset = new Vector3(0, 5, -Ship.PlayerShip.ShipModelInfo.CameraOffset);
            target = Ship.PlayerShip.transform;
        }

        ControlStatusUI.SetControlStatusText("Keyboard Flight");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            State = (State == CameraState.Orbit) ? CameraState.Chase : CameraState.Orbit;
            if (State == CameraState.Orbit)
            {
                Ship.PlayerShip.UsingMouseInput = false;
            }
            ControlStatusUI.SetControlStatusText(State == CameraState.Orbit ? "Free View" : "Keyboard Flight");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleFlightMode();
        }
    }

    public void ToggleFlightMode()
    {
        if (State != CameraState.Orbit)
        {
            Ship.PlayerShip.UsingMouseInput = !Ship.PlayerShip.UsingMouseInput;
            ControlStatusUI.SetControlStatusText(Ship.PlayerShip.UsingMouseInput ? "Mouse Flight" : "Keyboard Flight");
        }
        else
        {
            AutoOrbit = !AutoOrbit;
            ControlStatusUI.SetControlStatusText(AutoOrbit ? "Orbit View" : "Free View");
        }
    }

    void FixedUpdate()
	{
        if (!target)
            return;

        if (State == CameraState.Orbit)
		{
            // Orbit Camera
            if (!AutoOrbit)
            {
                if(Input.mousePosition.x < Screen.width * 0.25f)
                {
                    _velocityX = (Input.mousePosition.x - 0.25f * Screen.width) * xSpeed * (distance / 10) / (0.25f*Screen.width) * 0.02f;
                }
                if (Input.mousePosition.x > Screen.width * 0.75f)
                {
                    _velocityX = (Input.mousePosition.x - 0.75f * Screen.width) * xSpeed * (distance / 10) / (0.25f * Screen.width) * 0.02f;
                }
                if (Input.mousePosition.y < Screen.height * 0.25f)
                {
                    _velocityY = (Input.mousePosition.y - 0.25f * Screen.height) * ySpeed * (distance / 10) / (0.25f * Screen.height) * 0.02f;
                }
                if (Input.mousePosition.y > Screen.height * 0.75f)
                {
                    _velocityY = (Input.mousePosition.y - 0.75f * Screen.height) * ySpeed * (distance / 10) / (0.25f * Screen.height) * 0.02f;
                }
            }
            else
            {
                _velocityX = 0.1f;
            }
			
			_rotationYAxis += _velocityX;
			_rotationXAxis -= _velocityY;
			_rotationXAxis = ClampAngle(_rotationXAxis, yMinLimit, yMaxLimit);
			Quaternion rotation = Quaternion.Euler(_rotationXAxis, _rotationYAxis, 0);

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                distance = Mathf.Clamp(distance - 10, distanceMin, distanceMax);
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                distance = Mathf.Clamp(distance + 10, distanceMin, distanceMax);
            }            

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;

			transform.rotation = rotation;
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime*cameraFollowSpeed);
			_velocityX = Mathf.Lerp(_velocityX, 0, Time.deltaTime * smoothTime);
			_velocityY = Mathf.Lerp(_velocityY, 0, Time.deltaTime * smoothTime);
		}
        else
        { 
            // Chase camera
            transform.position = Vector3.Lerp(
                transform.position,
                target.TransformPoint(_startOffset),
                Time.deltaTime * cameraFollowSpeed);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                target.rotation,
                rotateSpeed * Time.deltaTime);
        }        
	}

    private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

    public void SetTarget(Transform newTarget, Vector3 offset)
    {
        target = newTarget;
        _startOffset = offset;
        State = CameraState.Orbit;
        AutoOrbit = true;
        distance = 200;
        distanceMin = 150;
        distanceMax = 250;
        xSpeed = 3;
        ySpeed = 3;
    }

    public void SetTargetShip(Ship ship)
    {
        target = ship.transform;
        _startOffset = new Vector3(0, 5, -ship.ShipModelInfo.CameraOffset);
        State = CameraState.Chase;
        AutoOrbit = false;
        distance = ship.ShipModelInfo.CameraOffset;
        distanceMin = ship.ShipModelInfo.CameraOffset;
        distanceMax = ship.ShipModelInfo.CameraOffset *3;
        xSpeed = 15;
        ySpeed = 15;
    }

    public void SetTargetPlayerShip()
    {
        target = Ship.PlayerShip.transform;
        _startOffset = new Vector3(0, 5, -Ship.PlayerShip.ShipModelInfo.CameraOffset);
        State = CameraState.Chase;
        AutoOrbit = false;
        distance = 20;
        distanceMin = 20;
        distanceMax = 50;
        xSpeed = 15;
        ySpeed = 15;
    }

    public static IEnumerator Shake()
    {
        if (CanvasViewController.TacticalModeActive || GameController.Instance.IsShipInputDisabled) // Dont shake if the player is in tactical mode or browsing menus
            yield return null;

        float shakeDuration = 0;

        while (!CanvasViewController.TacticalModeActive && !GameController.Instance.IsShipInputDisabled && shakeDuration < 0.1f)
        {
            Vector3 rotationAmount = Random.insideUnitSphere ;
            rotationAmount.z = 0;   // Don't change the Z; it looks funny.

            shakeDuration += Time.deltaTime;

            Camera.main.transform.eulerAngles += rotationAmount;    // Set the local rotation the be the rotation amount.

            yield return null;
        }
    }

    public Vector3 GetTargetCameraPosition()
    {
        return target == null ? Vector3.zero : target.TransformPoint(_startOffset);
    }
}