namespace ShmupBoss
{
    /// <summary>
    /// System event arguments which store the current level index, this is typically 
    /// used when needing to show the level number in an infinite spawner level.
    /// </summary>
    public class LevelArgs : System.EventArgs
    {
        public int LevelIndex;

        public LevelArgs(int levelIndex)
        {
            LevelIndex = levelIndex;
        }
    }
}