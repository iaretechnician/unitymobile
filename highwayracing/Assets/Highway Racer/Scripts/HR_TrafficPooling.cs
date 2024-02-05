﻿//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2023 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if PHOTON_UNITY_NETWORKING
using Photon;
using Photon.Pun;
#endif

[AddComponentMenu("BoneCracker Games/Highway Racer/Traffic/HR Traffic Pooling")]
public class HR_TrafficPooling : MonoBehaviour {

    #region SINGLETON PATTERN
    private static HR_TrafficPooling instance;
    public static HR_TrafficPooling Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<HR_TrafficPooling>();
            return instance;
        }
    }
    #endregion

    public TrafficCars[] trafficCars;       //  Traffic cars.
    public Transform[] lines;       // Traffic lines.

    [System.Serializable]
    public class TrafficCars {

        public GameObject trafficCar;
        public int frequence = 1;

    }

    private List<HR_TrafficCar> _trafficCars = new List<HR_TrafficCar>();       //  Spawned traffic cars.
    internal GameObject container;      //  Container of the spawned traffic cars.

    private void Start() {

#if PHOTON_UNITY_NETWORKING && BCG_HR_PHOTON

        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            return;

#endif

        CreateTraffic();

    }

    private void Update() {

#if PHOTON_UNITY_NETWORKING && BCG_HR_PHOTON

        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            return;

#endif

        if (HR_GamePlayHandler.Instance.gameStarted)
            AnimateTraffic();

    }

    /// <summary>
    /// Spawns all traffic cars.
    /// </summary>
    private void CreateTraffic() {

        //  Creating container for the spawned traffic cars.
        container = new GameObject("Traffic Container");

        for (int i = 0; i < trafficCars.Length; i++) {

            for (int k = 0; k < trafficCars[i].frequence; k++) {

                GameObject go;

                if (PlayerPrefs.GetInt("Multiplayer", 0) == 0) {

                    go = Instantiate(trafficCars[i].trafficCar, Vector3.zero, Quaternion.identity);

                } else {

#if PHOTON_UNITY_NETWORKING && BCG_HR_PHOTON
                 go = PhotonNetwork.Instantiate("Photon Traffic Vehicles/" + trafficCars[i].trafficCar.name, Vector3.zero, Quaternion.identity);
#else
                    go = Instantiate(trafficCars[i].trafficCar, Vector3.zero, Quaternion.identity);
#endif

                }

                _trafficCars.Add(go.GetComponent<HR_TrafficCar>());
                go.SetActive(false);
                go.transform.SetParent(container.transform, true);

            }

        }

    }

    /// <summary>
    /// Animates the traffic cars.
    /// </summary>
    private void AnimateTraffic() {

        //  If there is no camera, return.
        if (!Camera.main)
            return;

        //  If traffic car is below the camera or too far away, realign.
        for (int i = 0; i < _trafficCars.Count; i++) {

            if (Camera.main.transform.position.z > (_trafficCars[i].transform.position.z + 100) || Camera.main.transform.position.z < (_trafficCars[i].transform.position.z - 300))
                ReAlignTraffic(_trafficCars[i]);

        }

    }

    /// <summary>
    /// Realigns the traffic car.
    /// </summary>
    /// <param name="realignableObject"></param>
    private void ReAlignTraffic(HR_TrafficCar realignableObject) {

        if (!realignableObject.gameObject.activeSelf) {

            realignableObject.gameObject.SetActive(true);

#if PHOTON_UNITY_NETWORKING && BCG_HR_PHOTON

            if (HR_PhotonHandler.Instance)
                HR_PhotonHandler.Instance.EnableTrafficvehicle(realignableObject.gameObject);

#endif

        }

        int randomLine = Random.Range(0, lines.Length);

        realignableObject.currentLine = randomLine;
        realignableObject.transform.position = new Vector3(lines[randomLine].position.x, lines[randomLine].position.y, (Camera.main.transform.position.z + (Random.Range(100, 300))));

        switch (HR_GamePlayHandler.Instance.mode) {

            case (HR_GamePlayHandler.Mode.OneWay):
                realignableObject.transform.rotation = Quaternion.identity;
                break;
            case (HR_GamePlayHandler.Mode.TwoWay):
                if (realignableObject.transform.position.x <= 0f)
                    realignableObject.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 180f, 0f);
                else
                    realignableObject.transform.rotation = Quaternion.identity;
                break;
            case (HR_GamePlayHandler.Mode.TimeAttack):
                realignableObject.transform.rotation = Quaternion.identity;
                break;
            case (HR_GamePlayHandler.Mode.Bomb):
                realignableObject.transform.rotation = Quaternion.identity;
                break;

        }

        realignableObject.OnReAligned();

        if (CheckIfClipping(realignableObject.triggerCollider)) {

            realignableObject.gameObject.SetActive(false);

#if PHOTON_UNITY_NETWORKING && BCG_HR_PHOTON

            if (HR_PhotonHandler.Instance)
                HR_PhotonHandler.Instance.DisableTrafficvehicle(realignableObject.gameObject);

#endif

        }

    }

    /// <summary>
    /// Checks if the new aligned car is clipping with another traffic car.
    /// </summary>
    /// <param name="trafficCarBound"></param>
    /// <returns></returns>
    private bool CheckIfClipping(BoxCollider trafficCarBound) {

        for (int i = 0; i < _trafficCars.Count; i++) {

            if (!trafficCarBound.transform.IsChildOf(_trafficCars[i].transform) && _trafficCars[i].gameObject.activeSelf) {

                if (HR_BoundsExtension.ContainBounds(trafficCarBound.transform, trafficCarBound.bounds, _trafficCars[i].triggerCollider.bounds))
                    return true;

            }

        }

        return false;

    }

}
