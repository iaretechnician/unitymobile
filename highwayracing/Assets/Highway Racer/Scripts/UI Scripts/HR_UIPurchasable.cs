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
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("BoneCracker Games/Highway Racer/UI/HR UI Purchasable")]
public class HR_UIPurchasable : MonoBehaviour, IPointerClickHandler {

    RCCP_UI_Color color;
    RCCP_UI_Decal decal;
    RCCP_UI_Neon neon;
    RCCP_UI_Siren siren;
    RCCP_UI_Spoiler spoiler;
    RCCP_UI_Wheel wheel;

    private bool canPurchase = true;

    private Button button;
    public bool unlocked = false;

    public int price = 1000;
    public Text priceText;

    public GameObject lockImage;
    public AudioClip unlockAudioclip;

    public void OnPointerClick(PointerEventData eventData) {

        if (!button)
            button = GetComponent<Button>();

        if (!button) {

            Debug.LogError("Button is not found on " + transform.name + ". Disabling it.");
            enabled = false;
            return;

        }

        if (!unlocked) {

            int currentMoney = HR_API.GetCurrency();

            if (currentMoney >= price) {

                unlocked = true;
                HR_API.ConsumeCurrency(price);
                PlayerPrefs.SetInt("Unlocked_" + transform.name, 1);

                if (unlockAudioclip)
                    RCCP_AudioSource.NewAudioSource(gameObject, unlockAudioclip.name, 0f, 0f, 1f, unlockAudioclip, false, true, true);

            } else {

                if (HR_UIInfoDisplayer.Instance)
                    HR_UIInfoDisplayer.Instance.ShowInfo("", (price - HR_API.GetCurrency()).ToString() + " More Money To Purchase This Item!", HR_UIInfoDisplayer.InfoType.NotEnoughMoney);

            }

        }

    }

    private void Awake() {

        if (!button)
            button = GetComponent<Button>();

        color = GetComponent<RCCP_UI_Color>();
        decal = GetComponent<RCCP_UI_Decal>();
        neon = GetComponent<RCCP_UI_Neon>();
        siren = GetComponent<RCCP_UI_Siren>();
        spoiler = GetComponent<RCCP_UI_Spoiler>();
        wheel = GetComponent<RCCP_UI_Wheel>();

    }

    private void OnEnable() {

        if (PlayerPrefs.HasKey("Unlocked_" + transform.name))
            unlocked = true;

    }

    private void Update() {

        if (HR_API.GetCurrency() >= price)
            canPurchase = true;
        else
            canPurchase = false;

        if (unlocked) {

            ToggleComponent(true);

        } else {

            if (canPurchase)
                ToggleComponent(true);
            else
                ToggleComponent(false);

        }

        if (button)
            button.interactable = unlocked;

        if (lockImage)
            lockImage.SetActive(!unlocked);

        if (priceText) {

            if (!unlocked)
                priceText.text = price.ToString("F0");
            else
                priceText.text = "";

        }

    }

    private void ToggleComponent(bool state) {

        if (color)
            color.enabled = state;

        if (decal)
            decal.enabled = state;

        if (neon)
            neon.enabled = state;

        if (siren)
            siren.enabled = state;

        if (spoiler)
            spoiler.enabled = state;

        if (wheel)
            wheel.enabled = state;

    }

    //private void OnValidate() {

    //    foreach (Transform item in GetComponentsInChildren<Transform>()) {

    //        if (item.name == "Price Label")
    //            priceText = item.GetComponent<Text>();

    //        if (item.name == "Locked")
    //            lockImage = item.gameObject;

    //    }

    //}

}
