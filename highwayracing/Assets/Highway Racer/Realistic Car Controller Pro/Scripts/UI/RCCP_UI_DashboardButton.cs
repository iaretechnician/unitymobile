//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright � 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UI dashboard buttons for mobile / desktop.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/UI/RCCP UI Dashboard Button")]
public class RCCP_UI_DashboardButton : MonoBehaviour, IPointerClickHandler {

    /// <summary>
    /// Button types.
    /// </summary>
    public enum ButtonType { StartEngine, StopEngine, ABS, ESP, TCS, Headlights, LeftIndicator, RightIndicator, Low, Med, High, SH, GearUp, GearDown, HazardLights, ChangeCamera, SteeringHelper, TractionHelper, AngularDragHelper, TurnHelper, TrailAttachDetach, GearToggle };
    public ButtonType buttonType = ButtonType.ChangeCamera;

    /// <summary>
    /// On image will be used?
    /// </summary>
    public GameObject imageOn;

    private void OnEnable() {

        CheckImage();
        RCCP_Events.OnVehicleChanged += RCCP_SceneManager_OnVehicleChanged;

    }

    private void RCCP_SceneManager_OnVehicleChanged() {

        CheckImage();

    }

    public void OnPointerClick(PointerEventData eventData) {

        switch (buttonType) {

            case ButtonType.StartEngine:
                RCCP_InputManager.Instance.StartEngine();
                break;

            case ButtonType.StopEngine:
                RCCP_InputManager.Instance.StopEngine();
                break;

            case ButtonType.ABS:
                RCCP_InputManager.Instance.ABS();
                break;

            case ButtonType.ESP:
                RCCP_InputManager.Instance.ESP();
                break;

            case ButtonType.TCS:
                RCCP_InputManager.Instance.TCS();
                break;

            case ButtonType.Headlights:
                RCCP_InputManager.Instance.LowBeamHeadlights();
                break;

            case ButtonType.LeftIndicator:
                RCCP_InputManager.Instance.IndicatorLeftlights();
                break;

            case ButtonType.RightIndicator:
                RCCP_InputManager.Instance.IndicatorRightlights();
                break;

            case ButtonType.HazardLights:
                RCCP_InputManager.Instance.Indicatorlights();
                break;

            case ButtonType.ChangeCamera:
                RCCP_InputManager.Instance.ChangeCamera();
                break;

            case ButtonType.SteeringHelper:
                RCCP_InputManager.Instance.SteeringHelper();
                break;

            case ButtonType.TractionHelper:
                RCCP_InputManager.Instance.TractionHelper();
                break;

            case ButtonType.AngularDragHelper:
                RCCP_InputManager.Instance.AngularDragHelper();
                break;

            case ButtonType.TurnHelper:
                RCCP_InputManager.Instance.TurnHelper();
                break;

            case ButtonType.GearUp:
                RCCP_InputManager.Instance.GearShiftUp();
                break;

            case ButtonType.GearDown:
                RCCP_InputManager.Instance.GearShiftDown();
                break;

            case ButtonType.GearToggle:
                RCCP_InputManager.Instance.ToggleGear();
                break;

        }

        CheckImage();

    }

    private void CheckImage() {

        if (!imageOn)
            return;

        if (!RCCP_SceneManager.Instance.activePlayerVehicle)
            return;

        switch (buttonType) {

            case ButtonType.ABS:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.Stability)
                    imageOn.SetActive(RCCP_SceneManager.Instance.activePlayerVehicle.Stability.ABS);

                break;

            case ButtonType.ESP:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.Stability)
                    imageOn.SetActive(RCCP_SceneManager.Instance.activePlayerVehicle.Stability.ESP);

                break;

            case ButtonType.TCS:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.Stability)
                    imageOn.SetActive(RCCP_SceneManager.Instance.activePlayerVehicle.Stability.TCS);

                break;

            case ButtonType.Headlights:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.Lights)
                    imageOn.SetActive(RCCP_SceneManager.Instance.activePlayerVehicle.Lights.lowBeamHeadlights);

                break;

            case ButtonType.SteeringHelper:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.Stability)
                    imageOn.SetActive(RCCP_SceneManager.Instance.activePlayerVehicle.Stability.steeringHelper);

                break;

            case ButtonType.TractionHelper:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.Stability)
                    imageOn.SetActive(RCCP_SceneManager.Instance.activePlayerVehicle.Stability.tractionHelper);

                break;

            case ButtonType.AngularDragHelper:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.Stability)
                    imageOn.SetActive(RCCP_SceneManager.Instance.activePlayerVehicle.Stability.angularDragHelper);

                break;

            case ButtonType.TurnHelper:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.Stability)
                    imageOn.SetActive(RCCP_SceneManager.Instance.activePlayerVehicle.Stability.turnHelper);

                break;

            case ButtonType.TrailAttachDetach:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.OtherAddonsManager && RCCP_SceneManager.Instance.activePlayerVehicle.OtherAddonsManager.TrailAttacher) {

                    if (RCCP_SceneManager.Instance.activePlayerVehicle.OtherAddonsManager.TrailAttacher.attachedTrailer != null)
                        RCCP_SceneManager.Instance.activePlayerVehicle.OtherAddonsManager.TrailAttacher.attachedTrailer.DetachTrailer();

                }

                break;

            case ButtonType.GearToggle:

                if (RCCP_SceneManager.Instance.activePlayerVehicle.Gearbox)
                    imageOn.SetActive(RCCP_SceneManager.Instance.activePlayerVehicle.Gearbox.automaticTransmission);

                break;

        }

    }

    private void OnDisable() {

        RCCP_Events.OnVehicleChanged -= RCCP_SceneManager_OnVehicleChanged;

    }

}
