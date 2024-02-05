﻿//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2023 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Selectable wheels.
/// </summary>
[System.Serializable]
public class HR_Wheels : ScriptableObject {

    private static HR_Wheels instance;
    public static HR_Wheels Instance {

        get {

            if (instance == null)
                instance = Resources.Load("HR_Wheels") as HR_Wheels;

            return instance;

        }

    }

    [System.Serializable]
    public class Wheels {

        public GameObject wheel;
        public bool unlocked;
        public int price;

    }

    public Wheels[] wheels;

}
