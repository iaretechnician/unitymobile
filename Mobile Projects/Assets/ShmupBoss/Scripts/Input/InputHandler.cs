using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Selects which controls are active and how they are processed, if auto fire is enabled and its related 
    /// pointer options and if you are using mobile controls.<br></br>
    /// This class is also dependent on the LevelUI class to find the virtual joystick and buttons.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Input/Input Handler")]
    public sealed class InputHandler : MonoBehaviour
    {
        public static InputHandler Instance;
        
        /// <summary>
        /// Which input method is active and controls the player, if you select the by game manager option; 
        /// the selection of controls vs pointer will happen in the settings menu of the main menu.
        /// </summary>
        [Tooltip("Which input method is active and controls the player, if you select the by game manager option; " +
            "the selection of controls vs pointer will happen in the settings menu of the main menu.")]
        [SerializeField]
        private LevelInputMethod inputMethod;

        /// <summary>
        /// If auto fire is enabled, the player will not need to press fire1 to fire the weapons and they will
        /// always be shooting, if by game manager then the selection will be determined by the main menu settings.
        /// </summary>
        [Tooltip("If auto fire is enabled, the player will not need to press fire1 to fire the weapons and they " +
            "will always be shooting, if by game manager then the selection will be determined by the main menu " +
            "settings.")]
        [SerializeField]
        private AutoFire autoFire;
        public bool IsAutoFireEnabled
        {
            get
            {
                switch (autoFire)
                {
                    case AutoFire.Disabled:
                        return false;

                    case AutoFire.Enabled:
                        return true;

                    case AutoFire.ByGameManager:
                        return GameManager.Instance.CurrentGameSettings.IsAutoFiring;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Makes the movement uniform regardless of anything that can cause differences or 
        /// accelerating/deaccelerating input.
        /// </summary>
        [SerializeField]
        [Tooltip("Makes the movement uniform regardless of anything that can cause differences or " +
            "accelerating/deaccelerating input.")]
        private bool isMovementNormalized;

        /// <summary>
        /// When mobile touch controls are active (pointer input method), this offset 
        /// will enable that the touch pointer will be a little off the player so that 
        /// the user's finger is away from the player ship in the game.
        /// </summary>
        [Header("Pointer Settings")]
        [Tooltip("When mobile touch controls are active (pointer input method), this offset will enable " +
            "that the touch pointer will be a little off the player so that the user's finger is away " +
            "from the player ship in the game.")]
        [SerializeField]
        private Vector2 pointerOffset;

        /// <summary>
        /// This would avoid any possible jitter made from small movements of the mouse or 
        /// touch finger in mobile and make it so only big movements are accounted for. 
        /// </summary>
        [Tooltip("This would avoid any possible jitter made from small movements of the mouse " +
            "or touch finger in mobile and make it so only big movements are accounted for.")]
        [SerializeField]
        private float pointerThreshold = 0.2f;
        
        private Joystick virtualJoystick;
        private HoldButton fire1Button;
        private HoldButton fire2Button;

        private Player playerComp;
        private Vector2 inputDirection;

        private bool isMobileControlsHidden;

        private void Awake()
        {
            Instance = this;
            Initializer.Instance.SubscribeToStage(Initialize, 4);
        }

        private void Update()
        {
            FindInputDirection();
            FindInputButtons();
        }

        private void Initialize()
        {
            if (Level.Instance == null)
            {
                return;
            }

            playerComp = Level.Instance.PlayerComp;

            PlayerInput.IsAutoFireEnabled = IsAutoFireEnabled;

#if (UNITY_ANDROID || UNITY_IOS)
            if (LevelUI.Instance == null)
            {
                return;
            }

            FindMobileControlsInUI();
            ExposeMobileUI();
#endif
        }

        private void FindMobileControlsInUI()
        {
            virtualJoystick = LevelUI.Instance.VirtualJoystick;
            fire1Button = LevelUI.Instance.VirtualFire1Button;
            fire2Button = LevelUI.Instance.VirtualBombButton;
        }

        private void ExposeMobileUI()
        {
            bool isMobileBombButtonNeeded = false;

            if (Bomb.Instance != null)
            {
                isMobileBombButtonNeeded = true;
            }

            switch (inputMethod)
            {
                case LevelInputMethod.Controls:
                    LevelUI.Instance.SetActiveVirtualControls(true, !IsAutoFireEnabled, isMobileBombButtonNeeded);
                    break;

                case LevelInputMethod.Pointer:
                    LevelUI.Instance.SetActiveVirtualControls(false, false, isMobileBombButtonNeeded);
                    break;

                case LevelInputMethod.ByGameManager:
                    if (GameManager.Instance.CurrentGameSettings.CurrentInputMehtod == InputMethod.Controls)
                    {
                        LevelUI.Instance.SetActiveVirtualControls(true, !IsAutoFireEnabled, isMobileBombButtonNeeded);
                    }
                    else
                    {
                        LevelUI.Instance.SetActiveVirtualControls(false, false, isMobileBombButtonNeeded);
                    }
                    break;
            }
        }

        /// <summary>
        /// Finds the input direction after taking into account what type of controls
        /// are active, and if the build is for mobile or desktop and normalizes the
        /// input direction if needed.
        /// </summary>
        private void FindInputDirection()
        {
#if ((UNITY_STANDALONE || UNITY_WEBGL) || UNITY_EDITOR)
            switch (inputMethod)
            {
                case LevelInputMethod.Controls:
                    FindDesktopControlsDirection();
                    break;

                case LevelInputMethod.Pointer:
                    FindMouseDirection();
                    break;

                case LevelInputMethod.ByGameManager:
                    if(GameManager.Instance.CurrentGameSettings.CurrentInputMehtod == InputMethod.Controls)
                    {
                        FindDesktopControlsDirection();
                    }
                    else
                    {
                        FindMouseDirection();
                    }
                    break;
            }
#endif

#if ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR)
            switch (inputMethod)
            {
                case LevelInputMethod.Controls:
                    FindVirtualControlsDirection();
                    break;

                case LevelInputMethod.Pointer:
                    FindTouchDirection();
                    break;

                case LevelInputMethod.ByGameManager:
                    if (GameManager.Instance.CurrentGameSettings.CurrentInputMehtod == InputMethod.Controls)
                    {
                        FindVirtualControlsDirection();
                    }
                    else
                    {
                        FindTouchDirection();
                    }
                    break;
            }
#endif

            if (isMovementNormalized)
            {
                PlayerInput.Direction = inputDirection.normalized;
            }
            else
            {
                PlayerInput.Direction = inputDirection;
            }
        }

        private void FindDesktopControlsDirection()
        {
            float xAxisInput = Input.GetAxis(ProjectInput.Horizontal);
            float yAxisInput = Input.GetAxis(ProjectInput.Vertical);

            inputDirection = new Vector2(xAxisInput, yAxisInput);
        }

        private void FindMouseDirection()
        {
            Vector2 mousePosition = Input.mousePosition;

            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);

            Vector3 mouseCoord = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 directionFromPlayerToMouse = new Vector2(
                (mouseCoord.x + pointerOffset.x) - playerComp.transform.position.x,
                (mouseCoord.y + pointerOffset.y) - playerComp.transform.position.y);

            if (Mathf.Abs(directionFromPlayerToMouse.x) < pointerThreshold)
            {
                directionFromPlayerToMouse.x = 0.0f;
            }

            if (Mathf.Abs(directionFromPlayerToMouse.y) < pointerThreshold)
            {
                directionFromPlayerToMouse.y = 0.0f;
            }

            inputDirection = directionFromPlayerToMouse;
        }

        /// <summary>
        /// Find the direction given by the virtual 
        /// joystick when a mobile build is using the controls option.
        /// </summary>
        private void FindVirtualControlsDirection()
        {
            if (isMobileControlsHidden)
            {
                inputDirection = Vector2.zero;
            }
            else
            {
                inputDirection = virtualJoystick.Direction;
            }
        }

        /// <summary>
        /// Find the direction given by the touch pointer 
        /// when a mobile build is using the pointer option.
        /// </summary>
        private void FindTouchDirection()
        {
            if (Input.touchCount > 0)
            {
                Vector3 touchCoord = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                inputDirection = new Vector2(
                    (touchCoord.x + pointerOffset.x) - playerComp.transform.position.x,
                    (touchCoord.y + pointerOffset.y) - playerComp.transform.position.y);

                if (Mathf.Abs(inputDirection.x) < pointerThreshold)
                {
                    inputDirection.x = 0.0f;
                }

                if (Mathf.Abs(inputDirection.y) < pointerThreshold)
                {
                    inputDirection.y = 0.0f;
                }
            }
            else
            {
                inputDirection = Vector2.zero;
            }
        }

        private void FindInputButtons()
        {
#if ((UNITY_STANDALONE || UNITY_WEBGL) || UNITY_EDITOR)
            FindDesktopInputButtons();
#endif

#if ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR)
            FindMobileButtons();
#endif
        }

        private static void FindDesktopInputButtons()
        {
            PlayerInput.IsFire1Pressed = Input.GetButton(ProjectInput.Fire1);
            PlayerInput.IsFire2Pressed = Input.GetButton(ProjectInput.Fire2);
            PlayerInput.IsSubmitPressed = Input.GetButtonDown("Submit");
            PlayerInput.IsCancelPressed = Input.GetButtonDown("Cancel");
        }

        private void FindMobileButtons()
        {
            switch (inputMethod)
            {
                case LevelInputMethod.Controls:
                    FindVirtualMobileButtons();
                    break;

                case LevelInputMethod.Pointer:
                    FindTouchAsInputButtons();
                    break;

                case LevelInputMethod.ByGameManager:
                    if(GameManager.Instance.CurrentGameSettings.CurrentInputMehtod == InputMethod.Controls)
                    {
                        FindVirtualMobileButtons();
                    }
                    else
                    {
                        FindTouchAsInputButtons();
                    }
                    break;
            }
        }

        private void FindVirtualMobileButtons()
        {
            if (!isMobileControlsHidden)
            {
                PlayerInput.IsFire1Pressed = fire1Button.IsPressed;
            }
            else
            {
                PlayerInput.IsFire1Pressed = false;
            }

            FindVirtualBombButton();
        }

        private void FindTouchAsInputButtons()
        {
            if (Input.touchCount > 0)
            {
                PlayerInput.IsFire1Pressed = true;
            }
            else
            {
                PlayerInput.IsFire1Pressed = false;
            }

            FindVirtualBombButton();
        }

        private void FindVirtualBombButton()
        {
            if (fire2Button == null || Bomb.Instance == null)
            {
                return;
            }

            if (!isMobileControlsHidden)
            {
                PlayerInput.IsFire2Pressed = fire2Button.IsPressed;
            }
            else
            {
                PlayerInput.IsFire2Pressed = false;
            }
        }

        public void ResetVirtualInput()
        {
            isMobileControlsHidden = true;
        }

        public void ReactivateMobileControls()
        {
            isMobileControlsHidden = false;
        }
    }
}