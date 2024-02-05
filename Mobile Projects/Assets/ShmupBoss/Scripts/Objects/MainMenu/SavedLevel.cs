namespace ShmupBoss
{
    /// <summary>
    /// Holds information on level completion and its associated high score and upgrades.
    /// </summary>
    [System.Serializable]
    public class SavedLevel
    {
        public LevelStatus Status = LevelStatus.NotAvailable;
        public int HighScore;

        public int WeaponsUpgradeLevel;
        public int VisualUpgradeLevel;
    }
}