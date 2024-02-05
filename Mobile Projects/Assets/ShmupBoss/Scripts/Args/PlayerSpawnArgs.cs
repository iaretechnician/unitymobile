namespace ShmupBoss
{
    /// <summary>
    /// System event arguments which are related to the data needed when spawning the 
    /// player, this includes the player component and the current player lives, player 
    /// lives are typically used to update the level UI.
    /// </summary>
    public class PlayerSpawnArgs : System.EventArgs
    {
        public Player PlayerComponent;

        public int PlayerLives;

        public PlayerSpawnArgs(Player playerComponent, int playerLives)
        {
            PlayerComponent = playerComponent;
            PlayerLives = playerLives;
        }
    }
}