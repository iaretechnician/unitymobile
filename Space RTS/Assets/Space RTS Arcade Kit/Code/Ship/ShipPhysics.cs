using UnityEngine;

/// <summary>
/// Applies linear and angular forces to a ship.
/// This is based on the ship physics from https://github.com/brihernandez/UnityCommon/blob/master/Assets/ShipPhysics/ShipPhysics.cs
/// </summary>
public class ShipPhysics : MonoBehaviour
{
    [Tooltip("X: Lateral thrust\nY: Vertical thrust\nZ: Longitudinal Thrust")]
    [HideInInspector]
    public Vector3 linearForce = new Vector3(100.0f, 100.0f, 100.0f);

    [Tooltip("X: Pitch\nY: Yaw\nZ: Roll")]
    [HideInInspector]
    public Vector3 angularForce = new Vector3(100.0f, 100.0f, 100.0f);

    [Tooltip("Multiplier for all forces. Can be used to keep force numbers smaller and more readable.")]
    private float forceMultiplier = 100.0f;

    public Rigidbody Rigidbody { get { return rbody; } }

    private Vector3 appliedLinearForce = Vector3.zero;
    private Vector3 appliedAngularForce = Vector3.zero;

    private Vector3 maxAngularForce;

    private Rigidbody rbody;

    // Engine kill controls
    private float rBodyDrag;
    public bool IsEngineOn
    {
        get { return isEngineOn;  }
    }
    private bool isEngineOn = true;

    // Keep a reference to the ship this is attached to just in case.
    private Ship ship;

    // Use this for initialization
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        if (rbody == null)
        {
            Debug.LogWarning(name + ": ShipPhysics has no rigidbody.");
        }

        ship = GetComponent<Ship>();
        linearForce = ship.ShipModelInfo.LinearForce;
        angularForce = ship.ShipModelInfo.AngularForce;
    }

    private void Start()
    {
        rBodyDrag = rbody.drag;
        maxAngularForce = angularForce * forceMultiplier;
    }

    void FixedUpdate()
    {
        var linearInput = new Vector3(ship.PlayerInput.strafe, 0, ship.PlayerInput.throttle);
        rbody.AddRelativeForce(MultiplyByComponent(linearInput, linearForce) * forceMultiplier, ForceMode.Force);
        if (rbody != null)
        {
            if(isEngineOn)
                rbody.AddRelativeForce(appliedLinearForce, ForceMode.Force);

            rbody.AddRelativeTorque(
                ClampVector3(appliedAngularForce, -1 * maxAngularForce, maxAngularForce),
                ForceMode.Force);
        }
    }

    private void Update()
    {
        Vector3 linearInput, angularInput;

        if (ship.IsPlayerControlled)
        {
            linearInput = new Vector3(ship.PlayerInput.strafe, 0, ship.PlayerInput.throttle);
            angularInput = new Vector3(ship.PlayerInput.pitch, ship.PlayerInput.yaw, ship.PlayerInput.roll);
            SetPhysicsInput(linearInput, angularInput);
        }
        else
        {
            linearInput = new Vector3(0, 0, ship.AIInput.throttle);
            appliedLinearForce = MultiplyByComponent(linearInput, linearForce) * forceMultiplier;
            appliedAngularForce = ship.AIInput.angularTorque;
            appliedAngularForce.z = 0;
        }
    }

    /// <summary>
    /// Sets the input for how much of linearForce and angularForce are applied
    /// to the ship. Each component of the input vectors is assumed to be scaled
    /// from -1 to 1, but is not clamped.
    /// </summary>
    private void SetPhysicsInput(Vector3 linearInput, Vector3 angularInput)
    {
        appliedLinearForce = MultiplyByComponent(linearInput, linearForce) * forceMultiplier;
        appliedAngularForce = MultiplyByComponent(angularInput, angularForce) * forceMultiplier;
    }

    /// <summary>
    /// Turns the main engine intertial dampening off or on, by disabling the linear drag on the ship.
    /// </summary>
    public void ToggleEngines()
    {
        isEngineOn = !isEngineOn;
        if (!isEngineOn)
        {
            rbody.drag = 0;
            ConsoleOutput.PostMessage("Engines off. ", Color.yellow);
        }
        else
        {
            rbody.drag = rBodyDrag;
            ConsoleOutput.PostMessage("Engines on. ", Color.yellow);
        }
    }

    #region helper methods
    /// <summary>
    /// Returns a Vector3 where each component of Vector A is multiplied by the equivalent component of Vector B.
    /// </summary>
    public static Vector3 MultiplyByComponent(Vector3 a, Vector3 b)
    {
        Vector3 ret;

        ret.x = a.x * b.x;
        ret.y = a.y * b.y;
        ret.z = a.z * b.z;

        return ret;
    }

    /// <summary>
    /// Clamps vector components to a value between the minimum and maximum values given in min and max vectors.
    /// </summary>
    /// <param name="vector">Vector to be clamped</param>
    /// <param name="min">Minimum vector components allowed</param>
    /// <param name="max">Maximum vector components allowed</param>
    /// <returns></returns>
    public static Vector3 ClampVector3(Vector3 vector, Vector3 min, Vector3 max)
    {
        return new Vector3(
            Mathf.Clamp(vector.x, min.x, max.x),
            Mathf.Clamp(vector.y, min.y, max.y),
            Mathf.Clamp(vector.z, min.z, max.z)
            );
    }
    #endregion helper methods
}