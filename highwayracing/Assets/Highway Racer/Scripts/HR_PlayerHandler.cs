﻿//----------------------------------------------
//           	   Highway Racer
//
// Copyright © 2014 - 2023 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if PHOTON_UNITY_NETWORKING
using Photon;
using Photon.Pun;
#endif

/// <summary>
/// Player manager that containts current score, near misses.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RCCP_CarController))]
[AddComponentMenu("BoneCracker Games/Highway Racer/Player/HR Player Handler")]
public class HR_PlayerHandler : MonoBehaviour {

    private RCCP_CarController carController;
    public RCCP_CarController CarController {

        get {

            if (!carController)
                carController = GetComponent<RCCP_CarController>();

            return carController;

        }

    }

    private Rigidbody rigid;
    public Rigidbody Rigid {

        get {

            if (!rigid)
                rigid = GetComponent<Rigidbody>();

            return rigid;

        }

    }

    public bool canCrash = true;

    public float damage = 0f;       //  Current damage.
    private bool crashed = false;      //	Game is over now?

    internal float score;       //	Current score
    internal float timeLeft = 100f;     //	Time left.
    internal int combo;     //	Current near miss combo.
    internal int maxCombo;      //	Highest combo count.
    internal float distanceToNextPlayer = -9999f;     //	Time left.

    internal float speed = 0f;      //  Current speed.
    internal float distance = 0f;       //  Total distance traveled.
    internal float highSpeedCurrent = 0f;       //  Current high speed time.
    internal float highSpeedTotal = 0f;     //  Total high speed time.
    internal float opposideDirectionCurrent = 0f;       //  Current opposite direction time.
    internal float opposideDirectionTotal = 0f;     //  Total opposite direction time.
    internal int nearMisses;        //  Total near misses.
    private float comboTime;        //  Combo time for near misses.
    private Vector3 previousPosition;       //  Previous position used to calculate total traveled distance.

    private int MinimumSpeedToScore {
        get {
            return HR_HighwayRacerProperties.Instance._minimumSpeedForGainScore;
        }
    }
    private int MinimumSpeedToHighSpeed {
        get {
            return HR_HighwayRacerProperties.Instance._minimumSpeedForHighSpeed;
        }
    }

    public int TotalDistanceMoneyMP {
        get {
            return HR_HighwayRacerProperties.Instance._totalDistanceMoneyMP;
        }
    }
    public int TotalNearMissMoneyMP {
        get {
            return HR_HighwayRacerProperties.Instance._totalNearMissMoneyMP;
        }
    }
    public int TotalOverspeedMoneyMP {
        get {
            return HR_HighwayRacerProperties.Instance._totalOverspeedMoneyMP;
        }
    }
    public int TotalOppositeDirectionMP {
        get {
            return HR_HighwayRacerProperties.Instance._totalOppositeDirectionMP;
        }
    }

    private string currentTrafficCarNameLeft = null;
    private string currentTrafficCarNameRight = null;

    internal bool bombTriggered = false;
    internal float bombHealth = 100f;

    private AudioSource hornSource;

    public delegate void onNearMiss(HR_PlayerHandler player, int score, HR_UIDynamicScoreDisplayer.Side side);
    public static event onNearMiss OnNearMiss;

#if PHOTON_UNITY_NETWORKING
    private PhotonView photonView;
#endif

    private void OnEnable() {

#if PHOTON_UNITY_NETWORKING && BCG_HR_PHOTON

        photonView = GetComponent<PhotonView>();

        if (photonView && !photonView.IsMine)
            return;

#endif

        //	Creating horn audio source.
        hornSource = HR_CreateAudioSource.NewAudioSource(gameObject, "Horn", 10f, 100f, 1f, HR_HighwayRacerProperties.Instance.hornClip, true, false, false);

        CheckGroundGap();

        WheelCollider[] wheelColliders = GetComponentsInChildren<WheelCollider>(true);

        foreach (WheelCollider item in wheelColliders)
            item.forceAppPointDistance = .15f;

    }

