namespace ShmupBoss
{
    /// <summary>
    /// System event arguments which store the boss phase index that a sub 
    /// phase is connected to, this helsp when a sub phase is eliminated 
    /// to know which phase to modify or notify.
    /// </summary>
    public class BossSubPhaseArgs : System.EventArgs
    {
        public int BossPhaseIndex;

        public BossSubPhaseArgs(int bossPhaseIndex)
        {
            BossPhaseIndex = bossPhaseIndex;
        }
    }
}