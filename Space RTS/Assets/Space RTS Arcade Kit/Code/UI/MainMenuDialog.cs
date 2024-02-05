﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles keyboard and mouse input for main menu dialogs
/// </summary>
public class MainMenuDialog : MonoBehaviour {

    public MainMenuController menuCtrl;

	void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnYesClicked();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnNoClicked();
        }
    }

    public void OnYesClicked()
    {
        menuCtrl.OnYesClicked();
    }

    public void OnNoClicked()
    {
        menuCtrl.OnNoClicked();
    }
}
