//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RCCP_CreditsText : MonoBehaviour {

    private TextMeshProUGUI text;

    private void OnEnable() {

        if (!text)
            text = GetComponent<TextMeshProUGUI>();

        if (!text)
            return;

        string creditsText = "Realistic Car Controller Pro " + RCCP_Version.version + "\nMade by BoneCracker Games\nBugra Ozdoganlar";

        text.text = creditsText;

    }

}
