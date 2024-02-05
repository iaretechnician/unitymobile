namespace ShmupBoss
{
    /// <summary>
    /// <para>The tracker can be in any of the following conditions:</para>
    /// <br>StartedTracking</br>
    /// <br>TargetLost</br>
    /// <br>TargetFound</br>
    /// <br>TargetReached</br>
    /// <br>TargetDetected</br>
    /// <br>TargetOutOfRange</br>
    /// </summary>
    public enum TrackingState
    {
        StartedTracking,
        TargetLost,
        TargetFound,
        TargetReached,
        TargetDetected,
        TargetOutOfRange
    }
}