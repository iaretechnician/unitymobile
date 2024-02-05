//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2023 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("BoneCracker Games/Highway Racer/UI/HR UI Horn")]
public class HR_UIHorn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    bool isPressing = false;

    private void OnEnable() {

        if (!RCCP_Settings.Instance.mobileControllerEnabled) {

            gameObject.SetActive(false);
            return;

        }

    }

    private void Update() {

        if (isPressing)
            RCCP_SceneManager.Instance.activePlayerVehicle.Lights.highBeamHeadlights = true;
        else
            RCCP_SceneManager.Instance.activePlayerVehicle.Lights.highBeamHeadlights = false;

    }

    public void OnPointerDown(PointerEventData eventData) {

        isPressing = true;

    }

    public void OnPointerUp(PointerEventData eventData) {

        isPressing = false;

    }

}
