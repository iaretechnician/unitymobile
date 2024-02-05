namespace ShmupBoss
{
    /// <summary>
    /// System event arguments to pass the new bomb count to the level UI.
    /// </summary>
    public class BombArgs : System.EventArgs
    {
        public int BombCount;

        public BombArgs(int bombCount)
        {
            BombCount = bombCount;
        }
    }
}