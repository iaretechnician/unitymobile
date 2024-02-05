namespace ShmupBoss
{
    /// <summary>
    /// System Event arguments which store the current coins gained in the level.<br></br>
    /// This is used for events which are related to any coin change such as the elimination 
    /// of any enemies or the piclup of drops.
    /// </summary>
    public class CoinsArgs : System.EventArgs
    {
        public int CurrentCoins;

        public CoinsArgs(int currentCoins)
        {
            CurrentCoins = currentCoins;
        }
    }
}