    private void Update() {

#if PHOTON_UNITY_NETWORKING && BCG_HR_PHOTON

        if (photonView && !photonView.IsMine)
            return;

#endif

        //	If scene doesn't include gameplay manager, return.
        if (!HR_GamePlayHandler.Instance)
            return;

        //	If game is not started yet, return.
        if (crashed || !HR_GamePlayHandler.Instance.gameStarted)
            return;

        //	Speed of the car.
        speed = CarController.speed;

        // Total distance traveled.
        distance += Vector3.Distance(previousPosition, transform.position) / 1000f;
        previousPosition = transform.position;

        //	Is speed is high enough, gain score.
        if (speed >= MinimumSpeedToScore)
            score += CarController.speed * (Time.deltaTime * .05f);

        //	If speed is higher than high speed, gain score.
        if (speed >= MinimumSpeedToHighSpeed) {

            highSpeedCurrent += Time.deltaTime;
            highSpeedTotal += Time.deltaTime;

        } else {

            highSpeedCurrent = 0f;

        }

        // If car is at opposite direction, gain score.
        if (speed >= (MinimumSpeedToHighSpeed / 2f) && transform.position.x <= 0f && HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.TwoWay) {

            opposideDirectionCurrent += Time.deltaTime;
            opposideDirectionTotal += Time.deltaTime;

        } else {

            opposideDirectionCurrent = 0f;

        }

        //	If mode is time attack, reduce the timer.
        if (HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.TimeAttack) {

            timeLeft -= Time.deltaTime;

            // If timer hits 0, game over.
            if (timeLeft < 0) {

                timeLeft = 0;
                GameOver();

            }

        }

        comboTime += Time.deltaTime;

        //	If game mode is bomb...
        if (HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.Bomb) {

            //	Bomb will be triggered below 80 km/h.
            if (speed > 80f) {

                if (!bombTriggered)
                    bombTriggered = true;
                else
                    bombHealth += Time.deltaTime * 5f;

            } else if (bombTriggered) {

                bombHealth -= Time.deltaTime * 10f;

            }

            bombHealth = Mathf.Clamp(bombHealth, 0f, 100f);

            //	If bomb health hits 0, blow and game over.
            if (bombHealth <= 0f) {

                GameObject explosion = Instantiate(HR_HighwayRacerProperties.Instance.explosionEffect, transform.position, transform.rotation);
                explosion.transform.SetParent(null);
                Rigid.AddForce(Vector3.up * 10000f, ForceMode.Impulse);
                Rigid.AddTorque(Vector3.up * 10000f, ForceMode.Impulse);
                Rigid.AddTorque(Vector3.forward * 10000f, ForceMode.Impulse);

                if (GetComponentInChildren<HR_Bomb>())
                    Destroy(GetComponentInChildren<HR_Bomb>().gameObject);

                GameOver();

            }

        }

        if (comboTime >= 2)
            combo = 0;

        CheckStatus();

    }

    private void FixedUpdate() {

#if PHOTON_UNITY_NETWORKING && BCG_HR_PHOTON

        if (photonView && !photonView.IsMine)
            return;

#endif

        //	If scene doesn't include gameplay manager, return.
        if (!HR_GamePlayHandler.Instance)
            return;

        //	If game is started, check near misses with raycasts.
        if (!crashed && HR_GamePlayHandler.Instance.gameStarted)
            CheckNearMiss();

    }

    /// <summary>
    /// Checks near vehicles by drawing raycasts to the left and right sides.
    /// </summary>
    private void CheckNearMiss() {

        RaycastHit hit;

        Debug.DrawRay(CarController.AeroDynamics.COM.position, (-transform.right * 2f), Color.white);
        Debug.DrawRay(CarController.AeroDynamics.COM.position, (transform.right * 2f), Color.white);

        // Raycasting to the left side.
        if (Physics.Raycast(CarController.AeroDynamics.COM.position, (-transform.right), out hit, 2f, HR_HighwayRacerProperties.Instance.trafficCarsLayer) && !hit.collider.isTrigger) {

            //	If hits, get it's name.
            currentTrafficCarNameLeft = hit.transform.name;

        } else {

            if (currentTrafficCarNameLeft != null && speed > HR_HighwayRacerProperties.Instance._minimumSpeedForGainScore) {

                nearMisses++;
                combo++;
                comboTime = 0;

                if (maxCombo <= combo)
                    maxCombo = combo;

                score += 100f * Mathf.Clamp(combo / 1.5f, 1f, 20f);
                OnNearMiss(this, (int)(100f * Mathf.Clamp(combo / 1.5f, 1f, 20f)), HR_UIDynamicScoreDisplayer.Side.Left);

                currentTrafficCarNameLeft = null;

            } else {

                currentTrafficCarNameLeft = null;

            }

        }

        // Raycasting to the right side.
        if (Physics.Raycast(CarController.AeroDynamics.COM.position, (transform.right), out hit, 2f, HR_HighwayRacerProperties.Instance.trafficCarsLayer) && !hit.collider.isTrigger) {

            //	If hits, get it's name.
            currentTrafficCarNameRight = hit.transform.name;

        } else {

            if (currentTrafficCarNameRight != null && speed > HR_HighwayRacerProperties.Instance._minimumSpeedForGainScore) {

                nearMisses++;
                combo++;
                comboTime = 0;

                if (maxCombo <= combo)
                    maxCombo = combo;

                score += 100f * Mathf.Clamp(combo / 1.5f, 1f, 20f);
                OnNearMiss(this, (int)(100f * Mathf.Clamp(combo / 1.5f, 1f, 20f)), HR_UIDynamicScoreDisplayer.Side.Right);

                currentTrafficCarNameRight = null;

            } else {

                currentTrafficCarNameRight = null;

            }

        }

        // Raycasting to the front side. Used for taking down the lane.
        if (Physics.Raycast(CarController.AeroDynamics.COM.position, (transform.forward), out hit, 40f, HR_HighwayRacerProperties.Instance.trafficCarsLayer) && !hit.collider.isTrigger) {

            Debug.DrawRay(CarController.AeroDynamics.COM.position, (transform.forward * 20f), Color.red);

            if (CarController.Lights.highBeamHeadlights)
                hit.transform.SendMessage("ChangeLines");

        }

        // Horn and siren.
        if (hornSource) {

            hornSource.volume = Mathf.Lerp(hornSource.volume, CarController.Lights.highBeamHeadlights ? 1f : 0f, Time.deltaTime * 25f);

            if (CarController.Lights.highBeamHeadlights) {

                RCCP_VehicleUpgrade_Siren upgradeSiren = GetComponentInChildren<RCCP_VehicleUpgrade_Siren>();

                if (upgradeSiren && upgradeSiren.isActiveAndEnabled)
                    hornSource.clip = HR_HighwayRacerProperties.Instance.sirenAudioClip;

                if (!hornSource.isPlaying)
                    hornSource.Play();

            } else {

                hornSource.Stop();

            }

        }

    }

