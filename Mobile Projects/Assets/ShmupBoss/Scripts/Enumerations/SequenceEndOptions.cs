namespace ShmupBoss
{
    /// <summary>
    /// <para>What happens to an image sequence after it has finished its cycle: </para>
    /// <br>Loop Sequence</br>
    /// <br>Loop Final Image</br>
    /// <br>Stop At Final Image</br>
    /// <br>Finish Sequence</br>
    /// </summary>
    public enum SequenceEndOptions
    {
        LoopSequence,
        LoopFinalImage,
        StopAtFinalImage,
        FinishSequence
    }
}
