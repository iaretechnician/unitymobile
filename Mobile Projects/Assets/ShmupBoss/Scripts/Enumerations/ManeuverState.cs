namespace ShmupBoss
{
    /// <summary>
    /// <para>An enum used with the AI mover to define its current
    /// state and move accordingly, this includes: </para>
    /// <br>Starting Move</br>
    /// <br>Waiting</br>
    /// <br>Target Ahead</br>
    /// <br>Setting Maneuver Move</br>
    /// <br>Doing Maneuver Move</br>
    /// </summary>
    public enum ManeuverState
    {
        /// <summary>
        /// The first move an AI mover makes after being spawned which is a set direction.
        /// </summary>
        StartingMove,

        /// <summary>
        /// Indicates that an AI mover is waiting for the player to come within 
        /// (player is not ahead.)
        /// the average collider width.
        /// </summary>
        Waiting,

        /// <summary>
        /// Indicates that the player is currently within the average 
        /// collider width (player is ahead.)
        /// </summary>
        TargetAhead,

        /// <summary>
        /// Indicates that the AI mover is picking from a random direction where to go. 
        /// </summary>
        SettingManeuverMove,

        /// <summary>
        /// Currently moving and proceeding with the maneuver move.
        /// </summary>
        DoingManeuverMove
    }
}