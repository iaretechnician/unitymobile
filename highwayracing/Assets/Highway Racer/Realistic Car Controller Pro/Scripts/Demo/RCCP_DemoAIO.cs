//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright � 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RCCP_DemoAIO : MonoBehaviour {

    public static RCCP_DemoAIO Instance;

    public GameObject content;
    public GameObject loading;
    public GameObject credits;
    public GameObject back;

    public GameObject rtcButton;
    public GameObject[] photonButtons;
    public GameObject[] sharedAssetsButtons;
    public GameObject photonInfo;
    public GameObject sharedAssetsInfo;

    private void Awake() {

        if (Instance == null) {

            Instance = this;
            DontDestroyOnLoad(gameObject);

        } else {

            Destroy(gameObject);
            return;

        }

    }

    void Start() {

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

#if BCG_RTRC

        rtcButton.gameObject.SetActive(true);

#else

        rtcButton.gameObject.SetActive(false);

#endif

#if RCCP_PHOTON

        for (int i = 0; i < photonButtons.Length; i++)
            photonButtons[i].SetActive(true);

        photonInfo.SetActive(false);

#else

        for (int i = 0; i < photonButtons.Length; i++)
            photonButtons[i].SetActive(false);

        photonInfo.SetActive(true);

#endif

#if BCG_ENTEREXIT

        for (int i = 0; i < sharedAssetsButtons.Length; i++)
            sharedAssetsButtons[i].SetActive(true);

        sharedAssetsInfo.SetActive(false);

#else

        for (int i = 0; i < sharedAssetsButtons.Length; i++)
            sharedAssetsButtons[i].SetActive(false);

        sharedAssetsInfo.SetActive(true);

#endif

    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1) {

        loading.SetActive(false);

    }

    public void LoadScene(string sceneIndex) {

        loading.SetActive(true);
        SceneManager.LoadSceneAsync(sceneIndex);

        if (sceneIndex == RCCP_DemoScenes.Instance.path_city_AIO) {

            content.SetActive(true);
            back.SetActive(false);
            credits.SetActive(true);

        } else {

            content.SetActive(false);
            back.SetActive(true);
            credits.SetActive(false);

        }

#if RCCP_PHOTON

        if (sceneIndex == RCCP_DemoScenes_Photon.Instance.path_demo_PUN2Lobby || sceneIndex == RCCP_DemoScenes_Photon.Instance.path_demo_PUN2City)
            back.SetActive(false);

#endif

    }

}
