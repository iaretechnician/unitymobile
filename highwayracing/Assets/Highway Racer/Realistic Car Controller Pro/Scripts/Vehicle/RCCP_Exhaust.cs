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

/// <summary>
/// Exhaust.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller Pro/Misc/RCCP Exhaust")]
public class RCCP_Exhaust : RCCP_Component {

    /// <summary>
    /// Flames on engine cut off.
    /// </summary>
    public bool flameOnCutOff = false;

    /// <summary>
    /// Smoke particles.
    /// </summary>
    private ParticleSystem particle;

    /// <summary>
    /// Smoke emission.
    /// </summary>
    private ParticleSystem.EmissionModule emission;

    /// <summary>
    /// Flame particles.
    /// </summary>
    public ParticleSystem flame;

    /// <summary>
    /// Flame emission.
    /// </summary>
    private ParticleSystem.EmissionModule subEmission;

    /// <summary>
    /// Flame light.
    /// </summary>
    private Light flameLight;

    /// <summary>
    /// Lensflare of the flame light.
    /// </summary>
    private LensFlare lensFlare;

    /// <summary>
    /// Flare brightness.
    /// </summary>
    public float flareBrightness = 1f;

    /// <summary>
    /// Calculated flare brigtness.
    /// </summary>
    private float finalFlareBrightness;

    /// <summary>
    /// Flame time.
    /// </summary>
    public float flameTime = 0f;

    /// <summary>
    /// Flame audio source.
    /// </summary>
    private AudioSource flameSource;

    /// <summary>
    /// Flame color.
    /// </summary>
    public Color flameColor = Color.red;

    /// <summary>
    /// Boost / Nos flame color.
    /// </summary>
    public Color boostFlameColor = Color.blue;

    /// <summary>
    /// Emission limits
    /// </summary>
    public float minEmission = 5f;

    /// <summary>
    /// Emission limits
    /// </summary>
    public float maxEmission = 20f;

    /// <summary>
    /// Size limits.
    /// </summary>
    public float minSize = 1f;

    /// <summary>
    /// Size limits.
    /// </summary>
    public float maxSize = 4f;

    /// <summary>
    /// Speed limits.
    /// </summary>
    public float minSpeed = .1f;

    /// <summary>
    /// Speed limits.
    /// </summary>
    public float maxSpeed = 1f;

    public override void Start() {

        base.Start();

        // Getting components.
        particle = GetComponent<ParticleSystem>();

        if (!particle) {

            Debug.LogError("ParticleSystem couldn't found on this exhaust named " + transform.name + ", disabling this gameobject!");
            enabled = false;
            return;

        }

        emission = particle.emission;

        //  If flame exists...
        if (flame) {

            //  Getting emission of the flame, light, and creating audio source.
            subEmission = flame.emission;
            flameLight = flame.GetComponentInChildren<Light>();
            flameSource = RCCP_AudioSource.NewAudioSource(RCCP_Settings.Instance.audioMixer, gameObject, "Exhaust Flame AudioSource", 10f, 25f, .5f, RCCP_Settings.Instance.exhaustFlameClips[0], false, false, false);

            //  If flame light exists, set render mode of the light depending of the option in RCCP Settings.
            if (flameLight)
                flameLight.renderMode = LightRenderMode.ForceVertex;

        }

        //  Getting lensflare.
        lensFlare = GetComponentInChildren<LensFlare>();

        if (flameLight) {

            if (flameLight.flare != null)
                flameLight.flare = null;

        }

    }

    private void Update() {

        //  If no car controller found, or particle, return.
        if (!particle)
            return;

        if (!CarController.Engine) {

            if (emission.enabled)
                emission.enabled = false;

            return;

        }

        Smoke();
        Flame();

        if (lensFlare)
            LensFlare();

    }

    /// <summary>
    /// Smoke particles.
    /// </summary>
    private void Smoke() {

        //  If engine is running, set speed, size, and emission rates of the smoke particles.
        if (CarController.Engine.engineRunning) {

            var main = particle.main;

            if (Mathf.Abs(CarController.speed) > 20) {

                if (emission.enabled)
                    emission.enabled = false;

                return;

            }

            if (!emission.enabled)
                emission.enabled = true;

            emission.rateOverTime = Mathf.Clamp(maxEmission * CarController.throttleInput_V, minEmission, maxEmission);
            main.startSpeed = Mathf.Clamp(maxSpeed * CarController.throttleInput_V, minSpeed, maxSpeed);
            main.startSize = Mathf.Clamp(maxSize * CarController.throttleInput_V, minSize, maxSize);

        } else {

            if (emission.enabled)
                emission.enabled = false;

        }

    }

    /// <summary>
    /// Flame particles with light effects.
    /// </summary>
    private void Flame() {

        //  If engine is running, set color of the flame, create audio source.
        if (CarController.Engine.engineRunning) {

            var main = flame.main;

            if (CarController.throttleInput_V >= .25f)
                flameTime = 0f;

            if ((flameOnCutOff && (CarController.engineRPM >= 5000 && CarController.engineRPM <= 5500 && CarController.throttleInput_V <= .25f && flameTime <= .5f)) || CarController.nosInput_V >= .75f) {

                flameTime += Time.deltaTime;
                subEmission.enabled = true;

                if (flameLight)
                    flameLight.intensity = 3f * UnityEngine.Random.Range(.25f, 1f);

                if (CarController.nosInput_V >= .75f && flame) {

                    main.startColor = boostFlameColor;

                    if (flameLight)
                        flameLight.color = main.startColor.color;

                } else {

                    main.startColor = flameColor;

                    if (flameLight)
                        flameLight.color = main.startColor.color;

                }

                if (flameSource && !flameSource.isPlaying) {

                    flameSource.clip = RCCP_Settings.Instance.exhaustFlameClips[UnityEngine.Random.Range(0, RCCP_Settings.Instance.exhaustFlameClips.Length)];
                    flameSource.Play();

                }

            } else {

                subEmission.enabled = false;

                if (flameLight)
                    flameLight.intensity = 0f;

                if (flameSource && flameSource.isPlaying)
                    flameSource.Stop();

            }

        } else {

            if (emission.enabled)
                emission.enabled = false;

            subEmission.enabled = false;

            if (flameLight)
                flameLight.intensity = 0f;

            if (flameSource && flameSource.isPlaying)
                flameSource.Stop();

        }

    }

    /// <summary>
    /// Lensflare intensity calculation.
    /// </summary>
    private void LensFlare() {

        //  If there is no camera, return.
        if (!Camera.main)
            return;

        float distanceTocam = Vector3.Distance(transform.position, Camera.main.transform.position);
        float angle = Vector3.Angle(transform.forward, Camera.main.transform.position - transform.position);

        if (angle != 0)
            finalFlareBrightness = flareBrightness * (4 / distanceTocam) * ((100f - (1.11f * angle)) / 100f) / 2f;

        if (flameLight) {

            lensFlare.brightness = finalFlareBrightness * flameLight.intensity;
            lensFlare.color = flameLight.color;

        }

    }

}
