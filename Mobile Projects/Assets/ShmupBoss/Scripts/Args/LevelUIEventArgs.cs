namespace ShmupBoss
{
    /// <summary>
    /// System event arguments that store the type of UI event which just occurred
    /// such as a game over event, level complete, game paused, resume, etc.. 
    /// which determines what type of sound to play in the level audio manager.
    /// </summary>
    public class LevelUIEventArgs : System.EventArgs
    {
        public LevelUIEvent UIEvent;

        public LevelUIEventArgs(LevelUIEvent uiEvent)
        {
            UIEvent = uiEvent;
        }
    }
}