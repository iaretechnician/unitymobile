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
/// Other addons belongs to the vehicle, such as nos, dashboard, interior, cameras, exhaust, AI, recorder, attacher, etc...
/// </summary>
public class RCCP_OtherAddons : RCCP_Component {

    /// <summary>
    /// NOS.
    /// </summary>
    public RCCP_Nos Nos {

        get {

            if (_nos == null)
                _nos = GetComponentInChildren<RCCP_Nos>(true);

            return _nos;

        }
        set {

            _nos = value;

        }

    }

    /// <summary>
    /// Visual dashboard.
    /// </summary>
    public RCCP_Visual_Dashboard Dashboard {

        get {

            if (_dashboard == null)
                _dashboard = GetComponentInChildren<RCCP_Visual_Dashboard>(true);

            return _dashboard;

        }
        set {

            _dashboard = value;

        }

    }

    /// <summary>
    /// Hood and wheel cameras.
    /// </summary>
    public RCCP_Exterior_Cameras ExteriorCameras {

        get {

            if (_exteriorCameras == null)
                _exteriorCameras = GetComponentInChildren<RCCP_Exterior_Cameras>(true);

            return _exteriorCameras;

        }
        set {

            _exteriorCameras = value;

        }

    }

    /// <summary>
    /// Exhausts.
    /// </summary>
    public RCCP_Exhausts Exhausts {

        get {

            if (_exhausts == null)
                _exhausts = GetComponentInChildren<RCCP_Exhausts>(true);

            return _exhausts;

        }
        set {

            _exhausts = value;

        }

    }

    /// <summary>
    /// AI.
    /// </summary>
    public RCCP_AI AI {

        get {

            if (_AI == null)
                _AI = GetComponentInChildren<RCCP_AI>(true);

            return _AI;

        }
        set {

            _AI = value;

        }

    }

    /// <summary>
    /// Recorder.
    /// </summary>
    public RCCP_Recorder Recorder {

        get {

            if (_recorder == null)
                _recorder = GetComponentInChildren<RCCP_Recorder>(true);

            return _recorder;

        }
        set {

            _recorder = value;

        }

    }

    /// <summary>
    /// Trail attacher.
    /// </summary>
    public RCCP_TrailerAttacher TrailAttacher {

        get {

            if (_trailerAttacher == null)
                _trailerAttacher = GetComponentInChildren<RCCP_TrailerAttacher>(true);

            return _trailerAttacher;

        }
        set {

            _trailerAttacher = value;

        }

    }

    /// <summary>
    /// Limiter.
    /// </summary>
    public RCCP_Limiter Limiter {

        get {

            if (_limiter == null)
                _limiter = GetComponentInChildren<RCCP_Limiter>(true);

            return _limiter;

        }
        set {

            _limiter = value;

        }

    }

    /// <summary>
    /// Wheel Blur.
    /// </summary>
    public RCCP_WheelBlur WheelBlur {

        get {

            if (_wheelBlur == null)
                _wheelBlur = GetComponentInChildren<RCCP_WheelBlur>(true);

            return _wheelBlur;

        }
        set {

            _wheelBlur = value;

        }

    }

    /// <summary>
    /// Fuel Tank.
    /// </summary>
    public RCCP_FuelTank FuelTank {

        get {

            if (_fuelTank == null)
                _fuelTank = GetComponentInChildren<RCCP_FuelTank>(true);

            return _fuelTank;

        }
        set {

            _fuelTank = value;

        }

    }

    private RCCP_Nos _nos;
    private RCCP_Visual_Dashboard _dashboard;
    private RCCP_Exterior_Cameras _exteriorCameras;
    private RCCP_Exhausts _exhausts;
    private RCCP_AI _AI;
    private RCCP_Recorder _recorder;
    private RCCP_TrailerAttacher _trailerAttacher;
    private RCCP_Limiter _limiter;
    private RCCP_WheelBlur _wheelBlur;
    private RCCP_FuelTank _fuelTank;

}