    private void OnCollisionEnter(Collision col) {

#if PHOTON_UNITY_NETWORKING && BCG_HR_PHOTON

        if (photonView && !photonView.IsMine)
            return;

#endif

        //	If scene doesn't include gameplay manager, return.
        if (!HR_GamePlayHandler.Instance)
            return;

        if (!canCrash)
            return;

        if (crashed)
            return;

        //	Calculating collision impulse.
        float impulse = col.impulse.magnitude / 1000f;

        //	If impulse is below the limit, return.
        if (impulse < HR_HighwayRacerProperties.Instance._minimumCollisionForGameOver)
            return;

        // If hit is not a traffic car, return.
        if ((1 << col.gameObject.layer) != HR_HighwayRacerProperties.Instance.trafficCarsLayer.value)
            return;

        //  Increasinf damage.
        damage += impulse * 2f;

        // Resetting combo to 0.
        combo = 0;

        // If mode is bomb mode, reduce the bomb health.
        if (HR_GamePlayHandler.Instance.mode == HR_GamePlayHandler.Mode.Bomb) {

            bombHealth -= impulse * 3f;
            return;

        }

        if (damage >= 100f) {

            //	Game over.
            GameOver();

        }

    }

    /// <summary>
    /// Checks position of the car. If exceeds limits, respawns it.
    /// </summary>
    private void CheckStatus() {

        if (Rigid.isKinematic)
            return;

        if (!HR_GamePlayHandler.Instance.gameStarted)
            return;

        //	If speed is below 5, or X position of the car exceeds limits, respawn it.
        if (speed <= 15f || Mathf.Abs(transform.position.x) > 10f || Mathf.Abs(transform.position.y) > 10f) {

            transform.position = new Vector3(0f, 2.5f, transform.position.z + 15f);
            transform.rotation = Quaternion.identity;
            Rigid.angularVelocity = Vector3.zero;
            Rigid.velocity = new Vector3(0f, 0f, 12f);

        }

    }

    /// <summary>
    /// Game Over.
    /// </summary>
    public void GameOver() {

        crashed = true;
        CarController.canControl = false;
        CarController.engineRunning = false;
        CarController.Lights.indicatorsAll = true;
        Rigid.drag = 1f;

        int[] scores = new int[4];
        scores[0] = Mathf.FloorToInt(distance * TotalDistanceMoneyMP);
        scores[1] = Mathf.FloorToInt(nearMisses * TotalNearMissMoneyMP);
        scores[2] = Mathf.FloorToInt(highSpeedTotal * TotalOverspeedMoneyMP);
        scores[3] = Mathf.FloorToInt(opposideDirectionTotal * TotalOppositeDirectionMP);

        for (int i = 0; i < scores.Length; i++)
            HR_API.AddCurrency(scores[i]);

        HR_GamePlayHandler.Instance.CrashedPlayer(this, scores);

    }

    /// <summary>
    /// Eliminates ground gap distance on when spawned.
    /// </summary>
    private void CheckGroundGap() {

        WheelCollider wheel = GetComponentInChildren<WheelCollider>();
        float distancePivotBetweenWheel = Vector3.Distance(new Vector3(0f, transform.position.y, 0f), new Vector3(0f, wheel.transform.position.y, 0f));

        RaycastHit hit;

        if (Physics.Raycast(wheel.transform.position, -Vector3.up, out hit, 10f))
            transform.position = new Vector3(transform.position.x, hit.point.y + distancePivotBetweenWheel + (wheel.radius / 1f) + (wheel.suspensionDistance / 2f), transform.position.z);

    }

}
