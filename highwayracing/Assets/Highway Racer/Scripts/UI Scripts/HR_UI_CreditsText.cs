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

[AddComponentMenu("BoneCracker Games/Highway Racer/UI/HR UI Credits Text")]
public class HR_UI_CreditsText : MonoBehaviour {

    private Text text;

    private void OnEnable() {

        text = GetComponent<Text>();

        text.text = "Highway Racer " + HR_Version.version + "\n2014 - 2023 BoneCracker Games";

    }

}
