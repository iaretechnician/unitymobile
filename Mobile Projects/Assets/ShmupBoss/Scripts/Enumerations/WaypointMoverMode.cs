namespace ShmupBoss
{
    /// <summary>
    /// <para>Options for what happens when a waypoint mover finishes going through all the points :</para>
    /// <br>Stop</br>
    /// <br>Continue Loop</br>
    /// <br>Loop Back And Forth</br>
    /// </summary>
    public enum WaypointMoverMode
    {
        Stop,
        ContinueLoop,
        LoopBackAndForth
    }
}