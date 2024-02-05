//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

#pragma warning disable 0414

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mobile UI Drag used for orbiting Showroom Camera.
/// </summary>
public class RCCP_UI_MobileDrag : MonoBehaviour, IDragHandler, IEndDragHandler {

    /// <summary>
    /// Showroom camera.
    /// </summary>
    private RCCP_ShowroomCamera showroomCamera;

    private void Awake() {

        showroomCamera = FindObjectOfType<RCCP_ShowroomCamera>(true);

    }

    public void OnDrag(PointerEventData data) {

        if (showroomCamera)
            showroomCamera.OnDrag(data);

    }

    public void OnEndDrag(PointerEventData data) {

        //

    }

}
