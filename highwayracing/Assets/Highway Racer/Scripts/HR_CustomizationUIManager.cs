//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2023 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// RCCP UI Canvas that manages the event systems, panels, gauges, images and texts related to the vehicle and player.
/// </summary>
[AddComponentMenu("BoneCracker Games/Highway Racer/UI/HR UI Customization Manager")]
public class HR_CustomizationUIManager : MonoBehaviour {

    [Header("Customization Panels")]
    public GameObject paints;        //  Painting panel.
    public GameObject wheels;        //  Wheels panel.
    public GameObject customization;      //  Customization panel.
    public GameObject upgrades;      //  Upgrades panel.
    public GameObject spoilers;       //  Spoilers panel.
    public GameObject sirens;     //  Sirens panel.
    public GameObject decals;     //  Decals panel.
    public GameObject neons;     //  Neons panel.

    [Header("Customization Buttons")]
    public Button paintsButton;        //  Painting button.
    public Button wheelsButton;        //  Wheels button.
    public Button customizationButton;      //  Customization button.
    public Button upgradesButton;      //  Upgrades button.
    public Button spoilersButton;       //  Spoilers button.
    public Button sirensButton;     //  Sirens button.
    public Button decalsButton;     //  Decals button.
    public Button neonsButton;     //  Neons button.

    [Header("Sliders")]
    public Slider engine;
    public Slider handling;
    public Slider brake;
    public Slider speed;

    public void OpenCustomizationPanel(GameObject activeMenu) {

        CloseCustomizationPanels();

        if (activeMenu)
            activeMenu.SetActive(true);

    }

    public void CloseCustomizationPanels() {

        paints.SetActive(false);
        wheels.SetActive(false);
        customization.SetActive(false);
        upgrades.SetActive(false);
        spoilers.SetActive(false);
        sirens.SetActive(false);
        decals.SetActive(false);
        neons.SetActive(false);

    }

    private void Update() {

        if (!HR_MainMenuHandler.Instance) {

            paintsButton.interactable = false;
            wheelsButton.interactable = false;
            customizationButton.interactable = false;
            upgradesButton.interactable = false;
            spoilersButton.interactable = false;
            sirensButton.interactable = false;
            decalsButton.interactable = false;
            neonsButton.interactable = false;

            return;

        }

        if (!HR_MainMenuHandler.Instance.currentCar) {

            paintsButton.interactable = false;
            wheelsButton.interactable = false;
            customizationButton.interactable = false;
            upgradesButton.interactable = false;
            spoilersButton.interactable = false;
            sirensButton.interactable = false;
            decalsButton.interactable = false;
            neonsButton.interactable = false;

            return;

        }

        if (!HR_MainMenuHandler.Instance.currentCar.Customizer) {

            paintsButton.interactable = false;
            wheelsButton.interactable = false;
            customizationButton.interactable = false;
            upgradesButton.interactable = false;
            spoilersButton.interactable = false;
            sirensButton.interactable = false;
            decalsButton.interactable = false;
            neonsButton.interactable = false;

            return;

        }

        paintsButton.interactable = HR_MainMenuHandler.Instance.currentCar.Customizer.PaintManager;
        wheelsButton.interactable = HR_MainMenuHandler.Instance.currentCar.Customizer.WheelManager;
        customizationButton.interactable = HR_MainMenuHandler.Instance.currentCar.Customizer.CustomizationManager;
        upgradesButton.interactable = HR_MainMenuHandler.Instance.currentCar.Customizer.UpgradeManager;
        spoilersButton.interactable = HR_MainMenuHandler.Instance.currentCar.Customizer.SpoilerManager;
        sirensButton.interactable = HR_MainMenuHandler.Instance.currentCar.Customizer.SirenManager;
        decalsButton.interactable = HR_MainMenuHandler.Instance.currentCar.Customizer.DecalManager;
        neonsButton.interactable = HR_MainMenuHandler.Instance.currentCar.Customizer.NeonManager;

        engine.SetValueWithoutNotify(HR_MainMenuHandler.Instance.currentCar.Engine.maximumTorqueAsNM / 800f);

        if (HR_MainMenuHandler.Instance.currentCar.Stability)
            handling.SetValueWithoutNotify(HR_MainMenuHandler.Instance.currentCar.Stability.tractionHelperStrength / .75f);
        else
            handling.SetValueWithoutNotify(0f);

        brake.SetValueWithoutNotify(HR_MainMenuHandler.Instance.currentCar.FrontAxle.maxBrakeTorque / 6000f);
        speed.SetValueWithoutNotify(HR_MainMenuHandler.Instance.currentCar.maximumSpeed / 400f);

    }

}
