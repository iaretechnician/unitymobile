//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

/// <summary>
/// Interface for all modular components.
/// </summary>
public interface IRCCP_Component {

    /// <summary>
    /// Initializes and registers the target component.
    /// </summary>
    /// <param name="connectedCarController"></param>
    public void Initialize(RCCP_CarController connectedCarController);

}
