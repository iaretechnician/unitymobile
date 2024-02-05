namespace ShmupBoss
{
    /// <summary>
    /// System Event arguments which store the current score gained in the level.<br></br> 
    /// This is used for events which are related to any score change such as the elimination 
    /// of any enemies or the piclup of drops.
    /// </summary>
    public class ScoreArgs : System.EventArgs
    {
        public int CurrentScore;

        public ScoreArgs(int currentScore)
        {
            CurrentScore = currentScore;
        }
    }
}