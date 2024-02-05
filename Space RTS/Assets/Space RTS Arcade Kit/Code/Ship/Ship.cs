using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Ties all the primary ship components together.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ShipAI))]
[RequireComponent(typeof(ShipPhysics))]
[RequireComponent(typeof(ShipEquipment))]
[RequireComponent(typeof(ShipPlayerInput))]
public class Ship : MonoBehaviour
{
    public static event EventHandler ShipDestroyedEvent;

    [Tooltip("Determines how far away a ship will detect other ships")]
    public int ScannerRange;

    #region ship components

    // Player controls
    public ShipPlayerInput PlayerInput
    {
        get { return _playerInput; }
    }
    private ShipPlayerInput _playerInput;

    // Artificial intelligence controls
    public ShipAI AIInput
    {
        get { return _aiInput; }
    }
    private ShipAI _aiInput;

    // Weapon systems
    public ShipEquipment Equipment
    {
        get { return _shipEquipment; }
    }
    private ShipEquipment _shipEquipment;

    // Ship rigidbody physics
    public ShipPhysics Physics
    {
        get { return _physics; }
    }
    private ShipPhysics _physics;

    #endregion ship components

    // Getters for external objects to reference things like input.
    public bool UsingMouseInput
    {
        get { return _playerInput.useMouseInput; }
        set { _playerInput.useMouseInput = value; }
    }
    public Vector3 Velocity { get { return _physics.Rigidbody.velocity; } }
    public float Throttle { get { return _playerInput.throttle; } }
    public bool InSupercruise {
        get { return inSupercruise; }
        set { inSupercruise = value;
            ControlStatusUI.SetSupercruiseIcon(inSupercruise);
        }
    }
    private bool inSupercruise = false;

    // Keep a static reference for whether or not this is the player ship. It can be used
    // by various gameplay mechanics. Returns the player ship if possible, otherwise null.
    public static Ship PlayerShip { get { return _playerShip; } set { _playerShip = value; } }
    private static Ship _playerShip;

    [Tooltip("Ship model details and data holder")]
    public ModelInfo ShipModelInfo;

    [Header("Ship instance info")]
    [HideInInspector]
    // Maximum armor value can be modified by equipment
    [Tooltip("Ship's maximum armor value (modifiable by equipment)")]
    public float MaxArmor;
    [Tooltip("Ship's current armor value")]
    public float Armor;
    [Tooltip("Ship's faction")]
    public Faction faction;
    [Tooltip("Is this ship currently flown by the player?")]
    public bool IsPlayerControlled = true;

    private AudioSource _engineSound;
    private Ship _portWingman = null, _starboardWingman = null;

    private void Awake()
    {
        // Initialize ship properties
        MaxArmor = ShipModelInfo.MaxArmor;
        Armor = MaxArmor;

        _engineSound = GetComponent<AudioSource>();
        _playerInput = GetComponent<ShipPlayerInput>();
        _aiInput = GetComponent<ShipAI>();
        _physics = GetComponent<ShipPhysics>();
        _shipEquipment = GetComponent<ShipEquipment>();

        if (_playerInput == null || _aiInput == null || _physics == null)
            Debug.LogError("Component not found on ship "+name);

        if (IsPlayerControlled)
        {
            _playerShip = this;
        }
    }

