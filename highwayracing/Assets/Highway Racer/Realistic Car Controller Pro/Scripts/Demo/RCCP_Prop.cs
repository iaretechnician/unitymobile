//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RCCP_Prop : MonoBehaviour {

    private void OnEnable() {

        Rigidbody rigid = GetComponent<Rigidbody>();

        if (rigid)
            rigid.Sleep();

    }

}
