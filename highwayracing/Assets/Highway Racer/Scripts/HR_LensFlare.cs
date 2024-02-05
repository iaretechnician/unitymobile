using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HR_LensFlare : MonoBehaviour {

    private Light lightSource;
    private LensFlareComponentSRP lensFlare;

    public float flareBrightness = 1.5f;

    void Start() {

        lightSource = GetComponent<Light>();
        lensFlare = GetComponent<LensFlareComponentSRP>();

    }

    void Update() {

        if (!lightSource || !lensFlare)
            return;

        lensFlare.intensity = lightSource.intensity * flareBrightness;

    }

}