    private void Update()
    {

        _engineSound.pitch = 1.0f + Throttle * 2.0f;
        if (_physics.IsEngineOn)
            _engineSound.volume = 1f;
        else
            _engineSound.volume = 0f;

        if (InSupercruise)
            _shipEquipment.SupercruiseDrain();

        // If this is the player ship, then set the static reference. If more than one ship
        // is set to player, then whatever happens to be the last ship to be updated will be
        // considered the player. Don't let this happen.
        if (IsPlayerControlled)
        {
            _playerShip = this;
        }
        
        
        if(_playerShip != this)
        {
            return;
        }

        // Enable or disable autopilot
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A))
        { 
            if (!IsPlayerControlled)
            {
                ConsoleOutput.PostMessage("Autopilot off.", Color.red);
                AIInput.FinishOrder();
            }
            else
            {
                GameObject selectedTarget = HUDMarkers.Instance.GetCurrentSelectedTarget();
                if (selectedTarget != null)
                {
                    ConsoleOutput.PostMessage("Autopilot on.", Color.green);

                    if (selectedTarget.tag == "Ship")
                        AIInput.Follow(selectedTarget.transform);
                    else
                        AIInput.MoveTo(selectedTarget.transform);
                }
            }
        }
    }

    /// <summary>
    /// Turns the engine on and off.
    /// </summary>
    public void ToggleEngine()
    {
        _physics.ToggleEngines();
    }

    private bool isDestroyed = false;
    /// <summary>
    /// Invoked when this ship takes damage. Amount of damage is given.
    /// </summary>
    /// <param name="damage">amount of damage taken</param>
    public void TakeDamage(float damage, bool isPlayerShot)
    {
        if (isDestroyed)
            return;

        if(this == PlayerShip)
        {
            StartCoroutine(FlightCameraController.Shake());
            GameController.Instance.PlaySound(AudioController.Instance.SmallImpact);
        }

        Armor -= damage;
        if(Armor < 0)
        {
            ShipDestroyedEvent(gameObject, EventArgs.Empty);

            isDestroyed = true;

            ParticleController.Instance.CreateShipExplosionAtPos(transform.position);
            if (HUDMarkers.Instance != null && HUDMarkers.Instance.GetCurrentSelectedTarget() == this.gameObject)
                HUDMarkers.Instance.SetGUITarget(null);

            if (isPlayerShot) {
                // Mark player kill
                if (!ShipModelInfo.ExternalDocking)
                {                   
                    Player.Kills.x += 1;
                    TextFlash.ShowInfoText(faction.name + " fighter destroyed!");
                    ConsoleOutput.PostMessage(faction.name + " fighter destroyed!");
                }
                else
                {
                    Player.Kills.y += 1;
                    TextFlash.ShowInfoText(faction.name + " capital ship destroyed!");
                    ConsoleOutput.PostMessage(faction.name + " capital ship destroyed!");
                }
            }
            if (faction == Player.Instance.PlayerFaction)
            {
                if (this == PlayerShip)
                {
                    if (Player.Instance.SpawnedShips.Count == 0)
                    {
                        GameController.Instance.PlaySound(AudioController.Instance.HardImpact);
                    }
                    else
                    {
                        Ship nextplayership = Player.Instance.SpawnedShips.First().Value.GetComponent<Ship>();
                        nextplayership.IsPlayerControlled = true;
                        Camera.main.GetComponent<FlightCameraController>().SetTargetShip(nextplayership);
                    }
                }
            }

            GameObject.Destroy(this.gameObject);           
        }
       
    }

    private void OnDisable()
    {
        SectorNavigation.Ships.Remove(this.gameObject);
    }

    private void OnEnable()
    {
        if (SectorNavigation.Ships != null)
            SectorNavigation.Ships.Add(this.gameObject);
    }

    // Gets the formation position offset when invoked by an escort ship
    public Vector3 GetWingmanPosition(Ship requestee)
    {
        if (_portWingman == null)            // Port slot not occupied
        {
            //Debug.Log("[WINGMAN]: Ship " + requestee.name + " is port wingman for " + name);
            _portWingman = requestee;
            return new Vector3(-ShipModelInfo.CameraOffset, 0, -ShipModelInfo.CameraOffset);
        }
        else if (_starboardWingman == null)  // Starboard slot not occupied
        {
            //Debug.Log("[WINGMAN]: Ship " + requestee.name + " is starboard wingman for " + name);
            _starboardWingman = requestee;
            return new Vector3(-ShipModelInfo.CameraOffset, 0, ShipModelInfo.CameraOffset);
        }
        else    // Both slots occupied, ask port wingman 
        {
            return new Vector3(ShipModelInfo.CameraOffset, 0, -ShipModelInfo.CameraOffset) + _portWingman.GetWingmanPosition(requestee);
        }
    }

    public void RemoveWingman(Ship wingman)
    {
        if (_portWingman == wingman)
        {
            _portWingman = null;
        }
        else
        {
            _starboardWingman = null;
        }
    }

}
