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

[AddComponentMenu("BoneCracker Games/Highway Racer/UI/HR UI Purchasable For Upgrades")]
[RequireComponent(typeof(RCCP_UI_Upgrade))]
public class HR_UIPurchasable_ForUpgrades : MonoBehaviour, IPointerClickHandler {

    private RCCP_UI_Upgrade upgraderButton;

    private bool canUpgrade = true;
    private bool canPurchase = true;
    public int maxLevel = 5;

    public int price = 1000;

    public Text priceText;
    public Text levelText;

    public AudioClip unlockAudioclip;

    public void OnPointerClick(PointerEventData eventData) {

        if (!upgraderButton)
            return;

        if (!levelText)
            return;

        int.TryParse(levelText.text, out int level);

        if (level > maxLevel)
            canUpgrade = false;

        if (canUpgrade) {

            int currentMoney = HR_API.GetCurrency();

            if (currentMoney >= price) {

                HR_API.ConsumeCurrency(price);

                if (unlockAudioclip)
                    RCCP_AudioSource.NewAudioSource(gameObject, unlockAudioclip.name, 0f, 0f, 1f, unlockAudioclip, false, true, true);

            } else {

                if (HR_UIInfoDisplayer.Instance)
                    HR_UIInfoDisplayer.Instance.ShowInfo("", (price - HR_API.GetCurrency()).ToString("F0") + " More Money Needed To Purchase This Item!", HR_UIInfoDisplayer.InfoType.NotEnoughMoney);

            }

        }

    }

    private void Awake() {

        upgraderButton = GetComponent<RCCP_UI_Upgrade>();

    }

    private void OnEnable() {

        upgraderButton.Check();

        int.TryParse(levelText.text, out int level);

        if (level >= maxLevel)
            canUpgrade = false;
        else
            canUpgrade = true;

    }

    private void Update() {

        if (HR_API.GetCurrency() >= price)
            canPurchase = true;
        else
            canPurchase = false;

        if (upgraderButton) {

            if (canPurchase && canUpgrade)
                upgraderButton.enabled = true;
            else
                upgraderButton.enabled = false;

        }

        if (priceText) {

            if (canUpgrade)
                priceText.text = price.ToString("F0");
            else
                priceText.text = "";

        }

        int.TryParse(levelText.text, out int level);

        if (level >= maxLevel)
            canUpgrade = false;

    }

    //private void OnValidate() {

    //    foreach (Transform item in GetComponentsInChildren<Transform>()) {

    //        if (item.name == "Price Label")
    //            priceText = item.GetComponent<Text>();

    //    }

    //}

}
