namespace ShmupBoss
{
    /// <summary>
    /// default rotation angles of agents.<br></br> 
    /// The default facing directions for the player and enemies are translated into angles in float
    /// values by the Math2D class so that they can be used in methods which use float values angles.
    /// </summary>
    public static class FacingAngles
    {
        /// <summary>
        /// The default facing angle of the player, returns 90 in a vertical level and 0 in a horizontal.
        /// </summary>
        public static float Player
        {
            get
            {
                return Math2D.VectorToDegree(FacingDirections.Player);
            }
        }

        /// <summary>
        /// The default facing angle of enemies, returns 270 in a vertical level and 180 in a horizontal.
        /// </summary>
        public static float NonPlayer
        {
            get
            {
                return Math2D.VectorToDegree(FacingDirections.NonPlayer);
            }
        }
    }
}